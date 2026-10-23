using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Preparation;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Billing.Pos.Entity;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingPreparationServicesTests
{
    [Fact]
    public async Task Preparation_factory_selects_FE_and_POS_by_document_type_only()
    {
        var fe = new PrepareBillingFE(null!, null!, null!, null!, Configuration());
        var pos = new PrepareBillingPOS(null!);
        var services = new ServiceCollection();
        services.AddSingleton(fe);
        services.AddSingleton(pos);
        using var provider = services.BuildServiceProvider();
        var factory = new BillingPrepareFactory(provider);

        Assert.Same(fe, await factory.GetProcessorAsync(ResolutionType.FE));
        Assert.Same(pos, await factory.GetProcessorAsync(ResolutionType.POS));
        await Assert.ThrowsAsync<ArgumentException>(() => factory.GetProcessorAsync(ResolutionType.CreditNote));
    }

    [Fact]
    public async Task Electronic_preparation_returns_typed_internal_document_with_computed_totals()
    {
        var item = new ItemElectronicEntity
        {
            Code = 7, Description = "Item", InvoicedQuantity = 1, UnitPrice = 100,
            LineExtensionAmount = 100, TaxTotals = [new() { TaxId = 1, Percent = 19, TaxAmount = 19, TaxableAmount = 100 }]
        };
        var invoice = new CreateBilling
        {
            Number = "INV-7", Resolution = "INV-7", ItemElectronicEntity = [item],
            CustomerEntity = new CustomerEntity { TypeOfPerson = 2, IdentificationNumber = "901", Name = "Test customer" },
            PaymentEntity = new PaymentEntity { PaymentFormId = 1 }
        };
        invoice.DiscountAmountByInvoice = 10;
        var itemPreparation = new Mock<IPrepareItemBilling>();
        var taxTotals = new Mock<IGetAllTaxTotalsBilling>();
        var checkDigits = new Mock<ICalculateCheckDigits>();
        itemPreparation.Setup(x => x.PrepareItemBillingAsync(It.IsAny<List<ItemElectronicEntity>>())).ReturnsAsync([item]);
        taxTotals.Setup(x => x.IGetAllTaxTotalsBillingAsync(It.IsAny<List<ItemElectronicEntity>>())).ReturnsAsync([]);
        checkDigits.Setup(x => x.CalculateCheckDigit("901", It.IsAny<CancellationToken>())).ReturnsAsync("4");
        var service = new PrepareBillingFE(itemPreparation.Object, taxTotals.Object, checkDigits.Object, Mock.Of<IMapper>(), Configuration());
        var resolution = Resolution(20);

        var result = Assert.Single(await service.CreateInvoicesAsync([invoice], resolution, new CompanyProviderEntity(), 10, CancellationToken.None));

        Assert.Null(result.Error);
        Assert.Equal("10", invoice.Numeration);
        Assert.Equal("TST", invoice.Prefix);
        Assert.Equal("901", invoice.CustomerEntity!.IdentificationNumber);
        Assert.Equal(100, invoice.InvoiceBaseTotal);
        Assert.Equal(109, invoice.TotalToPay);
        var prepared = Assert.IsType<FERetailelectronicEntity>(result.Output!.ElectronicInvoice);
        Assert.Equal(10, prepared.number);
        Assert.Equal("R-20", prepared.resolution);
        Assert.Equal(109, prepared.totalToPay);
    }

    [Fact]
    public async Task Pos_preparation_returns_typed_document_and_rejects_expired_resolution()
    {
        var invoice = new CreateBilling
        {
            Number = "POS-1", Resolution = "POS-1", ItemElectronicEntity = [new ItemElectronicEntity
            {
                Code = 12, Description = "Item", InvoicedQuantity = 1, UnitPrice = 100,
                PriceAmount = 100, LineExtensionAmount = 100, Subtotal = 100
            }],
            CustomerEntity = new CustomerEntity { TypeOfPerson = 2, IdentificationNumber = "901", Name = "Test customer" },
            PaymentEntity = new PaymentEntity { PaymentFormId = 1 },
            TotalToPay = 100, InvoiceBaseTotal = 100, InvoiceTaxExclusiveTotal = 100, InvoiceTaxInclusiveTotal = 100
        };
        var service = new PrepareBillingPOS(Mock.Of<IMapper>());

        var result = Assert.Single(await service.CreateInvoicesAsync([invoice], Resolution(20), new CompanyProviderEntity(), 5, CancellationToken.None));

        Assert.Null(result.Error);
        Assert.Equal("5", invoice.Numeration);
        Assert.IsType<InvoicePosEntity>(result.Output!.PointOfSaleInvoice);
        Assert.Empty(await service.CreateInvoicesAsync([invoice], Resolution(5), new CompanyProviderEntity(), 5, CancellationToken.None));
    }

    private static DianResolutionEntity Resolution(int finalRange) => new()
    {
        ResolutionId = 20, ResolutionType = ResolutionType.FE, Prefix = "TST", ResolutionNumber = "R-20",
        Description = "Test resolution", FinalRange = finalRange, ExpirationDate = DateTime.Today.AddMonths(6)
    };

    private static IConfiguration Configuration() => new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["CosumerFinal:identification"] = "222222222222", ["CosumerFinal:dv"] = "7"
    }).Build();
}
