using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;

namespace Poliedro.Billing.Application.Billing.Services.Ports;

public interface IUdateItem
{
    Task<List<ItemElectronicEntity>> UpdateItemAsync(List<ItemElectronicEntity> items);
}
