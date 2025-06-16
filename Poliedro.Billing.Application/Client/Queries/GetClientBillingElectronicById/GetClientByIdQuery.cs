using MediatR;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById
{
    public record GetClientByIdQuery : IRequest<Result<ClientDto, Error>>
    {
        public int Id { get; init; }
    }
}
