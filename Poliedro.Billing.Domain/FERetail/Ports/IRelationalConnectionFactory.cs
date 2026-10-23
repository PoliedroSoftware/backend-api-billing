using System.Data.Common;

namespace Poliedro.Billing.Domain.FERetail.Ports;

/// <summary>Creates provider-specific connections while keeping database access replaceable in tests.</summary>
public interface IRelationalConnectionFactory
{
    DbConnection Create(string connectionString);
}
