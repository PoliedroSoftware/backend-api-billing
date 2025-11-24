using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Entities;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolutionCreditNote.DomineService.Impl;

public class ResponsesPlemsiCreditNoteRepository(
    IClientDomainService _clientDomainService,
    IInsertCreditNoteRepository _insertCreditNoteRepository,
    IUpdateCurrentlyNumberCreditNote _updateCurrentlyNumberCreditNote,
    IDatabaseUtils _databaseUtils
    ) : IResponsesPlemsiCreditNoteRepository
{
    public async Task IResponsesPlemsiCreditNoteRepositoryAsync(
        List<ApiResponseFERetailPos> response,
        IEnumerable<CreateBilling> processedInvoices,
        CancellationToken cancellationToken)
    {
        var paired = response.Zip(processedInvoices, (resp, invoice) => new { resp, invoice });

        foreach (var pair in paired)
        {
            var customerInfo = await _clientDomainService.GetByIdAsync(pair.invoice.CustomerEntity.ApiKey, cancellationToken);

            if (customerInfo == null)
            {
                Console.WriteLine($"Error Insert Invoice Success", customerInfo);
                continue;
            }

            var connectionString = _databaseUtils.GetConnectionString(customerInfo.Value.Server);

            int NumberInvoice = int.Parse(pair.invoice.Numeration);
            string CurrentlyDate = DateTime.Now.ToString();
            string LastedInvoiced = pair.invoice.Number;

            var CreditNoteResult = new CreditNoteResultEntity
            {
                ConsecutiveNumber = NumberInvoice,
                Cude = pair.resp.Data.Cude,
                QRCode = pair.resp.Data.QRCode,
                ConnectionString = connectionString,
                ProviderType = customerInfo.Value.ProviderId!,
                ClientBillingElectronicId = customerInfo.Value.ClientBillingElectronicId,
                NumberInvoice = pair.invoice.Number
            };


            await _insertCreditNoteRepository.InsertCreditNoteRepositoryAsync(CreditNoteResult);

            await _updateCurrentlyNumberCreditNote.UpdateCurrentlyNumberCreditNoteAsync(
                 new ParametersCurrentlyNumber(
                 NumberInvoice, 
                 CurrentlyDate,
                 customerInfo.Value.ResolutionId,
                 LastedInvoiced
                 ),
                 cancellationToken);
        }
    }
}