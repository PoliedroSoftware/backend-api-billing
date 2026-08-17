namespace Poliedro.Billing.Application.Billing.Dtos;

public record CreateBillingRequestDTO(IEnumerable<CreateBillingInputDTO> Data);