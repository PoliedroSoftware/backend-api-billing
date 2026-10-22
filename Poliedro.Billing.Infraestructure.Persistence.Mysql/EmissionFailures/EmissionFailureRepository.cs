using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EmissionFailures;

public sealed class EmissionFailureRepository(DataBaseContext db) : IEmissionFailureRepository
{
    public async Task RecordAsync(EmissionFailure failure, CancellationToken cancellationToken)
    {
        db.EmissionFailures.Add(failure);
        await db.SaveChangesAsync(cancellationToken);
    }
}
