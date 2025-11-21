namespace Poliedro.Billing.Domain.Server.DomainService;

public interface IServerExistsService
{
    Task<bool> EntityExists(int id);
}
