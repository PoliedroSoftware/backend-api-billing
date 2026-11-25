using System.Text.Json;
using Poliedro.Billing.Api.Administration;

namespace Poliedro.Billing.Api.Tests;

public class ClientResolutionStatusTests
{
    [Fact]
    public void DeletedDocumentWithCudeStillConsumesItsConsecutive()
    {
        var row = new ResolutionStatus { Prefix = "FE", ResolutionNumber = "123" };
        using var json = JsonDocument.Parse("""
            {"data":{"docs":[
              {"state":"Deleted","cude":"already-assigned","prefix":"FE","resolution_number":"123","number":"31","date":"2026-10-22","time":"09:30:00"},
              {"state":"Emitted","cude":"older","prefix":"FE","resolution_number":"123","number":"30","date":"2026-10-20","time":"15:04:02"}
            ]}}
            """);

        ClientResolutionStatus.ApplyDocuments(row, json.RootElement);

        Assert.Equal(31, row.LastInvoiceNumber);
        Assert.Equal("2026-10-22 09:30:00", row.LastInvoiceDateTime);
        Assert.Equal("confirmed", row.LookupStatus);
    }

    [Fact]
    public void UsesOnlyEmittedDocumentsOfExactResolutionAndPreservesDocumentTime()
    {
        var row = new ResolutionStatus { Prefix = "FE", ResolutionNumber = "123" };
        using var json = JsonDocument.Parse("""
            {"data":{"docs":[
              {"state":"Emitted","prefix":"FE","resolution_number":"old","number":"999"},
              {"state":"Draft","prefix":"FE","resolution_number":"123","number":"1000"},
              {"state":"Emitted","prefix":"OTHER","resolution_number":"123","number":"888"},
              {"state":"Emitted","prefix":"FE","resolution_number":"123","number":"30","date":"2026-10-20","time":"15:04:02","updatedAt":"2026-10-21T01:00:00Z"},
              {"state":"Emitted","prefix":"FE","resolution_number":"123","number":29}
            ]}}
            """);
        ClientResolutionStatus.ApplyDocuments(row, json.RootElement);
        Assert.Equal(30, row.LastInvoiceNumber);
        Assert.Equal("2026-10-20 15:04:02", row.LastInvoiceDateTime);
        Assert.Equal("confirmed", row.LookupStatus);
    }

    [Theory]
    [InlineData("{\"data\":{\"docs\":[]}}", "not_found_in_latest_documents")]
    [InlineData("{\"data\":null}", "invalid_response")]
    public void MissingDataIsExplicitAndDoesNotInventZeroOrTime(string payload, string status)
    {
        var row = new ResolutionStatus { Prefix = "FE", ResolutionNumber = "123" };
        using var json = JsonDocument.Parse(payload);
        ClientResolutionStatus.ApplyDocuments(row, json.RootElement);
        Assert.Equal(status, row.LookupStatus);
        Assert.Null(row.LastInvoiceNumber);
        Assert.Null(row.LastInvoiceDateTime);
    }
}
