using MediatR;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Server.Queries.GetServerById
{
    public record GetServerByIdQuery : IRequest<Result<ServerDto, Error>>
    {
        public int Id { get; init; }
    }
}