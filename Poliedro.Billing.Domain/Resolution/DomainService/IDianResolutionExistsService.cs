namespace Poliedro.Billing.Domain.Resolution.DomainService;

public interface IDianResolutionExistsService
{
    Task<bool> EntityExists(int resolutionId, CancellationToken cancellationToken);
}
