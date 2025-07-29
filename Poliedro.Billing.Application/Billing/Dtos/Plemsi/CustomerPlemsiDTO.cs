namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record CustomerPlemsiDTO
    (
    string identification_number,
    string? dv,
    string name,
    string? phone,
    string? address,
    string? email,
    string? merchant_registration,
    int? type_document_identification_id,
    int? type_organization_id,
    int? type_liability_id,
    int? municipality_id,
    string? municipality_code,
    int? type_regime_id
    );
