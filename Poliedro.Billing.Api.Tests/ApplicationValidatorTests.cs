using Moq;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.DeleteClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;
using Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Queries.GetServerById;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Api.Tests;

public sealed class ApplicationValidatorTests
{
    [Fact]
    public void Client_validators_accept_valid_commands_and_reject_missing_values()
    {
        var create = new CreateClientCommandValidator();
        Assert.True(create.Validate(new CreateClientCommand { Name = "Cliente", Active = true }).IsValid);
        Assert.False(create.Validate(new CreateClientCommand { Name = " " }).IsValid);
        Assert.False(new DeleteClientCommandValidator().Validate(new DeleteClientCommand { Id = 0 }).IsValid);
        Assert.True(new DeleteClientCommandValidator().Validate(new DeleteClientCommand { Id = 1 }).IsValid);
        Assert.False(new UpdateClientBillingElectronicCommandValidator().Validate(new UpdateClientCommand { CompanyId = 0, Name = "" }).IsValid);
        Assert.False(new GetClientBillingElectronicByIdQueryValidator().Validate(new GetClientByIdQuery { Id = 0 }).IsValid);
    }

    [Fact]
    public void Server_validators_enforce_connection_fields_and_positive_identifiers()
    {
        var valid = new CreateServerCommand { Ip = "127.0.0.1", DatabaseName = "billing_test", DbUsername = "test", DbPassword = "test" };
        Assert.True(new CreateServerCommandValidator().Validate(valid).IsValid);
        Assert.False(new CreateServerCommandValidator().Validate(new CreateServerCommand { Ip = "", DatabaseName = "", DbUsername = "", DbPassword = "" }).IsValid);
        Assert.False(new UpdateServerCommandValidator().Validate(new UpdateServerCommand { ServerId = 0 }).IsValid);
        Assert.False(new GetServerByIdQueryValidator().Validate(new GetServerByIdQuery { Id = 0 }).IsValid);
    }

    [Fact]
    public void Provider_validators_check_foreign_keys_and_environment_enum()
    {
        var dto = new CreateCompanyProviderDto(1, 2, 3, null, null, null, (EnvironmentType)0, null, null, 1);
        Assert.True(new CreateCompanyProviderCommandValidator().Validate(new CreateCompanyProviderCommand(dto)).IsValid);
        Assert.False(new CreateCompanyProviderCommandValidator().Validate(new CreateCompanyProviderCommand(dto with { CompanyId = 0 })).IsValid);

        var update = new UpdateCompanyProviderDto(1, 2, 3, null, null, null, (EnvironmentType)0, null, null, 1);
        Assert.False(new UpdateCompanyProviderCommandValidator().Validate(new UpdateCompanyProviderCommand(0, update)).IsValid);
    }

    [Fact]
    public void Dian_resolution_validator_checks_ranges_month_and_required_values()
    {
        var messages = new Mock<IMessageProvider>();
        messages.SetupGet(x => x.ErrorValidatorFieldNotNull).Returns("required");
        messages.SetupGet(x => x.ErrorValidatorFieldNotEmpty).Returns("required");
        messages.SetupGet(x => x.ErrorValidatorFieldGreatherThanZero).Returns("positive");
        messages.SetupGet(x => x.ErrorValidatorFieldLessThanTwelve).Returns("month");
        messages.SetupGet(x => x.ErrorValidatorResolutionTypeValid).Returns("enum");

        var validator = new CreateDianResolutionCommandValidator(messages.Object);
        var valid = new CreateDianResolutionCommand(1, 1, (ResolutionType)0, "123", "A", 0, 6, 0, 1, 100, 1, DateTime.Today, DateTime.Today.AddMonths(6), 0, 0, true);
        Assert.True(validator.Validate(valid).IsValid);
        Assert.False(validator.Validate(valid with { Prefix = "", InitialRange = 0, VigencyMonth = 12 }).IsValid);
    }

    [Fact]
    public void Dian_resolution_update_validator_accepts_partial_updates_and_rejects_invalid_supplied_values()
    {
        var messages = new Mock<IMessageProvider>();
        messages.SetupGet(x => x.ErrorValidatorFieldNotNull).Returns("required");
        messages.SetupGet(x => x.ErrorValidatorFieldNotEmpty).Returns("required");
        messages.SetupGet(x => x.ErrorValidatorFieldGreatherThanZero).Returns("positive");
        messages.SetupGet(x => x.ErrorValidatorFieldLessThanTwelve).Returns("month");
        messages.SetupGet(x => x.ErrorValidatorResolutionTypeValid).Returns("enum");
        var validator = new UpdateDianResolutionCommandValidator(messages.Object);
        var validPartial = new UpdateDianResolutionCommand(1, 2);

        Assert.True(validator.Validate(validPartial).IsValid);
        Assert.False(validator.Validate(validPartial with { Resolutionid = 0 }).IsValid);
        Assert.False(validator.Validate(validPartial with { InitialRange = -1 }).IsValid);
        Assert.False(validator.Validate(validPartial with { FinalRange = -1 }).IsValid);
        Assert.False(validator.Validate(validPartial with { VigencyMonth = 12 }).IsValid);
        Assert.False(validator.Validate(validPartial with { VigencyMonth = -1 }).IsValid);
        Assert.False(validator.Validate(validPartial with { ResolutionType = (ResolutionType)999 }).IsValid);
        Assert.True(validator.Validate(validPartial with
        {
            ResolutionNumber = "R-1", Prefix = "A", InitialRange = 1, FinalRange = 100,
            VigencyMonth = 11, ResolutionDate = DateTime.Today,
            ExpirationDate = DateTime.Today.AddMonths(11), ResolutionType = ResolutionType.FE
        }).IsValid);
    }
}
