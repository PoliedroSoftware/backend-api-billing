using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.CreditNote.Ports;
using System.Net;
using System.Net.Http.Json;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;

public class CreditNoteDomainService(
    HttpClient httpClient, 
    IConfiguration configuration) : ICreditNoteDomainService
{
    private readonly string _plemsiEndpoint = configuration["ApiPlemsi:CreateCreditNote"] ??
        throw new InvalidOperationException("ApiPlemsi:CreateCreditNote is missing");

    public async Task<Result<ApiResponseCreditNote, Error>> Create(CreditNoteEntity creditNote, CancellationToken cancellationToken)
    {
        try
        {
        
            var response = await httpClient.PostAsJsonAsync(_plemsiEndpoint, creditNote, cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        
            if (!response.IsSuccessStatusCode)
            {
                return Result<ApiResponseCreditNote, Error>.Failure(
                    Error.CreateInstance(
                        "PLEMSI_ERROR",
                        $"Error al enviar la nota crédito: {response.StatusCode} - {responseContent}",
                        response.StatusCode
                    )
                );
            }

            var apiResponse = new ApiResponseCreditNote();
            return Result<ApiResponseCreditNote, Error>.Success(apiResponse);
        }
        catch (Exception ex)
        {
          
            return Result<ApiResponseCreditNote, Error>.Failure(
                Error.CreateInstance("PLEMSI_EXCEPTION", ex.Message, HttpStatusCode.InternalServerError)
            );
        }
    }
}


