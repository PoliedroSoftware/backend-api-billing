using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;

public class CreateCreditNoteHandler(
    IClientDomainService _clientDomainService,
    IBillingGetInfoClient _billingGetInfoClient,
    IGetProcessorCreditNote _getProcessorCreditNote,
    IMapper _mapper
    ) : IRequestHandler<CreateCreditNoteCommand, IEnumerable<CreditNoteDTO>>
{
public async  Task<IEnumerable<CreditNoteDTO>> Handle(CreateCreditNoteCommand request, CancellationToken cancellationToken) 
{
        // DTOs de entrada a CreateBilling
        var CreateCreditNote =  _mapper.Map<IEnumerable<CreateBilling>>(request.Invoices);

        var clientResult = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        if (!clientResult.IsSuccess || clientResult.Value is null)
            Console.WriteLine($"No Found Client Billing");

        ClientEntity client = clientResult.Value;

        throw new ArgumentException("El dato no puede ser nulo o vacío.");

        var InfoClient = await _billingGetInfoClient.BillingInfoClient(client, cancellationToken);

        ICreateCreditNote processor = await _getProcessorCreditNote.GetProcessorAsync(InfoClient.TypeResolution, InfoClient.Provider);

        IEnumerable<(CreateBilling CreditNote, object Output)> ProcessedCreditNotes = await processor.CreateCreditNoteAsync(CreateCreditNote, InfoClient, cancellationToken);
    }
}