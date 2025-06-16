using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution
{
    public record UpdateDianResolutionCommand 
    (
     int Resolutionid, 
     string ResolutionNumber, 
     string Prefix,
     int InitialRange,
     int FinalRange,
     DateTime ResolutionDate = default,
     string Description = "",
     string? ResolutionFile = "",
     bool Active = false,
     DateTime CreationDate = default,
     int VigencyMonth = 0,
     DateTime ExpirationDate  = default,
     ResolutionType ResolutionType = default,
     int ClientBillingElectronicId = default
    ): IRequest<Result<VoidResult, Error>>;
    

}
