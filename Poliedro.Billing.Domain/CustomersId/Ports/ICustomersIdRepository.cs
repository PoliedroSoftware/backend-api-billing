using Poliedro.Billing.Domain.CustomersId.Entities;

namespace Poliedro.Billing.Domain.CustomersId.Ports;
public interface ICustomersIdRepository
{
    Task<Customers> GetByIdAsync(string id, string token);
}