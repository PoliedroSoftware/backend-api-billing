using AutoMapper;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Application.Server.AutoMappers
{
    public class PaginationMapper : Profile
    {
        public PaginationMapper()
        {
            CreateMap<PaginationResponse<ServerEntity>, PaginationResponseDto<ServerDto>>().ReverseMap();
        }
    }
}
