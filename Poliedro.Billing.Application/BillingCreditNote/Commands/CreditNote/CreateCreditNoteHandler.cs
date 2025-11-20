using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
using Poliedro.Billing.Application.BillingCreditNote.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;

public class CreateCreditNoteHandler(
    IClientDomainService _clientDomainService,
    IBillingGetInfgoClientCreditNote _billingGetInfoClient,
    IGetProcessorCreditNote _getProcessorCreditNote,
    IBillingCreditNoteSenderFactory _billingCreditNoteSenderFactory,
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

        var InfoClient = await _billingGetInfoClient.BillingInfoClientCreditNote(client, cancellationToken);

        ICreateCreditNote processor = await _getProcessorCreditNote.GetProcessorAsync(InfoClient.TypeResolution, InfoClient.Provider);

        IEnumerable<(CreateBilling CreditNote, object Output)> ProcessedCreditNotes = await processor.CreateCreditNoteAsync(CreateCreditNote, InfoClient, cancellationToken);

        IEnumerable<CreateBilling> billingEntitiesProcessed = ProcessedCreditNotes.Select(p => p.CreditNote);
        IEnumerable<object> outputEntitiesProcessed = ProcessedCreditNotes.Select(p => p.Output);

        IBillingCreditNoteSender sender = _billingCreditNoteSenderFactory.ResolveCreditNote(InfoClient.Provider, InfoClient.TypeResolution);

        var billingResults = new List<CreateBillingResultDTO>();




        throw new ArgumentException("El dato no puede ser nulo o vacío.");
    }
}