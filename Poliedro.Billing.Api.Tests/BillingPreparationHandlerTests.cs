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
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingPreparationHandlerTests
{
    [Fact]
    public async Task Preparation_failure_is_returned_without_sending_or_recording_an_emission_failure()
    {
        const int providerId = 1;
        var resolution = new DianResolutionEntity { ResolutionId = 7, CompanyProviderId = 11, ResolutionType = ResolutionType.FE, Prefix = "TST", Active = true };
        var companyProvider = new CompanyProviderEntity { CompanyProviderId = 11, CompanyId = 5, ProviderId = providerId, Active = 1, ApiKey = "test-key" };
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
        clientService.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(Result<ClientEntity, Error>.Success(new ClientEntity { CompanyId = 5, Name = "Test", Active = true }));
        authorizer.Setup(x => x.AuthorizeAsync(5, 11, It.IsAny<CancellationToken>())).ReturnsAsync(new BillingLicenseDecision(true, "active"));
        mapper.Setup(x => x.Map<IEnumerable<CreateBilling>>(It.IsAny<IEnumerable<CreateBillingDTO>>())).Returns([billing]);
        processorFactory.Setup(x => x.GetProcessorAsync(ResolutionType.FE)).ReturnsAsync(processor.Object);
        processor.Setup(x => x.CreateInvoicesAsync(It.IsAny<IEnumerable<CreateBilling>>(), resolution, companyProvider, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BillingPreparationResult(billing, null, "Detalle inválido")]);
        adapterFactory.Setup(x => x.Resolve(providerId)).Returns(adapter.Object);
        adapter.SetupGet(x => x.ProviderId).Returns(providerId);
        adapter.Setup(x => x.ValidateConfiguration(companyProvider)).Returns(ProviderConfigurationValidation.Valid());
        adapter.Setup(x => x.GetLastNumberAsync(resolution, companyProvider, It.IsAny<CancellationToken>())).ReturnsAsync(10);

        var handler = new CreateBillingHandler(resolutionService.Object, providerService.Object, processorFactory.Object,
            adapterFactory.Object, recorder.Object, mapper.Object, NullLogger<CreateBillingHandler>.Instance,
            clientService.Object, authorizer.Object, failures.Object);
        var result = await handler.Handle(new CreateBillingCommand(7, [InvoiceDto("42")]), CancellationToken.None);

        Assert.True(result.Success);
        var invoiceResult = Assert.Single(result.Results!);
        Assert.False(invoiceResult.Status);
        Assert.Equal("Detalle inválido", invoiceResult.Message);
        adapter.Verify(x => x.SendAsync(It.IsAny<BillingProviderSubmission>(), It.IsAny<CancellationToken>()), Times.Never);
        recorder.VerifyNoOtherCalls();
        failures.VerifyNoOtherCalls();
    }

    private static CreateBillingDTO InvoiceDto(string number) => new(
        DateTime.Today, DateTime.Now, null, null, null, number, "TST", null, null, false,
        null, null, null, null, null, null, null, null, null, null, null, null, null, null,
        0, 0, 0, 0, 0, null, null, 0);
}
