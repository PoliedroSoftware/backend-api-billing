using Poliedro.Billing.Domain.Siigo.Entities;
using Poliedro.Billing.Domain.Siigo.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Siigo;

public class AuthSiigo : IAuthSiigo
{
    public Task<AuthSiigoBearerEntity> GetAuthSiigoAsync(string Email, string Password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
