using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;

public class UpdateCompanyProviderCommandHandler(
    ICompanyProviderUpdateService _companyProviderUpdateService,
    IMapper mapper) :
    IRequestHandler<UpdateCompanyProviderCommand, CompanyProviderDto?>
{
    public async Task<CompanyProviderDto?> Handle(UpdateCompanyProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<CompanyProviderEntity>(request.Data);
        entity.CompanyProviderId = request.Id;

        var updated = await _companyProviderUpdateService.UpdateCompanyProviderAsync(entity, cancellationToken);

        return updated is null ? null : mapper.Map<CompanyProviderDto>(updated);
    }
}