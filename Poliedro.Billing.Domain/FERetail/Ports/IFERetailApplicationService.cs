namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IFERetailApplicationService
{
    Task ProcessFERetailAsync(CancellationToken cancellationToken);
}
