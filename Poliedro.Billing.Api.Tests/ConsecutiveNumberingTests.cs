using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Poliedro.Billing.Application.Billing.AutoMappers;
using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Application.Billing.Ports.Providers.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi.Helpers;

namespace Poliedro.Billing.Api.Tests;

/// <summary>
/// La numeración fiscal (consecutivo) la asigna la capa externa del proveedor justo
/// antes del envío. El "number" que entra en el request es el número de factura del
/// POS y solo alimenta head_note/foot_note.
/// </summary>
public class ConsecutiveNumberingTests
{
    private static readonly IMapper Mapper =
        new MapperConfiguration(config => config.AddProfile(new BillingAutoMapper())).CreateMapper();

    private static readonly DianResolutionEntity Resolution = new()
    {
        ResolutionId = 1,
        InitialRange = 1,
        FinalRange = 1000,
        ExpirationDate = DateTime.Now.AddYears(1),
        Prefix = "FE"
    };

    private static readonly CompanyProviderEntity Company = new() { ApiKey = "test-key" };

    [Fact]
    public async Task Prepare_KeepsPosNumberInNotes_AndLeavesConsecutiveEmpty()
    {
        var prepare = new PrepareBillingFE(
            new IdentityItems(),
            new NoTaxTotals(),
            new AlwaysValidCheckDigit(),
            new ConfigurationBuilder().Build());

        var invoice = new CreateBilling
        {
            Number = "3703",
            CustomerEntity = new CustomerEntity
            {
                IdentificationNumber = "222222222222",
                Name = "Consumidor Final",
                Email = "cliente@prueba.com",
                TypeOfPerson = 3
            },
            PaymentEntity = new PaymentEntity { PaymentFormId = 1 },
            ItemElectronicEntity =
            [
                new ItemElectronicEntity
                {
                    Description = "COFFY TIPS X100PCS",
                    Code = 275,
                    InvoicedQuantity = 1,
                    UnitPrice = 12000,
                    PriceAmount = 12000,
                    LineExtensionAmount = 12000,
                    Percent = 0,
                    TaxTotals = []
                }
            ],
            TransactionDate = new DateTime(2026, 11, 11, 10, 54, 0)
        };

        var results = (await prepare.CreateInvoicesAsync(
            [invoice],
            new DianResolutionEntity { Prefix = "FE", ResolutionNumber = "1876", Description = "Resolución" },
            new CompanyProviderEntity(),
            default)).ToList();

        var document = Assert.IsType<FERetailelectronicEntity>(Assert.Single(results).Output);
        Assert.Equal(0, document.number);
        Assert.Equal("3703", document.head_note);
        Assert.Equal("3703", document.foot_note);
        Assert.Equal("3703", invoice.Number);
    }

    [Fact]
    public async Task Provider_AssignsConsecutive_AndKeepsPosNumberInNotes()
    {
        var sender = new Sender();
        var provider = Provider(
            new Preparation(Prepared(Invoice("3703"), Document("3703"))),
            new Consecutive(101),
            sender);

        var result = await provider.ProcessAsync([Invoice("3703")], Resolution, Company, default);

        Assert.True(result.Success);
        var sent = Assert.Single(sender.Sent);
        Assert.Equal(101, sent.number);
        Assert.Equal("3703", sent.head_note);
        Assert.Equal("3703", sent.foot_note);
    }

    [Fact]
    public async Task Provider_ContinuesAfterInvoiceFailure_OptionA()
    {
        var sender = new Sender(dto => dto.number == 102
            ? new BillingResultItem { InvoiceNumber = "102", Success = false, Message = "rechazada" }
            : new BillingResultItem { InvoiceNumber = dto.number.ToString(), Success = true, Message = "ok" });

        var provider = Provider(
            new Preparation(
                Prepared(Invoice("1"), Document("1")),
                Prepared(Invoice("2"), Document("2")),
                Prepared(Invoice("3"), Document("3"))),
            new Consecutive(101, 102, 103),
            sender);

        var result = await provider.ProcessAsync(
            [Invoice("1"), Invoice("2"), Invoice("3")], Resolution, Company, default);

        Assert.False(result.Success);
        Assert.Equal(3, result.Results.Count());
        Assert.Equal(new[] { 101, 102, 103 }, sender.Sent.Select(x => x.number));
        Assert.Equal(2, result.Results.Count(x => x.Success));
    }

    [Fact]
    public async Task Provider_DoesNotSend_WhenConsecutiveIsUnavailable()
    {
        var sender = new Sender();
        var provider = Provider(
            new Preparation(
                Prepared(Invoice("1"), Document("1")),
                Prepared(Invoice("2"), Document("2"))),
            new FailingConsecutive("resolution_expired", "La resolución está vencida."),
            sender);

        var result = await provider.ProcessAsync([Invoice("1"), Invoice("2")], Resolution, Company, default);

        Assert.Empty(sender.Sent);
        Assert.False(result.Success);
        Assert.Equal(2, result.Results.Count());
        Assert.All(result.Results, r => Assert.Equal("La resolución está vencida.", r.Message));
    }

    [Fact]
    public async Task Consecutive_CachesLastEmitted_AndAdvancesPerInvoice()
    {
        var last = new LastEmitted(100);
        var provider = new PlemsiConsecutiveProvider(last, NullLogger<PlemsiConsecutiveProvider>.Instance);

        var first = await provider.NextAsync(Resolution, Company, default);
        var second = await provider.NextAsync(Resolution, Company, default);

        Assert.True(first.Success);
        Assert.True(second.Success);
        Assert.Equal(101, first.Number);
        Assert.Equal(102, second.Number);
        Assert.Equal(1, last.Calls);
    }

    [Fact]
    public async Task Consecutive_StartsAtInitialRange_WhenThereAreNoDocuments()
    {
        var resolution = new DianResolutionEntity
        {
            ResolutionId = 1,
            InitialRange = 5000,
            FinalRange = 5100,
            ExpirationDate = DateTime.Now.AddYears(1),
            Prefix = "FE"
        };

        var provider = new PlemsiConsecutiveProvider(
            new LastEmitted(0), NullLogger<PlemsiConsecutiveProvider>.Instance);

        var result = await provider.NextAsync(resolution, Company, default);

        Assert.True(result.Success);
        Assert.Equal(5000, result.Number);
    }

    [Fact]
    public async Task Consecutive_Fails_WhenRangeIsExhausted()
    {
        var provider = new PlemsiConsecutiveProvider(
            new LastEmitted(1000), NullLogger<PlemsiConsecutiveProvider>.Instance);

        var result = await provider.NextAsync(Resolution, Company, default);

        Assert.False(result.Success);
        Assert.Equal("range_exhausted", result.ErrorCode);
    }

    [Fact]
    public async Task Consecutive_Fails_WhenResolutionIsExpired()
    {
        var resolution = new DianResolutionEntity
        {
            ResolutionId = 1,
            InitialRange = 1,
            FinalRange = 1000,
            ExpirationDate = DateTime.Now.AddDays(-1),
            Prefix = "FE"
        };

        var provider = new PlemsiConsecutiveProvider(
            new LastEmitted(10), NullLogger<PlemsiConsecutiveProvider>.Instance);

        var result = await provider.NextAsync(resolution, Company, default);

        Assert.False(result.Success);
        Assert.Equal("resolution_expired", result.ErrorCode);
    }

    private static PlemsiBillingFEProvider Provider(
        ICreateBilling preparation,
        IConsecutiveProvider consecutive,
        ISenderPlemsi sender) =>
        new(preparation, sender, consecutive, Mapper, NullLogger<PlemsiBillingFEProvider>.Instance);

    private static CreateBilling Invoice(string number) => new() { Number = number };

    private static BillingPreparationResult Prepared(CreateBilling billing, FERetailelectronicEntity document) =>
        new(billing, document, null);

    private static FERetailelectronicEntity Document(string posNumber) => new()
    {
        date = "2026-11-11",
        time = "10:54:00",
        prefix = "FE",
        orderReference = new OrderReferenceEntity { IdOrder = "COT" },
        send_email = true,
        customer = new CustomerEntity
        {
            IdentificationNumber = "222222222222",
            Name = "Consumidor Final",
            Email = "cliente@prueba.com"
        },
        payment = new PaymentEntity { PaymentFormId = 1 },
        items = [],
        resolution = "18760000001",
        head_note = posNumber,
        foot_note = posNumber,
        invoiceBaseTotal = 22000,
        invoiceTaxExclusiveTotal = 22000,
        invoiceTaxInclusiveTotal = 22000,
        totalToPay = 22000,
        finalTotalToPay = 22000,
        number = 0
    };

    private sealed class Preparation(params BillingPreparationResult[] results) : ICreateBilling
    {
        public Task<IEnumerable<BillingPreparationResult>> CreateInvoicesAsync(
            IEnumerable<CreateBilling> invoices,
            DianResolutionEntity dianResolutionEntity,
            CompanyProviderEntity companyProviderEntity,
            CancellationToken cancellationToken) =>
            Task.FromResult<IEnumerable<BillingPreparationResult>>(results);
    }

    private sealed class Consecutive(params int[] numbers) : IConsecutiveProvider
    {
        private int _index;

        public Task<ConsecutiveResult> NextAsync(
            DianResolutionEntity resolution,
            CompanyProviderEntity companyProvider,
            CancellationToken cancellationToken) =>
            Task.FromResult(ConsecutiveResult.Ok(numbers[_index++]));
    }

    private sealed class FailingConsecutive(string errorCode, string message) : IConsecutiveProvider
    {
        public Task<ConsecutiveResult> NextAsync(
            DianResolutionEntity resolution,
            CompanyProviderEntity companyProvider,
            CancellationToken cancellationToken) =>
            Task.FromResult(ConsecutiveResult.Fail(errorCode, message));
    }

    private sealed class Sender(Func<SenderRequestFEDTO, BillingResultItem>? behavior = null) : ISenderPlemsi
    {
        public List<SenderRequestFEDTO> Sent { get; } = [];

        public Task<BillingResultItem> SendAsync(
            object invoice,
            DianResolutionEntity resolution,
            CompanyProviderEntity company,
            CancellationToken cancellationToken)
        {
            var dto = (SenderRequestFEDTO)invoice;
            Sent.Add(dto);

            return Task.FromResult(behavior?.Invoke(dto) ?? new BillingResultItem
            {
                InvoiceNumber = dto.number.ToString(),
                Success = true,
                Message = "ok"
            });
        }
    }

    private sealed class LastEmitted(int last) : IGetLastInvoiceBilling
    {
        public int Calls { get; private set; }

        public Task<int> GetLastEmittedNumberAsync(
            DianResolutionEntity dianResolutionEntity,
            CompanyProviderEntity companyProviderEntity,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(last);
        }
    }

    private sealed class IdentityItems : IPrepareItemBilling
    {
        public Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items) =>
            Task.FromResult(items);
    }

    private sealed class NoTaxTotals : IGetAllTaxTotalsBilling
    {
        public Task<List<AllTaxTotalEntity>> IGetAllTaxTotalsBillingAsync(List<ItemElectronicEntity> allTaxTotal) =>
            Task.FromResult(new List<AllTaxTotalEntity>());
    }

    private sealed class AlwaysValidCheckDigit : ICalculateCheckDigits
    {
        public Task<string> CalculateCheckDigit(string identificationNumber, CancellationToken cancellationToken) =>
            Task.FromResult("7");
    }
}
