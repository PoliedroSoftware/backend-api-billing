using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution
{
    public class UpdateDianResolutionCommandHandler(
        IDianResolutionDomainService dianResolutionDomainService,
        IMapper mapper) : IRequestHandler<UpdateDianResolutionCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(UpdateDianResolutionCommand request, CancellationToken cancellationToken)
        {
            var dianResolutionEntity = mapper.Map<DianResolutionEntity>(request);

            var result = await dianResolutionDomainService.UpdateAsync(dianResolutionEntity, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }

}
