using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlConnector;
using Moq;
using Poliedro.Billing.Api.Endpoints.v1.Billing;
using Poliedro.Billing.Application;
using Poliedro.Billing.Application.Billing.AutoMappers;
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
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Billing;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingTemporaryMySqlEmissionTest
{
    [IsolatedMySqlFact]
    public async Task Billing_endpoint_simulates_provider_acceptance_and_reads_back_invoice_from_mysql()
    {
        var connectionString = Environment.GetEnvironmentVariable("BILLING_QA_MYSQL_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connectionString));

        await using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            await using var create = new MySqlCommand("""
                CREATE TABLE IF NOT EXISTS invoice_success (
                    invoice INT NOT NULL,
                    providerid INT NOT NULL,
                    dateregister DATETIME(6) NOT NULL,
                    cude VARCHAR(200) NOT NULL,
                    qrCode TEXT NOT NULL,
                    client_id INT NOT NULL,
                    verify VARCHAR(100) NOT NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
                """, connection);
            await create.ExecuteNonQueryAsync();
        }

        const int generatedNumber = 9001;
        const int testClientId = 91001;
        const int testProviderId = 919;
        const string syntheticCude = "QA-SYNTHETIC-CUDE-9001";
        const string syntheticQr = "QA-SYNTHETIC-QR-9001";
        await using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            await using var removePriorRun = new MySqlCommand(
                "DELETE FROM invoice_success WHERE invoice=@invoice AND client_id=@client AND cude=@cude", connection);
            removePriorRun.Parameters.AddWithValue("@invoice", generatedNumber);
            removePriorRun.Parameters.AddWithValue("@client", testClientId);
            removePriorRun.Parameters.AddWithValue("@cude", syntheticCude);
            await removePriorRun.ExecuteNonQueryAsync();
        }

        var resolution = new DianResolutionEntity
        {
            ResolutionId = 57, CompanyProviderId = 5, ResolutionType = ResolutionType.FE,
            Prefix = "QA", CurrentRange = 9000, InitialRange = 1, FinalRange = 10000,
            Active = true, ExpirationDate = DateTime.Today.AddDays(10)
        };
        var companyProvider = new CompanyProviderEntity
        {
            CompanyProviderId = 5, CompanyId = testClientId, ProviderId = testProviderId,
            ServiceId = 4, Active = 1
        };
        var client = new ClientEntity { CompanyId = testClientId, Name = "CLIENTE QA SINTÉTICO", Active = true };

        var resolutionLookup = new Mock<IDianResolutionGetByIdService>();
        resolutionLookup.Setup(x => x.GetByIdAsync(57, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DianResolutionEntity, Error>.Success(resolution));
        var companyLookup = new Mock<ICompanyProviderGetByIdService>();
        companyLookup.Setup(x => x.GetCompanyProviderByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(companyProvider);
        var clientLookup = new Mock<IClientGetByIdService>();
        clientLookup.Setup(x => x.GetByIdAsync(testClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ClientEntity, Error>.Success(client));

        var provider = new SyntheticBillingProvider(testProviderId, syntheticCude, syntheticQr);
        var adapterFactory = new Mock<IBillingProviderAdapterFactory>();
        adapterFactory.Setup(x => x.Resolve(testProviderId)).Returns(provider);
        var processor = new SyntheticInvoiceProcessor(generatedNumber);
        var processorFactory = new Mock<IGetProcessorBilling>();
        processorFactory.Setup(x => x.GetProcessorAsync(ResolutionType.FE)).ReturnsAsync(processor);
        var serverLookup = new Mock<IServerGetByIdService>();
        serverLookup.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ServerEntity, Error>.Success(new ServerEntity { ServerId = 4 }));
        var connectionFactory = new Mock<IDatabaseUtils>();
        connectionFactory.Setup(x => x.GetConnectionString(It.IsAny<ServerEntity>())).Returns(connectionString!);
        var sequence = new Mock<IUpdateCurrentlyNumber>();
        sequence.Setup(x => x.UpdateCurrentlyNumberAsync(It.IsAny<ParametersCurrentlyNumber>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var recorder = new BillingEmissionResultRecorder(new MySqlInvoiceEmissionStore(), sequence.Object, serverLookup.Object, connectionFactory.Object);
        var license = new Mock<IBillingLicenseAuthorizer>();
        license.Setup(x => x.AuthorizeAsync(testClientId, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BillingLicenseDecision(true, "qa-synthetic"));
        var failures = new Mock<IEmissionFailureRepository>();
        failures.Setup(x => x.RecordAsync(It.IsAny<EmissionFailure>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var mapperServices = new ServiceCollection();
        mapperServices.AddApplication();
        using var mapperProvider = mapperServices.BuildServiceProvider();
        var mapper = mapperProvider.GetRequiredService<IMapper>();

        var handler = new CreateBillingHandler(
            resolutionLookup.Object, companyLookup.Object, processorFactory.Object, adapterFactory.Object,
            recorder, mapper, NullLogger<CreateBillingHandler>.Instance, clientLookup.Object,
            license.Object, failures.Object);
        var mediator = DispatchProxy.Create<IMediator, ExecutingBillingMediator>();
        ((ExecutingBillingMediator)(object)mediator).Execute = handler.Handle;

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(mediator);
        var app = builder.Build();
        app.MapGroup("/api/v1/billing").MapBillingEndpoints();
        await app.StartAsync();

        using var http = app.GetTestClient();
        var response = await http.PostAsJsonAsync("/api/v1/billing/57", new
        {
            data = new[] { new { number = "INPUT-ONLY" } }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        using var resultJson = JsonDocument.Parse(responseContent);
        Assert.True(resultJson.RootElement[0].GetProperty("status").GetBoolean());
        Assert.True(provider.SawPreparedInvoice, "The billing handler did not submit its prepared invoice to the simulated provider.");

        await using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            await using var readBack = new MySqlCommand(
                "SELECT invoice, providerid, cude, qrCode, client_id, verify FROM invoice_success WHERE invoice=@invoice AND client_id=@client AND cude=@cude ORDER BY dateregister DESC LIMIT 1", connection);
            readBack.Parameters.AddWithValue("@invoice", generatedNumber);
            readBack.Parameters.AddWithValue("@client", testClientId);
            readBack.Parameters.AddWithValue("@cude", syntheticCude);
            await using var row = await readBack.ExecuteReaderAsync();
            Assert.True(await row.ReadAsync(), "The accepted synthetic invoice was not persisted in MySQL invoice_success.");
            Assert.Equal(generatedNumber, row.GetInt32("invoice"));
            Assert.Equal(testProviderId, row.GetInt32("providerid"));
            Assert.Equal(syntheticCude, row.GetString("cude"));
            Assert.Equal(syntheticQr, row.GetString("qrCode"));
            Assert.Equal(testClientId, row.GetInt32("client_id"));
            Assert.Equal("QA-9001", row.GetString("verify"));
        }

        await app.StopAsync();
        await app.DisposeAsync();
    }

    private sealed class SyntheticInvoiceProcessor(int number) : ICreateBilling
    {
        public Task<IEnumerable<BillingPreparationResult>> CreateInvoicesAsync(
            IEnumerable<CreateBilling> invoices, DianResolutionEntity resolution,
            CompanyProviderEntity companyProvider, int lastInvoiceNumber, CancellationToken cancellationToken)
        {
            var billing = new CreateBilling { Number = "QA-9001", Numeration = number.ToString(), Prefix = "QA" };
            var document = PreparedBillingDocument.FromElectronicInvoice(new FERetailelectronicEntity
            {
                date = "2026-10-23", time = "12:00:00", prefix = "QA", number = number,
                orderReference = new OrderReferenceEntity(), send_email = false,
                customer = new CustomerEntity { TypeOfPerson = 2, IdentificationNumber = "900000001", Name = "CLIENTE QA SINTÉTICO" },
                payment = new PaymentEntity { PaymentFormId = 1 }, items = [], resolution = "QA-57",
                invoiceBaseTotal = 100, invoiceTaxExclusiveTotal = 100,
                invoiceTaxInclusiveTotal = 100, totalToPay = 100, finalTotalToPay = 100
            });
            return Task.FromResult<IEnumerable<BillingPreparationResult>>([new(billing, document, null)]);
        }
    }

    private sealed class SyntheticBillingProvider(int providerId, string cude, string qrCode) : IBillingProviderAdapter
    {
        public int ProviderId => providerId;
        public string ProviderKey => "qa-synthetic";
        public bool SawPreparedInvoice { get; private set; }
        public ProviderConfigurationValidation ValidateConfiguration(CompanyProviderEntity companyProvider) => ProviderConfigurationValidation.Valid();
        public Task<int> GetLastNumberAsync(DianResolutionEntity resolution, CompanyProviderEntity companyProvider, CancellationToken cancellationToken) => Task.FromResult(9000);
        public Task<IReadOnlyList<BillingProviderResponse>> SendAsync(BillingProviderSubmission submission, CancellationToken cancellationToken)
        {
            SawPreparedInvoice = submission.PreparedDocuments.Count == 1 && submission.PreparedDocuments[0].ElectronicInvoice?.number == 9001;
            return Task.FromResult<IReadOnlyList<BillingProviderResponse>>([new(200, true, "ACEPTADA EN SIMULACIÓN", cude, qrCode, null)]);
        }
    }

    public class ExecutingBillingMediator : DispatchProxy
    {
        public Func<CreateBillingCommand, CancellationToken, Task<CreateBillingCommandResult>>? Execute { get; set; }
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod?.Name == "Send" && args?.FirstOrDefault() is CreateBillingCommand command)
                return Execute!(command, args.Length > 1 && args[1] is CancellationToken ct ? ct : CancellationToken.None);
            throw new NotSupportedException($"Unexpected mediator method: {targetMethod?.Name}");
        }
    }
}

public sealed class IsolatedMySqlFactAttribute : FactAttribute
{
    public IsolatedMySqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BILLING_QA_MYSQL_CONNECTION")))
            Skip = "Set BILLING_QA_MYSQL_CONNECTION to run this isolated MySQL end-to-end test.";
    }
}
