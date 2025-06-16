using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Server.Commands.CreateServer;

public record CreateServerCommand : IRequest<Result<VoidResult, Error>>
{

    public string Ip { get; init; }
    public string DatabaseName { get; init; }
    public string DbUsername { get; init; }
    public string DbPassword { get; init; }

}
