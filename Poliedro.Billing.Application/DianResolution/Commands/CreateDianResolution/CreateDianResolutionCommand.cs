using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;

public record CreateDianResolutionCommand(
    int ResolutionId,
    int CompanyProviderId,
    ResolutionType ResolutionType,
    string ResolutionNumber,
    string Prefix,
    int MultipleResolution,
    int VigencyMonth,
    int Automatic,
    int InitialRange,
    int FinalRange,
    int CurrentRange,
    DateTime ResolutionDate,
    DateTime ExpirationDate,
    int ExpirationDays,
    int ExpirationNumber,
    bool Active
    ) : IRequest<Result<VoidResult, Error>>;

