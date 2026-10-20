using MediatR;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.DeleteCompanyProvider;

public class DeleteCompanyProviderCommandHandler(
    ICompanyProviderDeleteService _companyProviderDeleteService) :
    IRequestHandler<DeleteCompanyProviderCommand, bool>
{
    public Task<bool> Handle(DeleteCompanyProviderCommand request, CancellationToken cancellationToken)
        => _companyProviderDeleteService.DeleteCompanyProviderAsync(request.Id, cancellationToken);
}