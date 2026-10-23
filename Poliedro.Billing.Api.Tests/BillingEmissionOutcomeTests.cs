using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingEmissionOutcomeTests
{
    [Fact]
    public async Task Empty_provider_response_is_recorded_for_follow_up()
    {
        var fixture = CreateFixture();
        fixture.Adapter.Setup(x => x.SendAsync(It.IsAny<BillingProviderSubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await fixture.Handler.Handle(Command(), CancellationToken.None);

        Assert.False(Assert.Single(result.Results!).Status);
        fixture.Failures.Verify(x => x.RecordAsync(It.Is<EmissionFailure>(f => f.InvoiceNumber == "42"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejected_provider_response_is_reported_and_recorded()
    {
        var fixture = CreateFixture();
        fixture.Adapter.Setup(x => x.SendAsync(It.IsAny<BillingProviderSubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BillingProviderResponse(400, false, "DIAN rechazó el documento", null, null, null)]);

        var result = await fixture.Handler.Handle(Command(), CancellationToken.None);

        var invoice = Assert.Single(result.Results!);
        Assert.False(invoice.Status);
        Assert.Equal("DIAN rechazó el documento", invoice.Message);
        fixture.Failures.Verify(x => x.RecordAsync(It.Is<EmissionFailure>(f => f.Reason == "DIAN rechazó el documento"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Adapter_exception_is_recorded_and_returned()
    {
        var fixture = CreateFixture();
        fixture.Adapter.Setup(x => x.SendAsync(It.IsAny<BillingProviderSubmission>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("  provider temporary failure  "));

        var result = await fixture.Handler.Handle(Command(), CancellationToken.None);

        Assert.Equal("provider temporary failure", Assert.Single(result.Results!).Message);
        fixture.Failures.Verify(x => x.RecordAsync(It.Is<EmissionFailure>(f => f.Reason == "provider temporary failure"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Confirmed_emission_with_local_persistence_error_is_not_recorded_as_provider_failure()
    {
        var fixture = CreateFixture();
        fixture.Adapter.Setup(x => x.SendAsync(It.IsAny<BillingProviderSubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BillingProviderResponse(200, true, "accepted", "cude", "qr", null)]);
        fixture.Recorder.Setup(x => x.RecordAsync(It.IsAny<IReadOnlyList<BillingProviderResponse>>(), It.IsAny<IEnumerable<CreateBilling>>(),
                It.IsAny<DianResolutionEntity>(), It.IsAny<CompanyProviderEntity>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("local write failed"));

        var result = await fixture.Handler.Handle(Command(), CancellationToken.None);

        var invoice = Assert.Single(result.Results!);
        Assert.False(invoice.Status);
        Assert.Contains("fue emitida pero falló la persistencia local", invoice.Message);
        fixture.Failures.VerifyNoOtherCalls();
    }

    private static CreateBillingCommand Command() => new(7, [new CreateBillingDTO(
        DateTime.Today, DateTime.Now, null, null, null, "42", "TST", null, null, false,
        null, null, null, null, null, null, null, null, null, null, null, null, null, null,
        0, 0, 0, 0, 0, null, null, 0)]);

    private static Fixture CreateFixture()
    {
        const int providerId = 1;
        var resolution = new DianResolutionEntity { ResolutionId = 7, CompanyProviderId = 11, ResolutionType = ResolutionType.FE, Prefix = "TST", Active = true };
        var companyProvider = new CompanyProviderEntity { CompanyProviderId = 11, CompanyId = 5, ProviderId = providerId, Active = 1, ApiKey = "test-key" };
        var client = new ClientEntity { CompanyId = 5, Name = "Test client", Active = true };
        var billing = new CreateBilling { Number = "42", Prefix = "TST" };
        var resolutionService = new Mock<IDianResolutionGetByIdService>();
        var providerService = new Mock<ICompanyProviderGetByIdService>();
        var clientService = new Mock<IClientGetByIdService>();
        var authorizer = new Mock<IBillingLicenseAuthorizer>();
        var processorFactory = new Mock<IGetProcessorBilling>();
        var processor = new Mock<ICreateBilling>();
        var adapterFactory = new Mock<IBillingProviderAdapterFactory>();
        var adapter = new Mock<IBillingProviderAdapter>();
        var recorder = new Mock<IBillingEmissionResultRecorder>();
        var mapper = new Mock<IMapper>();
        var failures = new Mock<IEmissionFailureRepository>();

        resolutionService.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(Result<DianResolutionEntity, Error>.Success(resolution));
        providerService.Setup(x => x.GetCompanyProviderByIdAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(companyProvider);
        clientService.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(Result<ClientEntity, Error>.Success(client));
        authorizer.Setup(x => x.AuthorizeAsync(5, 11, It.IsAny<CancellationToken>())).ReturnsAsync(new BillingLicenseDecision(true, "active"));
        processorFactory.Setup(x => x.GetProcessorAsync(ResolutionType.FE)).ReturnsAsync(processor.Object);
        adapterFactory.Setup(x => x.Resolve(providerId)).Returns(adapter.Object);
        adapter.SetupGet(x => x.ProviderId).Returns(providerId);
        adapter.Setup(x => x.ValidateConfiguration(companyProvider)).Returns(ProviderConfigurationValidation.Valid());
        adapter.Setup(x => x.GetLastNumberAsync(resolution, companyProvider, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<IEnumerable<CreateBilling>>(It.IsAny<IEnumerable<CreateBillingDTO>>())).Returns([billing]);
        processor.Setup(x => x.CreateInvoicesAsync(It.IsAny<IEnumerable<CreateBilling>>(), resolution, companyProvider, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BillingPreparationResult(billing, PreparedBillingDocument.FromElectronicInvoice(PreparedInvoice()), null)]);

        var handler = new CreateBillingHandler(resolutionService.Object, providerService.Object, processorFactory.Object,
            adapterFactory.Object, recorder.Object, mapper.Object, NullLogger<CreateBillingHandler>.Instance,
            clientService.Object, authorizer.Object, failures.Object);
        return new Fixture(handler, adapter, recorder, failures);
    }

    private sealed record Fixture(CreateBillingHandler Handler, Mock<IBillingProviderAdapter> Adapter,
        Mock<IBillingEmissionResultRecorder> Recorder, Mock<IEmissionFailureRepository> Failures);

    private static FERetailelectronicEntity PreparedInvoice() => new()
    {
        date = "2026-01-01", time = "10:00:00", prefix = "TST", number = 1,
        orderReference = new OrderReferenceEntity { IdOrder = "test" }, send_email = false,
        customer = new CustomerEntity(), payment = new PaymentEntity(), items = [], resolution = "test",
        invoiceBaseTotal = 0, invoiceTaxExclusiveTotal = 0, invoiceTaxInclusiveTotal = 0,
        totalToPay = 0, finalTotalToPay = 0
    };
}
