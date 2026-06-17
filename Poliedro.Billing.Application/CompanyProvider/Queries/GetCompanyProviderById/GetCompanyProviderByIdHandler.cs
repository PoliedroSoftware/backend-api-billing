
using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;

public class GetCompanyProviderByIdHandler(
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IMapper mapper) : 
    IRequestHandler<GetCompanyProviderByIdQuery, CompanyProviderDto>
{
    public async Task<CompanyProviderDto> Handle(GetCompanyProviderByIdQuery request, CancellationToken cancellationToken)
    {
        CompanyProviderEntity CompanyProviderEntity = await _companyProviderGetByIdService.GetCompanyProviderByIdAsync(request.Id, cancellationToken);

        return mapper.Map<CompanyProviderDto>(CompanyProviderEntity);
    }
}