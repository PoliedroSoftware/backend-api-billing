using System.Text.Json;
using AutoMapper;
using Poliedro.Billing.Application.GetInvoice.AutoMappers;
using Poliedro.Billing.Application.GetInvoice.Dtos;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Api.Tests;

public class InvoiceRecoveryTests
{
    [Theory]
    [InlineData("\"25.00\"")]
    [InlineData("25.00")]
    public void DetailPreservesLinesIdentityAndDocumentAllowance(string amount)
    {
        // Synthetic document reproduces Plemsi's string amounts and decimal percentage.
        var json = """
            {"code":200,"success":true,"data":{
              "prefix":"QA","number":"7","cude":"synthetic-cufe",
              "legal_monetary_totals":{"line_extension_amount":"1000.00",
                "allowance_total_amount":"25.00","payable_amount":"975.00"},
              "allowance_charges":[{"charge_indicator":false,"discount_id":1,
                "allowance_charge_reason":"RETENCIONES","multiplier_factor_numeric":2.5,
                "base_amount":"1000.00","amount":AMOUNT}],
              "invoice_lines":[{"description":"Synthetic item","invoiced_quantity":"2",
                "price_amount":"500.00","line_extension_amount":"1000.00"}],
              "attachments":[],"payment_form":[{"payment_form_id":1}]
            }}
            """.Replace("AMOUNT", amount);
        var document = JsonSerializer.Deserialize<GetInvoiceApiResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!.Data;
        var config = new MapperConfiguration(options => options.AddProfile<GetInvoiceMapper>());
        config.AssertConfigurationIsValid();
        var dto = config.CreateMapper().Map<GetInvoiceDto>(document);
        Assert.Equal("synthetic-cufe", dto.Cude);
        Assert.Equal("Synthetic item", Assert.Single(dto.InvoiceLine).Description);
        Assert.NotNull(dto.Attachment);
        Assert.Equal("25.00", dto.LegalMonetaryTotals.AllowanceTotalAmount);
        var allowance = Assert.Single(dto.AllowanceCharges);
        Assert.False(allowance.ChargeIndicator);
        Assert.Equal(2.5m, allowance.Percent);
        Assert.Equal(1000m, allowance.BaseAmount);
        Assert.Equal(25m, allowance.Amount);
        Assert.Equal("RETENCIONES", allowance.Reason);
    }
}
