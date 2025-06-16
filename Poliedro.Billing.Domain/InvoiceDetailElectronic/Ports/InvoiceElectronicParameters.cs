namespace Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

public record InvoiceElectronicParameters(
    string ApiKey
    , int PageNumber
    , int PageSize
    , string OrderBy
    );