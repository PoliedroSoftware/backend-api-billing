using System;
using System.Collections.Generic;
using System.Text;
using Poliedro.Billing.Domain.Location.Entities;

namespace Poliedro.Billing.Domain.Location.Ports;

public interface IMunicipalityService
{
    Task<IEnumerable<MunicipalityEntity>> GetAllAsync(string apiKey, CancellationToken cancellationToken);
}
