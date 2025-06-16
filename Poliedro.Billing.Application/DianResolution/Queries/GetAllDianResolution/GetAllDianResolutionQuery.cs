using MediatR;

using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Application.DianResolution.Dtos;

namespace Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;

public record GetAllDianResolutionQuery : IRequest<Result<IEnumerable<DianResolutionDto>,Error>>;
