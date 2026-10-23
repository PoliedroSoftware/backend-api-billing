using Moq;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Ports;
using ItemValidator = Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.ItemValidator;
using CustomerValidator = Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.CustomerValidator;
using TaxTotalValidator = Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.TaxTotalValidator;

namespace Poliedro.Billing.Api.Tests;

public sealed class FeRetailApplicationTests
{
    [Fact]
    public async Task Fe_retail_command_handler_sends_all_loaded_clients_to_provider()
    {
        var clients = new[] { new ClientEntity { CompanyId = 1 }, new ClientEntity { CompanyId = 2 } };
        var clientService = new Mock<IClientDomainService>();
        var feRetailService = new Mock<IFERetailService>();
        var response = new BillingProviderResponse(200, true, "accepted", "cude", "qr", null);
        clientService.Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<ClientEntity>, Error>.Success(clients));
        feRetailService.Setup(service => service.CreateElectronicInvoicesAsync(clients, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<BillingProviderResponse, Error>.Success(response));

        var result = await new CreateFERetailHandle(clientService.Object, feRetailService.Object)
            .Handle(FakeCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        feRetailService.Verify(service => service.CreateElectronicInvoicesAsync(clients, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Fe_retail_command_handler_preserves_provider_failure()
    {
        var clients = new[] { new ClientEntity { CompanyId = 1 } };
        var clientService = new Mock<IClientDomainService>();
        var feRetailService = new Mock<IFERetailService>();
        var error = Error.CreateInstance("rejected", "Provider rejected invoice", System.Net.HttpStatusCode.BadRequest);
        clientService.Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<ClientEntity>, Error>.Success(clients));
        feRetailService.Setup(service => service.CreateElectronicInvoicesAsync(clients, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<BillingProviderResponse, Error>.Failure(error));

        var result = await new CreateFERetailHandle(clientService.Object, feRetailService.Object)
            .Handle(FakeCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Fe_retail_nested_validators_accept_valid_provider_payloads()
    {
        var messages = new BillingTestMessages();
        var item = new ItemElectronicEntity
        {
            UnitMeasureId = 70, LineExtensionAmount = 100, FreeOfChargeIndicator = false,
            AllowanceCharges = [], Description = "Service", PriceAmount = 100, Code = 1,
            TypeItemIdentificationId = 1, BaseQuantity = 1, InvoicedQuantity = 1
        };
        var customer = new Customer("123", "4", "Customer", "3000000000", "Street 1", "customer@example.com",
            "R-123", 1, 1, 1, 1, "05001", 1);

        Assert.True(new ItemValidator(messages).Validate(item).IsValid);
        Assert.True(new CustomerValidator(messages).Validate(customer).IsValid);
        Assert.True(new AllowanceChargeValidator(messages).Validate(new AllowanceCharge(false, "Discount", .1m, 10m, 100m)).IsValid);
        Assert.True(new GeneralAllowanceValidator(messages).Validate(new GeneralAllowance("Discount", 10m, 10m, 100m)).IsValid);
        Assert.True(new TaxTotalValidator(messages).Validate(new TaxTotal(1, 19m, 19m, 100m)).IsValid);
    }

    [Fact]
    public void Fe_retail_nested_validators_reject_invalid_ranges_and_missing_fields()
    {
        var messages = new BillingTestMessages();
        var invalidItem = new ItemElectronicEntity
        {
            UnitMeasureId = 0, LineExtensionAmount = 0, FreeOfChargeIndicator = false,
            AllowanceCharges = null, Description = "", PriceAmount = 0, Code = null,
            TypeItemIdentificationId = 0, BaseQuantity = 0, InvoicedQuantity = 0
        };
        var invalidCustomer = new Customer("", "", "", "", "", "", "", 0, 0, 0, 0, "", 0);

        Assert.False(new ItemValidator(messages).Validate(invalidItem).IsValid);
        Assert.False(new CustomerValidator(messages).Validate(invalidCustomer).IsValid);
        Assert.False(new AllowanceChargeValidator(messages).Validate(new AllowanceCharge(false, "", 2m, 0m, 0m)).IsValid);
        Assert.False(new GeneralAllowanceValidator(messages).Validate(new GeneralAllowance("", 101m, -1m, 0m)).IsValid);
        Assert.False(new TaxTotalValidator(messages).Validate(new TaxTotal(1, -1m, 101m, 100m)).IsValid);
    }

    private static CreateFERetailCommand FakeCommand() => new(
        "2026-01-01", "12:00:00", "TST", 1, new OrderReference("test"), false,
        new Attachment("one.pdf", "AA=="), new Attachment("two.pdf", "AA=="),
        new Customer("1", "1", "test", "1", "test", "test@example.com", "1", 1, 1, 1, 1, "1", 1),
        new Payment(1, 1, "2026-01-01", "1"), [], [], "test", "test", "", "", "", 0, 0, 0, 0, 0, [], [], [], 0);

    private sealed class BillingTestMessages : IMessageProvider
    {
        public string ErrorValidatorFieldNotNull => "required";
        public string ErrorValidatorFieldNotEmpty => "required";
        public string ErrorValidatorFieldGreatherThanZero => "positive";
        public string ErrorValidatorFieldLessThanTwelve => "range";
        public string ErrorValidatorResolutionTypeValid => "enum";
        public string ErrorValidatorFieldPositive => "positive";
        public string ErrorValidatorFieldMustBeGreaterThanZero => "positive";
        public string ErrorValidatorFieldMustBeNonNegative => "non-negative";
        public string ErrorValidatorFieldMustBePercentage => "percentage";
        public string ErrorValidatorInvalidEmail => "email";
        public string ErrorValidatorFieldMustBeGreaterThanOrEqualToZero => "non-negative";
        public string ErrorValidatorFieldIsRequired => "required";
    }
}
