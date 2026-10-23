using Moq;
using Poliedro.Billing.Application.Siigo.Commands;
using Poliedro.Billing.Application.Siigo.Ports;
using Poliedro.Billing.Application.Siigo.Contracts.Models;
using Poliedro.Billing.Application.Siigo.Contracts.Responses;
using Poliedro.Billing.Application.TNS.Commands;
using Poliedro.Billing.Application.Tns.Commands;
using Poliedro.Billing.Application.TNS.Ports;
using Poliedro.Billing.Application.TNS.Contracts.Models;
using Poliedro.Billing.Application.TNS.Contracts.Responses;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Api.Tests;

public sealed class ProviderCommandHandlerTests
{
    [Fact]
    public async Task Siigo_handler_returns_provider_success_and_forwards_request()
    {
        var service = new Mock<ISiigoGateway>();
        var invoices = new List<Invoice>();
        var response = new ApiResponseSiigo();
        service.Setup(x => x.CreateAndSendDianAsync(invoices, "token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApiResponseSiigo, Error>.Success(response));

        var result = await new CreateInvoicesSiigoHandler(service.Object)
            .Handle(new CreateInvoicesSiigoCommand(invoices, "token"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        service.Verify(x => x.CreateAndSendDianAsync(invoices, "token", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Siigo_handler_preserves_provider_error()
    {
        var service = new Mock<ISiigoGateway>();
        var invoices = new List<Invoice>();
        var error = Error.CreateInstance("provider", "rejected", System.Net.HttpStatusCode.BadRequest);
        service.Setup(x => x.CreateAndSendDianAsync(invoices, "token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApiResponseSiigo, Error>.Failure(error));

        var result = await new CreateInvoicesSiigoHandler(service.Object)
            .Handle(new CreateInvoicesSiigoCommand(invoices, "token"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Tns_handler_returns_provider_success_and_forwards_request()
    {
        var service = new Mock<ITNSGateway>();
        var orders = new List<Order>();
        var response = new ApiResponseTNS();
        service.Setup(x => x.CreateAsync(orders, "branch", "token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApiResponseTNS, Error>.Success(response));

        var result = await new CreateSalesTNSHandler(service.Object)
            .Handle(new CreateSalesTNSCommand(orders, "branch", "token"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        service.Verify(x => x.CreateAsync(orders, "branch", "token", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Tns_handler_preserves_provider_error()
    {
        var service = new Mock<ITNSGateway>();
        var orders = new List<Order>();
        var error = Error.CreateInstance("provider", "rejected", System.Net.HttpStatusCode.BadRequest);
        service.Setup(x => x.CreateAsync(orders, "branch", "token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApiResponseTNS, Error>.Failure(error));

        var result = await new CreateSalesTNSHandler(service.Object)
            .Handle(new CreateSalesTNSCommand(orders, "branch", "token"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }
}
