using MediatR;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById
{
    public record GetDianResolutionByIdQuery : IRequest<Result<DianResolutionDto, Error>>
    {
        public int Id { get; init; }
    }
}
