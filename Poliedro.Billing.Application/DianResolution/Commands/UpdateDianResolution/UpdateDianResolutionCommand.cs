using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution
{
    public record UpdateDianResolutionCommand 
    (
     int Resolutionid,
     int CompanyProviderId,
     ResolutionType ResolutionType = default,
     string ResolutionNumber = default, 
     string Prefix = default,
     int MultipleResolution = 0,
     int VigencyMonth = 0,
     int Automatic = 0,
     int InitialRange = 0,
     int FinalRange = 0,
     int CurrentRange = 0,
     DateTime ResolutionDate = default,
     DateTime ExpirationDate = default,
     int ExpirationDays = 0,
     int ExpirationNumber = 0,
     bool Active = false
    ): IRequest<Result<VoidResult, Error>>;
    

}
