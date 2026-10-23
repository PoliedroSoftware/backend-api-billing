using Moq;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Billing;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingResponsePersistenceTests
{
    [Fact]
    public async Task Confirmed_provider_response_persists_invoice_and_advances_resolution_sequence()
    {
        var store = new Mock<IInvoiceEmissionStore>();
        var sequence = new Mock<IUpdateCurrentlyNumber>();
        var servers = new Mock<IServerGetByIdService>();
        var database = new Mock<IDatabaseUtils>();
        var server = new ServerEntity { ServerId = 4 };
        servers.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(Result<ServerEntity, Error>.Success(server));
        database.Setup(x => x.GetConnectionString(server)).Returns("isolated-test-connection");
        store.Setup(x => x.RecordSuccessAsync(42, "cude-test", "qr-test", "isolated-test-connection", 1, 3, "INV-42", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sequence.Setup(x => x.UpdateCurrentlyNumberAsync(It.Is<ParametersCurrentlyNumber>(p => p.Invoice == 42 && p.ResolutionId == 8 && p.LastedInvoiced == "INV-42"), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var recorder = new BillingEmissionResultRecorder(store.Object, sequence.Object, servers.Object, database.Object);

        await recorder.RecordAsync(
            [new BillingProviderResponse(200, true, "accepted", "cude-test", "qr-test", null)],
            [new CreateBilling { Number = "INV-42", Numeration = "42" }],
            new DianResolutionEntity { ResolutionId = 8 },
            new CompanyProviderEntity { CompanyProviderId = 5, CompanyId = 3, ProviderId = 1, ServiceId = 4 },
            CancellationToken.None);

        store.VerifyAll();
        sequence.VerifyAll();
    }

    [Fact]
    public async Task Missing_provider_identifiers_are_rejected_before_persisting()
    {
        var store = new Mock<IInvoiceEmissionStore>(MockBehavior.Strict);
        var servers = new Mock<IServerGetByIdService>();
        var server = new ServerEntity { ServerId = 4 };
        servers.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(Result<ServerEntity, Error>.Success(server));
        var database = new Mock<IDatabaseUtils>();
        database.Setup(x => x.GetConnectionString(server)).Returns("isolated-test-connection");
        var recorder = new BillingEmissionResultRecorder(store.Object, Mock.Of<IUpdateCurrentlyNumber>(), servers.Object, database.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => recorder.RecordAsync(
            [new BillingProviderResponse(200, true, "accepted", null, "qr", null)],
            [new CreateBilling { Number = "INV-42", Numeration = "42" }],
            new DianResolutionEntity { ResolutionId = 8 },
            new CompanyProviderEntity { CompanyProviderId = 5, CompanyId = 3, ProviderId = 1, ServiceId = 4 },
            CancellationToken.None));

        store.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Missing_server_fails_before_invoice_store_is_called()
    {
        var servers = new Mock<IServerGetByIdService>();
        servers.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ServerEntity, Error>.Failure(Error.CreateInstance("missing", "missing", HttpStatusCode.NotFound)));
        var store = new Mock<IInvoiceEmissionStore>(MockBehavior.Strict);
        var recorder = new BillingEmissionResultRecorder(store.Object, Mock.Of<IUpdateCurrentlyNumber>(), servers.Object, Mock.Of<IDatabaseUtils>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => recorder.RecordAsync(
            [new BillingProviderResponse(200, true, "accepted", "cude", "qr", null)],
            [new CreateBilling { Number = "INV-42", Numeration = "42" }],
            new DianResolutionEntity(),
            new CompanyProviderEntity { CompanyProviderId = 5, ServiceId = 4 },
            CancellationToken.None));

        store.VerifyNoOtherCalls();
    }
}
