using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Application.Server.Commands.UpdateServer
{
    public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, Result<VoidResult, Error>>
    {
        private readonly IServerDomainService _serverDomainService;
        private readonly IMapper _mapper;

        public UpdateServerCommandHandler(IServerDomainService serverDomainService, IMapper mapper)
        {
            _serverDomainService = serverDomainService;
            _mapper = mapper;
        }

        public async Task<Result<VoidResult, Error>> Handle(UpdateServerCommand request,
            CancellationToken cancellationToken)
        {
            var serverEntity = _mapper.Map<ServerEntity>(request);


            var result = await _serverDomainService.UpdateAsync(serverEntity);


            if (!result.IsSuccess)
                return result.Error!;


            return result.Value!;
        }
    }
}
