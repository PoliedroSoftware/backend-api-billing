using System.Text.Json;
using Poliedro.Billing.Domain.Billing.Pos.Entity;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.InvoicePos.Entities;
using CreditNotePlemsiEntity = Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.CreditNote.CreditNotePlemsiEntity;

namespace Poliedro.Billing.Domain.Test;

public sealed class BillingProviderPayloadContractTests
{
    private static readonly JsonSerializerOptions WebJson = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Fe_retail_invoice_payload_round_trips_the_provider_contract_and_financial_totals()
    {
        const string payload = """
        {
          "date":"2026-10-22","time":"10:00:00","prefix":"FE","number":17,
          "softwareManufacturer":{"ownerName":"Poliedro","softwareName":"Billing","companyName":"Poliedro Software"},
          "orderReference":{"idOrder":"ORDER-17"},"send_email":true,
          "attachment1":{"filename":"TST17.pdf","b64data":"cGRm"},
          "customer":{"identification_number":"900336664","dv":"8","name":"Cliente QA","email":"billing@example.test","municipality_id":"1","type_document_identification_id":6},
          "payment":{"payment_form_id":1,"payment_method_id":10,"payment_due_date":"2026-10-22","duration_measure":"0"},
          "generalAllowances":[{"allowance_charge_reason":"Campaña","amount":5,"base_amount":100}],
          "items":[{"unit_measure_id":70,"line_extension_amount":100,"free_of_charge_indicator":false,"allowance_charges":[{"charge_indicator":false,"allowance_charge_reason":"Descuento","multiplier_factor_numeric":5,"amount":5,"base_amount":100}],"tax_totals":[{"tax_id":1,"percent":19,"tax_amount":19,"taxable_amount":100}],"description":"Producto","notes":"línea","code":170,"type_item_identification_id":4,"price_amount":50,"base_quantity":1,"invoiced_quantity":2}],
          "resolution":"res-1","resolutionText":"DIAN autorización","head_note":"Encabezado","foot_note":"Pie","notes":"Observación","allowanceTotal":5,
          "invoiceBaseTotal":100,"invoiceTaxExclusiveTotal":100,"invoiceTaxInclusiveTotal":119,"totalToPay":119,
          "allTaxTotals":[{"tax_id":1,"tax_amount":19,"percent":19,"taxable_amount":100}],
          "allHoldingsTaxTotals":[{"tax_id":5,"tax_amount":2,"percent":2,"taxable_amount":100}],
          "customSubtotals":[{"concept":"Retención","amount":2}],"finalTotalToPay":119
        }
        """;

        var invoice = JsonSerializer.Deserialize<FERetailelectronicEntity>(payload, WebJson)!;
        var encoded = JsonSerializer.Serialize(invoice, WebJson);
        using var json = JsonDocument.Parse(encoded);

        Assert.Equal("ORDER-17", invoice.orderReference.IdOrder);
        Assert.Equal("Cliente QA", invoice.customer.Name);
        Assert.Equal(19, invoice.items.Single().TaxTotals!.Single().TaxAmount);
        Assert.Equal(5, invoice.items.Single().AllowanceCharges!.Single().Amount);
        Assert.Equal(119, invoice.finalTotalToPay);
        Assert.Equal("FE", json.RootElement.GetProperty("prefix").GetString());
        Assert.Equal("TST17.pdf", json.RootElement.GetProperty("attachment1").GetProperty("filename").GetString());
        Assert.Equal(119, json.RootElement.GetProperty("totalToPay").GetDecimal());
    }

    [Fact]
    public void Pos_invoice_payload_preserves_point_of_sale_payment_and_item_amounts()
    {
        const string payload = """
        {
          "number":301,"date":"2026-10-22","time":"10:15:00",
          "softwareManufacturer":{"ownerName":"Poliedro","softwareName":"Billing POS","companyName":"Poliedro"},
          "sendToEmail":"customer@example.test","resolution":"POS-RES","prefix":"P1",
          "headNote":"POS","footNote":"Gracias","payPointInfo":{"code":"STORE-1","address":"Calle 1","cashierName":"Operador","payPointType":"Caja","saleCode":"S-1"},
          "payment":{"payment_form_id":1,"payment_method_id":10,"payment_due_date":"2026-10-22","duration_measure":"0"},
          "invoiceBaseTotal":"100","invoiceTaxExclusiveTotal":"100","invoiceTaxInclusiveTotal":"119","totalToPay":"119",
          "allTaxTotals":[{"tax_id":1,"tax_amount":19,"percent":19,"taxable_amount":100}],
          "items":[{"unit_measure_id":70,"invoiced_quantity":2,"line_extension_amount":100,"free_of_charge_indicator":false,"tax_totals":[{"tax_id":1,"tax_amount":19,"percent":19,"taxable_amount":100}],"description":"Producto POS","notes":"","code":"SKU-301","type_item_identification_id":4,"price_amount":"50","base_quantity":1}]
        }
        """;

        var invoice = JsonSerializer.Deserialize<InvoiceBillingPosEntity>(payload, WebJson)!;
        var encoded = JsonSerializer.Serialize(invoice, WebJson);
        using var json = JsonDocument.Parse(encoded);

        Assert.Equal("STORE-1", invoice.payPointInfo.code);
        Assert.Equal(10, invoice.payment.payment_method_id);
        Assert.Equal("119", invoice.totalToPay);
        Assert.Equal("SKU-301", invoice.items.Single().code);
        Assert.Equal(19, invoice.allTaxTotals.Single().tax_amount);
        Assert.Equal("Producto POS", json.RootElement.GetProperty("items")[0].GetProperty("description").GetString());
    }

    [Fact]
    public void Credit_note_and_pos_view_contracts_keep_reference_and_reconciliation_fields()
    {
        const string creditNoteJson = """
        {"Discrepancy":{"Code":2,"Description":"Corrección"},"Resolution":"NC-RES","Prefix":"NC","Number":9,
         "InvoiceReference":{"IssueDate":"2026-10-01","Uuid":"CUFE-1","Number":"FE-1"},"Items":[],"AllTaxTotals":[],
         "Seller":{"IdentificationNumber":"900336664","Dv":"8","Name":"Poliedro"},"HeadNote":"Nota crédito","FootNote":"Fin","TotalToPay":119}
        """;
        var creditNote = JsonSerializer.Deserialize<Poliedro.Billing.Domain.CreditNote.Entity.CreditNoteEntity>(creditNoteJson, WebJson)!;
        var posView = new InvoicePosViewEntity
        {
            Id = "invoice-301", Company = "company-1", State = "Emitted", GeneratedBy = "cashier",
            Resolution = "POS-RES", ResolutionNumber = "RES-1", Prefix = "P1", Number = "301",
            TypeDocumentId = 15, Date = "2026-10-22", Time = "10:15:00", Notes = "note",
            HeadNote = "head", FootNote = "foot", Consecutive = "P1301", QRStr = "qr", Cude = "cude",
            CreatedAt = DateTime.Parse("2026-10-22T10:15:00Z"), UpdatedAt = DateTime.Parse("2026-10-22T10:16:00Z"),
            LineExtensionAmount = 100, TaxExclusiveAmount = 100, TaxInclusiveAmount = 119, PayableAmount = 119,
            PaymentFormId = 1, PaymentMethodId = 10, PaymentDueDate = "2026-10-22"
        };
        var viewJson = JsonSerializer.Serialize(posView, WebJson);

        Assert.Equal("FE-1", creditNote.InvoiceReference.Number);
        Assert.Equal("CUFE-1", creditNote.InvoiceReference.Uuid);
        Assert.Equal(119, creditNote.TotalToPay);
        using var view = JsonDocument.Parse(viewJson);
        Assert.Equal("cude", view.RootElement.GetProperty("cude").GetString());
        Assert.Equal(119, view.RootElement.GetProperty("payableAmount").GetDecimal());
        Assert.Equal(10, view.RootElement.GetProperty("paymentMethodId").GetInt32());
    }

    [Fact]
    public void Fe_retail_persisted_invoice_round_trips_header_customer_items_and_totals()
    {
        var source = new Poliedro.Billing.Domain.FERetail.Entity.FERetailEntity
        {
            Id = 17, date = "2026-10-22", time = "10:30:00", prefix = "FE", number = 17,
            SoftwareManufacturer = new Poliedro.Billing.Domain.Billing.SoftwareManufacturerEntity
                { ownerName = "Poliedro", softwareName = "Billing", companyName = "Poliedro Software" },
            orderReference = new OrderReferenceEntity { IdOrder = "ORDER-17" }, send_email = true,
            attachment1 = new AttachmentEntity { FileName = "invoice.pdf", B64Data = "cGRm" },
            Attachment2 = new AttachmentEntity { FileName = "support.pdf", B64Data = "c3VwcG9ydA==" },
            customer = new CustomerEntity { IdentificationNumber = "900336664", Dv = "8", Name = "Cliente QA", Email = "billing@example.test" },
            payment = new PaymentEntity { PaymentFormId = 1, PaymentMethodId = 10, PaymentDueDate = "2026-10-22", DurationMeasure = "0" },
            generalAllowances = [new GeneralAllowanceEntity { AllowanceChargeReason = "Campaña", Amount = 5, BaseAmount = 100 }],
            items = [new Poliedro.Billing.Domain.FERetail.Entity.ItemEntity
            {
                unit_measure_id = 70, line_extension_amount = 100, free_of_charge_indicator = false,
                allowance_charges = [new AllowanceChargeEntity { ChargeIndicator = false, AllowanceChargeReason = "Descuento", Amount = 5, BaseAmount = 100 }],
                tax_totals = [new TaxTotalEntity { TaxId = 1, Percent = 19, TaxAmount = 19, TaxableAmount = 100 }],
                description = "Producto", notes = "línea", code = "SKU-17", type_item_identification_id = 4,
                price_amount = 50, base_quantity = 1, invoiced_quantity = 2
            }],
            resolution = "RES-1", resolutionText = "DIAN", head_note = "Encabezado", foot_note = "Pie", notes = "Observación",
            allowanceTotal = 5, invoiceBaseTotal = 100, invoiceTaxExclusiveTotal = 100,
            invoiceTaxInclusiveTotal = 119, totalToPay = 119,
            allTaxTotals = [new AllTaxTotalEntity { TaxId = 1, Percent = 19, TaxAmount = 19, TaxableAmount = 100 }],
            allHoldingsTaxTotals = [new AllHoldingsTaxTotalEntity { TaxId = 5, Percent = 2, TaxAmount = 2, TaxableAmount = 100 }],
            customSubtotals = [new CustomSubtotalEntity { Concept = "Retención", Amount = 2 }],
            finalTotalToPay = 119
        };

        var json = JsonSerializer.Serialize(source, WebJson);
        var restored = JsonSerializer.Deserialize<Poliedro.Billing.Domain.FERetail.Entity.FERetailEntity>(json, WebJson)!;

        Assert.Equal(17, restored.Id);
        Assert.Equal("Poliedro Software", restored.SoftwareManufacturer.companyName);
        Assert.Equal("ORDER-17", restored.orderReference.IdOrder);
        Assert.Equal("Cliente QA", restored.customer.Name);
        Assert.Equal("support.pdf", restored.Attachment2!.FileName);
        Assert.Equal("SKU-17", restored.items.Single().code);
        Assert.Equal(19, restored.items.Single().tax_totals!.Single().TaxAmount);
        Assert.Equal(119, restored.invoiceTaxInclusiveTotal);
        Assert.Equal(119, restored.finalTotalToPay);
    }

    [Fact]
    public void Invoice_data_read_model_round_trips_response_and_nested_provider_data()
    {
        var source = new Poliedro.Billing.Domain.GetInvoice.Entities.InvoiceData
        {
            OrderReference = new() { IdOrder = "ORDER-20" },
            LegalMonetaryTotals = new() { LineExtensionAmount = "100", TaxExclusiveAmount = "100", TaxInclusiveAmount = "119", PayableAmount = "119" },
            Id = "invoice-20", Company = "company-1", CreatedBy = "cashier", State = "Emitted", Preview = "html",
            ResolutionNumber = "RES-1", Prefix = "FE", Number = "20", Consecutive = "FE20", TypeDocumentId = 1,
            Date = "2026-10-22", Time = "10:30:00", EmailDeliveryStatus = "sent",
            Response = new() { Message = "accepted", Data = new() { IsValid = "true", StatusDescription = "Accepted", ErrorMessage = new() { String = [] }, IssueDate = "2026-10-22", IssueTime = "10:30:00", Filename = "FE20.xml" } },
            CreatedAt = "created", UpdatedAt = "updated",
            Customer = new() { _id = "customer-1", company = "company-1", invoice = "invoice-20", IdentificationNumber = "900336664", dv = "8", Name = "Cliente QA", phone = "3000000000", address = "Calle 1", email = "billing@example.test", merchant_registration = "0001", type_document_identification_id = 6, type_organization_id = 1, type_liability_id = 117, municipality_id = 1, type_regime_id = 2 },
            PaymentForm = new() { Id = "payment-1", Company = "company-1", Invoice = "invoice-20", PaymentFormId = 1, PaymentMethodId = 10, PaymentDueDate = "2026-10-22", PaidAmount = "119" },
            TaxTotals = [new() { Id = "tax-1", Company = "company-1", Invoice = "invoice-20", IsHolding = false, TaxId = 1, TaxAmount = "19", Percent = "19", TaxableAmount = "100" }],
            WithHoldingTaxTotal = [new() { _id = "withholding-1", company = "company-1", invoice = "invoice-20", is_holding = true, tax_id = 5, tax_amount = "2", percent = "2", taxable_amount = "100" }],
            HeadNote = "head", FootNote = "foot", Notes = "notes", AllowanceCharges = [new() { ChargeType = "discount", Amount = 5 }],
            Attachments = [new() { _id = "attachment-1", Company = "company-1", Invoice = "invoice-20", Filename = "FE20.pdf", B64data = "cGRm" }],
            TechProviderDefaultFootNote = "provider footer"
        };

        var json = JsonSerializer.Serialize(source, WebJson);
        var restored = JsonSerializer.Deserialize<Poliedro.Billing.Domain.GetInvoice.Entities.InvoiceData>(json, WebJson)!;

        Assert.Equal("invoice-20", restored.Id);
        Assert.Equal("ORDER-20", restored.OrderReference.IdOrder);
        Assert.Equal("FE20.xml", restored.Response.Data.Filename);
        Assert.Equal("Cliente QA", restored.Customer.Name);
        Assert.Equal("119", restored.PaymentForm.PaidAmount);
        Assert.Equal("19", restored.TaxTotals.Single().TaxAmount);
        Assert.Equal("2", restored.WithHoldingTaxTotal.Single().tax_amount);
        Assert.Equal("FE20.pdf", restored.Attachments.Single().Filename);
        Assert.Equal("provider footer", restored.TechProviderDefaultFootNote);
    }

    [Fact]
    public void Credit_note_provider_payload_round_trips_reference_discrepancy_customer_item_and_totals()
    {
        const string payload = """
        {"prefix":"NC","number":7,"send_email":true,
         "invoiceReference":{"number":"FE-17","uuId":"CUFE-17","issueDate":"2026-10-20"},
         "discrepancy":{"code":2,"description":"Corrección de factura"},
         "customer":{"identification_number":"900336664","dv":"8","name":"Cliente QA","email":"billing@example.test"},
         "payment":{"payment_form_id":1,"payment_method_id":10,"payment_due_date":"2026-10-22","duration_measure":"0"},
         "generalAllowances":[{"allowance_charge_reason":"Ajuste","amount":5,"base_amount":100}],
         "items":[{"unit_measure_id":70,"line_extension_amount":100,"free_of_charge_indicator":false,"description":"Producto devuelto","code":170,"type_item_identification_id":4,"price_amount":50,"base_quantity":1,"invoiced_quantity":2,"tax_totals":[{"tax_id":1,"percent":19,"tax_amount":19,"taxable_amount":100}]}],
         "resolution":"NC-RES","resolutionText":"DIAN","head_note":"Nota crédito","foot_note":"Pie","notes":"Devolución","allowanceTotal":5,
         "invoiceBaseTotal":100,"invoiceTaxExclusiveTotal":100,"invoiceTaxInclusiveTotal":119,"totalToPay":119,
         "allTaxTotals":[{"tax_id":1,"tax_amount":19,"percent":19,"taxable_amount":100}],
         "allHoldingsTaxTotals":[{"tax_id":5,"tax_amount":2,"percent":2,"taxable_amount":100}]}
        """;

        var creditNote = JsonSerializer.Deserialize<CreditNotePlemsiEntity>(payload, WebJson)!;
        var encoded = JsonSerializer.Serialize(creditNote, WebJson);
        using var result = JsonDocument.Parse(encoded);

        Assert.Equal("NC", creditNote.prefix);
        Assert.Equal(7, creditNote.number);
        Assert.True(creditNote.send_email);
        Assert.Equal("CUFE-17", creditNote.invoiceReference.UuId);
        Assert.Equal(2, creditNote.discrepancy.Code);
        Assert.Equal("Cliente QA", creditNote.customer.Name);
        Assert.Equal(10, creditNote.payment.PaymentMethodId);
        Assert.Equal(170, creditNote.items.Single().Code);
        Assert.Equal(19, creditNote.items.Single().TaxTotals!.Single().TaxAmount);
        Assert.Equal(119, creditNote.totalToPay);
        Assert.Equal("Corrección de factura", result.RootElement.GetProperty("discrepancy").GetProperty("description").GetString());
    }

    [Fact]
    public void Billing_client_context_and_invoice_ledger_model_keep_license_and_sequence_fields()
    {
        var client = new Poliedro.Billing.Domain.Billing.BillingInfoClient
        {
            ApiKey = "test-client-key", ServerRepository = new() { ServerId = 9, Ip = "127.0.0.1", DatabaseName = "billing_test", DbUsername = "reader", DbPassword = "test-only", Port = 3306 },
            TypeResolution = Poliedro.Billing.Domain.Resolution.Enums.ResolutionType.FE,
            Provider = "test-provider", Prefix = "TST", ExpirationDate = DateTime.Parse("2026-12-31"), FinalRange = 5000,
            ResolucionNumber = "RES-1", Descripcion = "Resolución de prueba", CurrentlyNumber = 17,
            MultipleResolution = 0, HeadNote = "head", FootNote = "foot", ResolutionId = 12
        };
        var invoice = new Poliedro.Billing.Domain.Billing.InvoiceEntity
        {
            Id = 17, Date = DateTime.Parse("2026-10-22"), Time = TimeSpan.FromHours(10), Resolution = "RES-1",
            Prefix = "TST", Number = "17", ResolutionType = "FE", Note = "invoice notes", AllowanceTotal = 5,
            InvoiceBaseTotal = 100, InvoiceTaxExclusiveTotal = 100, InvoiceTaxInclusiveTotal = 119,
            TotalToPay = 119, FinalTotalToPay = 119
        };

        var restoredClient = JsonSerializer.Deserialize<Poliedro.Billing.Domain.Billing.BillingInfoClient>(JsonSerializer.Serialize(client, WebJson), WebJson)!;
        var restoredInvoice = JsonSerializer.Deserialize<Poliedro.Billing.Domain.Billing.InvoiceEntity>(JsonSerializer.Serialize(invoice, WebJson), WebJson)!;

        Assert.Equal("test-client-key", restoredClient.ApiKey);
        Assert.Equal("test-provider", restoredClient.Provider);
        Assert.Equal(17, restoredClient.CurrentlyNumber);
        Assert.Equal(5000, restoredClient.FinalRange);
        Assert.Equal("billing_test", restoredClient.ServerRepository!.DatabaseName);
        Assert.Equal(3306u, restoredClient.ServerRepository.Port);
        Assert.Equal("17", restoredInvoice.Number);
        Assert.Equal(119, restoredInvoice.FinalTotalToPay);
    }

    [Fact]
    public void Persisted_invoice_line_contract_round_trips_tax_and_allowance_details()
    {
        var line = new Poliedro.Billing.Domain.InvoicePos.Entities.InvoiceLine
        {
            unit_measure_id = 70, invoiced_quantity = "2", line_extension_amount = "100", free_of_charge_indicator = false,
            tax_totals = [new() { tax_id = 1, tax_amount = "19", percent = 19, taxable_amount = "100" }],
            description = "Producto", notes = "line note", code = "SKU-17", type_item_identification_id = 4,
            price_amount = "50", base_quantity = "1", _id = "line-1", allowance_charges = [new { reason = "discount", amount = 5m }]
        };
        var encoded = JsonSerializer.Serialize(line, WebJson);
        var restored = JsonSerializer.Deserialize<Poliedro.Billing.Domain.InvoicePos.Entities.InvoiceLine>(encoded, WebJson)!;
        using var json = JsonDocument.Parse(encoded);

        Assert.Equal("SKU-17", restored.code);
        Assert.Equal("2", restored.invoiced_quantity);
        Assert.Equal("19", restored.tax_totals.Single().tax_amount);
        Assert.Equal(5m, json.RootElement.GetProperty("allowance_charges")[0].GetProperty("amount").GetDecimal());
    }
}
