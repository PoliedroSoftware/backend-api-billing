using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution
{
    public class DeleteDianResolutionCommandHandler(
        IDianResolutionDomainService dianResolutionDomainService,
        IMapper mapper) : IRequestHandler<DeleteDianResolutionCommand, Result<VoidResult, Error>>
    {
        public readonly IMapper mapper = mapper;

        public async Task<Result<VoidResult, Error>> Handle(DeleteDianResolutionCommand request, CancellationToken cancellationToken)
        {
            var result = await dianResolutionDomainService.DeleteAsync(new DianResolutionEntity { ResolutionId = request.Id }, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}