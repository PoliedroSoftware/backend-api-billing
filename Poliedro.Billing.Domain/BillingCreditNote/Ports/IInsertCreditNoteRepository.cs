using Poliedro.Billing.Domain.BillingCreditNote.Entities;
namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public  interface IInsertCreditNoteRepository
{
    Task InsertCreditNoteRepositoryAsync(
        CreditNoteResultEntity creditNoteResultEntity);
}