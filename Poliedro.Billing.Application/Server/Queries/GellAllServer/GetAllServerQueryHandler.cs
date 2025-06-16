using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Server.DomainService;

namespace Poliedro.Billing.Application.Server.Queries.GellAllServer;
public class GetAllServerQueryHandler
(
    IServerDomainService serverDomainService,
    IMapper mapper)
    : IRequestHandler<GellAllServerQuery, IEnumerable<ServerDto>>
{
    public async Task<IEnumerable<ServerDto>> Handle(GellAllServerQuery request, CancellationToken cancellationToken)
    {
        var result = await serverDomainService.GetAllAsync(request.PaginationParams);
        return mapper.Map<List<ServerDto>>(result);
    }
}


