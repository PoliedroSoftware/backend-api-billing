namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record CustomerRequestFEDTO
{
    public required string identification_number {  get; init; }
    public string? dv {  get; init; }
    public required string name { get; init; }
    public string? phone { get; init; }
    public string? address { get; init; }
    public string? email { get; init; }
    public string? merchant_registration { get; init; }
    public int? type_document_identification_id { get; init; }
    public int? type_organization_id { get; init; }
    public int? type_liability_id { get; init; }
    public int? municipality_id { get; init; }
    public string? municipality_code { get; init; }
    public int? type_regime_id { get; init; }
}
    
    

    
