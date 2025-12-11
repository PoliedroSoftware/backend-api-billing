using Poliedro.Billing.Domain.Siigo.Entities;

namespace Poliedro.Billing.Domain.Siigo.Ports;

public interface IAuthSiigo
{
    Task<AuthSiigoBearerEntity> GetAuthSiigoAsync(string Email, string Password, CancellationToken cancellationToken);
}
