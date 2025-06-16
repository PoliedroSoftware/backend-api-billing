using MediatR;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Queries.GetAllClient;

public record GetAllClientQuery : IRequest<Result<IEnumerable<ClientDto>, Error>>
{
}
