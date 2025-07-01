namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IBillingPosService
{
    Task ProcessBillingAsync(CancellationToken cancellationToken);
}
