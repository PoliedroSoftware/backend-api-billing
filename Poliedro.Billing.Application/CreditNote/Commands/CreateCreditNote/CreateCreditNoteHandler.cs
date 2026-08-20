using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.CreditNote.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;


namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;


public class CreateCreditNoteHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    ICreditNoteDomainService _creditNoteDomainService,
    IMapper mapper
) : IRequestHandler<CreateCreditNoteCommand, IEnumerable<CreateCreditNoteResultDto>>
{
    public async Task<IEnumerable<CreateCreditNoteResultDto>> Handle(
        CreateCreditNoteCommand request,
        CancellationToken cancellationToken)
    {
        var resolution = await _dianResolutionGetByIdService
            .GetByIdAsync(request.Id, cancellationToken);


        var companyProvider = await _companyProviderGetByIdService
            .GetCompanyProviderByIdAsync(resolution.Value.CompanyProviderId, cancellationToken);


        var results = new List<CreateCreditNoteResultDto>();


        foreach (var creditNoteInput in request.CreditNotes)
        {
            try
            {
                var creditNoteEntity = mapper.Map<CreditNoteEntity>(creditNoteInput);


                creditNoteEntity.Resolution = resolution.Value.ResolutionNumber;
                creditNoteEntity.Prefix = resolution.Value.Prefix;
                creditNoteEntity.Customer.ApiKey = companyProvider.ApiKey;


                var result = await _creditNoteDomainService
                    .Create(creditNoteEntity, cancellationToken);


                if (result.IsSuccess)
                {
                    results.Add(new CreateCreditNoteResultDto
                    {
                        Status = true,
                        Message = "Nota crédito procesada exitosamente",
                        Data = result.Value
                    });
                }
                else
                {
                    results.Add(new CreateCreditNoteResultDto
                    {
                        Status = false,
                        Message = result.Error!.Description,
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Excepción procesando nota crédito: {ex.Message}");
                results.Add(new CreateCreditNoteResultDto
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        return results;
    }
}
