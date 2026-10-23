using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingPayloadMappingTests
{
    [Fact]
    public void Electronic_invoice_dto_maps_to_domain_and_back_with_nested_financial_data_intact()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();
        var timestamp = new DateTime(2026, 10, 22, 10, 30, 0, DateTimeKind.Utc);
        var dto = new CreateBillingDTO(
            Date: timestamp,
            Time: timestamp,
            SendToEmail: "customer@example.test",
            SoftwareManufacturer: new SoftwareManufacturerDTO("Poliedro", "Billing", "Poliedro Software"),
            PayPointInfo: new PayPointInfoDTO("STORE-1", "Calle 1", "Operador", "Caja", "SALE-1"),
            Number: "FE-17",
            Prefix: "TST",
            TransactionDate: timestamp,
            OrderReference: new OrderReferenceDTO("ORDER-17"),
            SendEmail: true,
            Attachment1: new AttachmentDTO("invoice.pdf", "cGRm"),
            Attachment2: new AttachmentDTO("support.pdf", "c3VwcG9ydA=="),
            CustomerEntity: new CustomerBillingDTO("900336664", 1, "0", "test-api-key", "8", 0, "Cliente QA", "3000000000", "Calle 1", "billing@example.test", "0001", "Bogotá", "Cundinamarca", "CO", 6, 1, 117, 1, "11001", 2),
            PaymentEntity: new PaymentDTO(1, 10, "2026-10-22", "0"),
            GeneralAllowanceEntity: [new GeneralAllowanceDTO("Campaña", 5, 5, 100)],
            ItemElectronicEntity: [new ItemElectronicDTO(70, 100, 5, "percentage", 50, 0, false,
                [new AllowanceChargeDTO(false, "Descuento", 5, 5, 100)],
                [new TaxTotalDTO(1, 19, 19, 100)],
                [new WIthHoldingTaxTotalDTO(5, 2, 2, 100)],
                "Producto", "línea", "17", 4, 50, 1, 2, 19, 19, 50, 100)],
            Resolution: "RES-1",
            ResolutionText: "Autorización DIAN",
            HeadNote: "Encabezado",
            FootNote: "Pie",
            Notes: "Observación",
            TotalBeforeTax: 100,
            DiscountAmountByInvoice: 5,
            DiscountType: "percentage",
            AllowanceTotal: 5,
            InvoiceBaseTotal: 100,
            InvoiceTaxExclusiveTotal: 100,
            InvoiceTaxInclusiveTotal: 119,
            TotalToPay: 119,
            AllTaxTotalEntity: [new AllTaxTotalDTO(1, 19, 19, 100)],
            AllHoldingsTaxTotalEntity: [new AllHoldingsTaxTotalDTO(5, 2, 2, 100)],
            FinalTotalToPay: 119);

        var domain = mapper.Map<CreateBilling>(dto);
        var roundTrip = mapper.Map<CreateBillingDTO>(domain);

        Assert.Equal("FE-17", domain.Number);
        Assert.Equal("ORDER-17", domain.OrderReference!.IdOrder);
        Assert.Equal("900336664", domain.CustomerEntity!.IdentificationNumber);
        var mappedItem = Assert.Single(roundTrip.ItemElectronicEntity!);
        Assert.Equal("17", mappedItem.Code);
        Assert.Equal(19, Assert.Single(mappedItem.TaxTotals!).TaxAmount);
        Assert.Equal(5, Assert.Single(mappedItem.AllowanceCharges!).Amount);
        Assert.Equal("invoice.pdf", roundTrip.Attachment1!.FileName);
        Assert.Equal(119, roundTrip.FinalTotalToPay);
        Assert.Null(domain.Numeration); // Sequence assignment remains owned by the resolution service.
    }
}
