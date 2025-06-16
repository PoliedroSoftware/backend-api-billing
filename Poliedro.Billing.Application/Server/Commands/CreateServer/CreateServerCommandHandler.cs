using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Application.Server.Commands.CreateServer
{
    public class CreateServerCommandHandler(
        IServerDomainService serverDomainService,
        IMapper mapper) : IRequestHandler<CreateServerCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            var serverEntity = mapper.Map<ServerEntity>(request);
            var result = await serverDomainService.CreateAsync(serverEntity);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}




