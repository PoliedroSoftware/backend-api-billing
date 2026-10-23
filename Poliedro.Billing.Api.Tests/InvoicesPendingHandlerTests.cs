using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class InvoicesPendingHandlerTests
{
    [Fact]
    public async Task Pending_invoice_query_stops_when_resolution_is_missing()
    {
        var resolution = new Mock<IDianResolutionGetByIdService>();
        resolution.Setup(x => x.GetByIdAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DianResolutionEntity, Error>.Failure(Error.CreateInstance("missing", "missing", HttpStatusCode.NotFound)));
        var provider = new Mock<ICompanyProviderGetByIdService>(MockBehavior.Strict);
        var handler = CreateHandler(resolution.Object, provider.Object, Mock.Of<IServerGetByIdService>(), Mock.Of<IMapper>(), Mock.Of<IInvoicesPendingWithDetailsStrategyFactory>());

        Assert.Null(await handler.Handle(new InvoicesPendingWithDetailsQuery(77), CancellationToken.None));
        provider.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Pending_invoice_query_stops_for_missing_company_provider_or_server()
    {
        var resolution = ResolutionService();
        var missingProvider = new Mock<ICompanyProviderGetByIdService>();
        missingProvider.Setup(x => x.GetCompanyProviderByIdAsync(22, It.IsAny<CancellationToken>())).ReturnsAsync((CompanyProviderEntity)null!);
        var handler = CreateHandler(resolution.Object, missingProvider.Object, Mock.Of<IServerGetByIdService>(), Mock.Of<IMapper>(), Mock.Of<IInvoicesPendingWithDetailsStrategyFactory>());
        Assert.Null(await handler.Handle(new InvoicesPendingWithDetailsQuery(77), CancellationToken.None));

        var provider = ProviderService();
        var missingServer = new Mock<IServerGetByIdService>();
        missingServer.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ServerEntity, Error>.Failure(Error.CreateInstance("missing", "missing", HttpStatusCode.NotFound)));
        handler = CreateHandler(resolution.Object, provider.Object, missingServer.Object, Mock.Of<IMapper>(), Mock.Of<IInvoicesPendingWithDetailsStrategyFactory>());
        Assert.Null(await handler.Handle(new InvoicesPendingWithDetailsQuery(77), CancellationToken.None));
    }

    [Fact]
    public async Task Pending_invoice_query_uses_resolution_strategy_and_returns_mapped_invoices()
    {
        var resolution = ResolutionService();
        var provider = ProviderService();
        var server = new ServerEntity { ServerId = 5 };
        var serverService = new Mock<IServerGetByIdService>();
        serverService.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(Result<ServerEntity, Error>.Success(server));
        var strategy = new Mock<IInvoicesPendingWithDetailsStrategy>();
        var source = new[] { new CreateBilling { Number = "INV-1" } };
        strategy.Setup(x => x.GetAllInvoicePendingWithDetails(server, It.IsAny<DianResolutionEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(source);
        var factory = new Mock<IInvoicesPendingWithDetailsStrategyFactory>();
        factory.Setup(x => x.GetStrategy(ResolutionType.FE)).Returns(strategy.Object);
        var mapper = new Mock<IMapper>();
        var mapped = new List<CreateBillingDTO>();
        mapper.Setup(x => x.Map<List<CreateBillingDTO>>(source)).Returns(mapped);
        var handler = CreateHandler(resolution.Object, provider.Object, serverService.Object, mapper.Object, factory.Object);

        var result = await handler.Handle(new InvoicesPendingWithDetailsQuery(77), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(0, result.Count);
        Assert.Same(mapped, result.Data);
    }

    private static Mock<IDianResolutionGetByIdService> ResolutionService()
    {
        var mock = new Mock<IDianResolutionGetByIdService>();
        mock.Setup(x => x.GetByIdAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DianResolutionEntity, Error>.Success(new DianResolutionEntity { ResolutionId = 77, CompanyProviderId = 22, ResolutionType = ResolutionType.FE }));
        return mock;
    }

    private static Mock<ICompanyProviderGetByIdService> ProviderService()
    {
        var mock = new Mock<ICompanyProviderGetByIdService>();
        mock.Setup(x => x.GetCompanyProviderByIdAsync(22, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyProviderEntity { CompanyProviderId = 22, ServiceId = 5 });
        return mock;
    }

    private static InvoicesPendingWithDetailsHandler CreateHandler(IDianResolutionGetByIdService resolution,
        ICompanyProviderGetByIdService provider, IServerGetByIdService server, IMapper mapper,
        IInvoicesPendingWithDetailsStrategyFactory factory)
        => new(resolution, server, provider, mapper, Mock.Of<IDatabaseUtils>(), factory,
            NullLogger<InvoicesPendingWithDetailsHandler>.Instance);
}
