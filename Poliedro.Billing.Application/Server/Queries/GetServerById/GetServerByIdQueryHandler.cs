using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;

namespace Poliedro.Billing.Application.Server.Queries.GetServerById
{
    public class GetServerByIdQueryHandler(
        IServerDomainService serverDomainService,
        IMapper mapper)
        : IRequestHandler<GetServerByIdQuery, Result<ServerDto, Error>>
    {
        public async Task<Result<ServerDto, Error>> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await serverDomainService.GetByIdAsync(request.Id);
            if (!result.IsSuccess)
                return result.Error!;

            return mapper.Map<ServerDto>(result.Value);
        }
    }
}
