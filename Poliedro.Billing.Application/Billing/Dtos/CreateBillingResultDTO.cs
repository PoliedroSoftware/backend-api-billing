namespace Poliedro.Billing.Application.Billing.Dtos;

public record CreateBillingResultDTO
{
        public bool Status { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
}
