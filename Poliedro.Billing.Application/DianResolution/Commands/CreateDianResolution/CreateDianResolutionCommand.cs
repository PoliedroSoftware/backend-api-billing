using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;

public record CreateDianResolutionCommand(
    string ResolutionNumber,
    string Prefix,
    int InitialRange,
    int FinalRange,
    DateTime ResolutionDate,
    string Description,
    bool Active,
    DateTime CreationDate,
    int VigencyMonth,
    DateTime ExpirationDate,
    ResolutionType ResolutionType,
    int ClientBillingElectronicId
    ) : IRequest<Result<VoidResult, Error>>;

