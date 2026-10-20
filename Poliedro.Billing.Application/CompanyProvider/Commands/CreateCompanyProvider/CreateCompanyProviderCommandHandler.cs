
using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;

public class CreateCompanyProviderCommandHandler(
    ICompanyProviderCreateService _companyProviderCreateService,
    IMapper mapper) :
    IRequestHandler<CreateCompanyProviderCommand, CompanyProviderDto>
{
    public async Task<CompanyProviderDto> Handle(CreateCompanyProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<CompanyProviderEntity>(request.Data);
        var created = await _companyProviderCreateService.CreateCompanyProviderAsync(entity, cancellationToken);
        return mapper.Map<CompanyProviderDto>(created);
    }
}