namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingPosService
{
    Task ProcessBillingAsync(CancellationToken cancellationToken);
}
