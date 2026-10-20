using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public record CreateBillingCommandResult(
    bool Success,
    int StatusCode,
    string? Message,
    IEnumerable<CreateBillingResultDTO>? Results)
{
    public static CreateBillingCommandResult Ok(IEnumerable<CreateBillingResultDTO> results)
        => new(true, 200, null, results);

    public static CreateBillingCommandResult BadRequest(string message)
        => new(false, 400, message, null);

    public static CreateBillingCommandResult NotFound(string message)
        => new(false, 404, message, null);
}