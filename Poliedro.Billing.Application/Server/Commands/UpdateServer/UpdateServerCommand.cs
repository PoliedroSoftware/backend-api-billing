using MediatR;
using Poliedro.Billing.Domain.Server;
using Poliedro.Billing.Application.Server.Commands;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.Server.Dtos;

namespace Poliedro.Billing.Application.Server.Commands.UpdateServer
{

    public record UpdateServerCommand : IRequest<Result<VoidResult, Error>>
    {
        public int ServerId { get; init; }
        public string Ip { get; init; }
        public string DatabaseName { get; init; }
        public string DbUsername { get; init; }
        public string DbPassword { get; init; }
    }

    public record GetServerByIdCommand : IRequest<Result<ServerDto, Error>>
    {
        public int Id { get; init; }
    }
}