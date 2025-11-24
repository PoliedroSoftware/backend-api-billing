using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.BillingCreditNote.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;

public class CreateCreditNoteHandler(
    IClientDomainService _clientDomainService,
    IBillingGetInfgoClientCreditNote _billingGetInfoClient,
    IGetProcessorCreditNote _getProcessorCreditNote,
    IBillingCreditNoteSenderFactory _billingCreditNoteSenderFactory,
    IResponsesPlemsiCreditNoteRepository _responsesPlemsiCreditNoteRepository,
    IMapper _mapper
    ) : IRequestHandler<CreateCreditNoteCommand, IEnumerable<CreateBillingResultDTO>>
{
public async  Task<IEnumerable<CreateBillingResultDTO>> Handle(CreateCreditNoteCommand request, CancellationToken cancellationToken) 
{
        // DTOs de entrada a CreateBilling
        var CreateCreditNote =  _mapper.Map<IEnumerable<CreateBilling>>(request.Invoices);

        var clientResult = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        if (!clientResult.IsSuccess || clientResult.Value is null)
            Console.WriteLine($"No Found Client Billing");

        ClientEntity client = clientResult.Value;

        var InfoClient = await _billingGetInfoClient.BillingInfoClientCreditNote(client, cancellationToken);

        ICreateCreditNote processor = await _getProcessorCreditNote.GetProcessorAsync(InfoClient.TypeResolution, InfoClient.Provider);

        IEnumerable<(CreateBilling CreditNote, object Output)> ProcessedCreditNotes = await processor.CreateCreditNoteAsync(CreateCreditNote, InfoClient, cancellationToken);

        IEnumerable<CreateBilling> billingEntitiesProcessed = ProcessedCreditNotes.Select(p => p.CreditNote);
        IEnumerable<object> outputEntitiesProcessed = ProcessedCreditNotes.Select(p => p.Output);

        IBillingCreditNoteSender sender = _billingCreditNoteSenderFactory.ResolveCreditNote(InfoClient.Provider, InfoClient.TypeResolution);

        var billingResults = new List<CreateBillingResultDTO>();

        foreach(var processed in ProcessedCreditNotes)
        {
            var invoiceRequest = new PlemsiInvoiceRequest
            {
                ApiKey = request.ApiKey,
                Invoices = new List<object> { processed.Output }
            };

            var responses = await sender.SendCreditNoteAsync(invoiceRequest, InfoClient, cancellationToken);
            var result = responses.FirstOrDefault();

            if(result is not null)
            {
                if (result.Success)
                {
                   await _responsesPlemsiCreditNoteRepository.IResponsesPlemsiCreditNoteRepositoryAsync(
                       new List<ApiResponseFERetailPos> { result },
                       new List<CreateBilling> { processed.CreditNote },
                       cancellationToken
                       );
                }
                else
                {
                    Console.WriteLine($"Factura fallida: {result.Info}");
                }

                billingResults.Add(new CreateBillingResultDTO
                {
                    Status = result.Success,
                    Message = result.Success ? "Factura procesada exitosamente" : result.Info,
                    Data = null
                });
            }

        }
        return billingResults;

    }
}