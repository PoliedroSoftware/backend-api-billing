using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Application.Billing.Services.Ports;
public interface IUdateItem
{
    Task<List<ItemElectronicEntity>> UpdateItemAsync(List<ItemElectronicEntity> items);
}
