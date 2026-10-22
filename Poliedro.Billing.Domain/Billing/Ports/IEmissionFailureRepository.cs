namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IEmissionFailureRepository
{
    Task RecordAsync(EmissionFailure failure, CancellationToken cancellationToken);
}
