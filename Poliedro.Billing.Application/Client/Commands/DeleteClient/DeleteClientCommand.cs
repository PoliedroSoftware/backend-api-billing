using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Commands.DeleteClient;

public record DeleteClientCommand : IRequest<Result<VoidResult, Error>>
{
    public int Id { get; init; }
}
