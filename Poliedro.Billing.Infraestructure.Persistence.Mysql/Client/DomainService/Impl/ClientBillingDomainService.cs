using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientBillingDomainService(
    IClientCreateService createService,
    IClientUpdateService updateService,
    IClientGetAllService getAllService,
    IClientGetByIdService getByIdService,
    IClientGetByTokenService getByTokenService,
    IClientDeleteService deleteService) : IClientDomainService
{
    public Task<Result<VoidResult, Error>> CreateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
        => createService.CreateAsync(clientBillingElectronicEntity, cancellationToken);

    public Task<Result<VoidResult, Error>> UpdateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
        => updateService.UpdateAsync(clientBillingElectronicEntity, cancellationToken);

    public Task<Result<IEnumerable<ClientEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
        => getAllService.GetAllAsync(cancellationToken);

    public Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
        => getByIdService.GetByIdAsync(id, cancellationToken);

    public Task<Result<ClientEntity, Error>> GetByIdAsync(string Apikey, CancellationToken cancellationToken)
        => getByIdService.GetByIdAsync(Apikey, cancellationToken);

    public Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken)
        => getByTokenService.GetByTokenAsync(token, cancellationToken);

    public Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken)
        => deleteService.DeleteAsync(id, cancellationToken);
}
