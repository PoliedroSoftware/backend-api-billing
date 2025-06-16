using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;


namespace Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution
{
    public class CreateDianResolutionCommandHandler(
        IDianResolutionDomainService dianResolutionDomainService,
        IMapper mapper) : IRequestHandler<CreateDianResolutionCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(CreateDianResolutionCommand request, CancellationToken cancellationToken)
        {
            var dianResolutionEntity = mapper.Map<DianResolutionEntity>(request);
            var result = await dianResolutionDomainService.CreateAsync(dianResolutionEntity, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
