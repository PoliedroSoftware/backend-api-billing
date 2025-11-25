namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientExistsService
{
    Task<bool> EntityExists(int id, CancellationToken cancellationToken);
}
