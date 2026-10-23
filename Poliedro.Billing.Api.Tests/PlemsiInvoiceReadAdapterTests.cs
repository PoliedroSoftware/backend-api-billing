using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.GetInvoice;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SuccessInvoice;
using System.Net;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiInvoiceReadAdapterTests
{
    [Theory]
    [InlineData("token", "Bearer token")]
    [InlineData("Bearer token", "Bearer token")]
    [InlineData("bearer token", "Bearer token")]
    public async Task Get_invoice_sends_normalized_bearer_token_and_deserializes_data(string token, string expectedHeader)
    {
        var handler = new CaptureHandler((request, _) =>
        {
            Assert.Equal("https://provider.invalid/invoices/CUFE-1", request.RequestUri!.ToString());
            Assert.Equal(expectedHeader, request.Headers.Authorization!.ToString());
            Assert.Contains(request.Headers.Accept, header => header.MediaType == "application/json");
            return Task.FromResult(Json("""{"success":true,"data":{"_id":"invoice-10","order_reference":{"id_order":"ORDER-10"},"legal_monetary_totals":{"line_extension_amount":"100","tax_exclusive_amount":"100","tax_inclusive_amount":"119","payable_amount":"119"},"company":"company-1","createdBy":"cashier","state":"Emitted","preview":"html","resolution_number":"RES-1","prefix":"TST","number":"FE-10","type_document_id":1,"date":"2026-10-22","time":"10:00:00","emailDeliveryStatus":"sent","sendEmailNotification":true,"consecutive":"TST10","createdAt":"created","updatedAt":"updated","response":{"message":"accepted","data":{"IsValid":"true","StatusDescription":"Accepted","ErrorMessage":{"String":[]},"IssueDate":"2026-10-22","IssueTime":"10:00:00","Filename":"TST10.xml"}},"customer":{"_id":"customer-1","company":"company-1","invoice":"invoice-10","identification_number":"900336664","dv":"8","name":"Cliente de prueba","phone":"3000000000","address":"Calle 1","email":"billing@example.test","merchant_registration":"0001","type_document_identification_id":6,"type_organization_id":1,"type_liability_id":117,"municipality_id":1,"type_regime_id":2},"payment_form":[{"_id":"payment-1","company":"company-1","invoice":"invoice-10","payment_form_id":1,"payment_method_id":10,"payment_due_date":"2026-10-22","paid_amount":"119"}],"tax_totals":[{"_id":"tax-1","company":"company-1","invoice":"invoice-10","is_holding":false,"tax_id":1,"tax_amount":"19","percent":"19","taxable_amount":"100"}],"with_holding_tax_total":[{"_id":"withholding-1","company":"company-1","invoice":"invoice-10","is_holding":true,"tax_id":5,"tax_amount":"2.5","percent":"2.5","taxable_amount":"100"}],"head_note":"head","foot_note":"foot","notes":"notes","resolution":"resolution text","QRStr":"qr-value","allowance_charges":[{"charge_type":"discount","amount":5}],"invoice_lines":[{"_id":"line-1","company":"company-1","invoice":"invoice-10","unit_measure_id":70,"invoiced_quantity":"2","line_extension_amount":"100","free_of_charge_indicator":false,"tax_totals":[],"description":"Product","notes":"line note","code":"SKU-1","type_item_identification_id":4,"price_amount":"50","base_quantity":"1","with_holding_tax_total":[]}],"attachments":[{"_id":"attachment-1","company":"company-1","invoice":"invoice-10","filename":"TST10.pdf","b64data":"cGRm"}],"techProviderDefaultFootNote":"provider footer"}}"""));
        });
        using var client = new HttpClient(handler);
        var configuration = Config(("ApiPlemsi:GetInvoice", "https://provider.invalid/invoices/"));

        var invoice = await new GetInvoiceDomainGetInvoice(configuration, client)
            .GetByIdAsync(" CUFE-1 ", token, CancellationToken.None);

        Assert.NotNull(invoice);
        Assert.Equal("FE-10", invoice.Number);
        Assert.Equal("TST", invoice.Prefix);
        Assert.Equal("ORDER-10", invoice.OrderReference.IdOrder);
        Assert.Equal("Cliente de prueba", invoice.Customer.Name);
        Assert.Equal("119", invoice.PaymentForm.Single().PaidAmount);
        Assert.Equal("19", invoice.TaxTotals.Single().TaxAmount);
        Assert.Equal("TST10.xml", invoice.Response.Data.Filename);
        Assert.Equal("SKU-1", invoice.InvoiceLines.Single().Code);
        Assert.Equal("cGRm", invoice.Attachments.Single().B64data);
        Assert.Equal(1, handler.Requests);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, "{}")]
    [InlineData(HttpStatusCode.OK, "not-json")]
    [InlineData(HttpStatusCode.OK, "null")]
    public async Task Get_invoice_returns_null_for_provider_failure_or_unusable_body(HttpStatusCode status, string body)
    {
        using var client = new HttpClient(new CaptureHandler((_, _) => Task.FromResult(Json(body, status))));
        var invoice = await new GetInvoiceDomainGetInvoice(
            Config(("ApiPlemsi:GetInvoice", "https://provider.invalid/invoices/")), client)
            .GetByIdAsync("CUFE-1", "test", CancellationToken.None);
        Assert.Null(invoice);
    }

    [Fact]
    public async Task Success_invoice_query_uses_parameters_authorization_and_maps_docs()
    {
        var handler = new CaptureHandler((request, _) =>
        {
            Assert.Equal("https://provider.invalid/success?page=2&perPage=25&state=Emitted&order=desc", request.RequestUri!.ToString());
            Assert.Equal("Bearer client-token", request.Headers.Authorization!.ToString());
            Assert.Contains(request.Headers.Accept, header => header.MediaType == "application/json");
            return Task.FromResult(Json("""{"success":true,"data":{"docs":[{"_id":"doc-1","company":"1","createdBy":"user","state":"Emitted","resolution_number":"R1","prefix":"TST","number":"10","date":"2026-10-22","time":"10:00:00","customer":{"Identification_Number":"9001","Name":"Cliente"},"legal_monetary_totals":{"Line_Extension_Amount":"100","Tax_Exclusive_Amount":"100","Tax_Inclusive_Amount":"119","Payable_Amount":"119"}}]}}"""));
        });
        using var client = new HttpClient(handler);
        var repository = new SuccessInvoiceRepository(Config(("ApiPlemsi:FEUrl", "https://provider.invalid/success")), client);

        var invoices = (await repository.GetAllInvoice(new SuccessInvoiceQueryParameters("client-token", "desc", 2, 25))).ToList();

        var invoice = Assert.Single(invoices);
        Assert.Equal("doc-1", invoice._Id);
        Assert.Equal("10", invoice.Number);
        Assert.Equal(1, handler.Requests);
    }

    [Theory]
    [InlineData(HttpStatusCode.ServiceUnavailable, "{}")]
    [InlineData(HttpStatusCode.OK, "{\"success\":true,\"data\":{\"docs\":null}}")]
    public async Task Success_invoice_returns_empty_for_failed_or_empty_provider_response(HttpStatusCode status, string body)
    {
        using var client = new HttpClient(new CaptureHandler((_, _) => Task.FromResult(Json(body, status))));
        var repository = new SuccessInvoiceRepository(Config(("ApiPlemsi:FEUrl", "https://provider.invalid/success")), client);

        var invoices = await repository.GetAllInvoice(new SuccessInvoiceQueryParameters("token", "asc", 1, 10));

        Assert.Empty(invoices);
    }

    [Fact]
    public async Task Success_invoice_repository_does_not_hide_malformed_provider_json()
    {
        using var client = new HttpClient(new CaptureHandler((_, _) => Task.FromResult(Json("not-json"))));
        var repository = new SuccessInvoiceRepository(Config(("ApiPlemsi:FEUrl", "https://provider.invalid/success")), client);

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() =>
            repository.GetAllInvoice(new SuccessInvoiceQueryParameters("token", "asc", 1, 10)));
    }

    private static IConfiguration Config(params (string Key, string Value)[] pairs)
        => new ConfigurationBuilder().AddInMemoryCollection(pairs.ToDictionary(pair => pair.Key, pair => (string?)pair.Value)).Build();

    private static HttpResponseMessage Json(string body, HttpStatusCode status = HttpStatusCode.OK)
        => new(status) { Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json") };

    private sealed class CaptureHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> response) : HttpMessageHandler
    {
        public int Requests { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            return response(request, cancellationToken);
        }
    }
}
