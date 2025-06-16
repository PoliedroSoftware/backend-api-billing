namespace Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
public interface IUpdateCurrentlyNumber
{
    Task UpdateCurrentlyNumberAsync(ParametersCurrentlyNumber Parameters, CancellationToken cancellationToken);
}