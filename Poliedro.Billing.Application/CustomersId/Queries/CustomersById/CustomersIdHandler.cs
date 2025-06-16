using MediatR;
using Poliedro.Billing.Domain.CustomersId.Ports;
using Poliedro.Billing.Domain.CustomersId.Entities;
using Microsoft.Extensions.Logging;

namespace Poliedro.Billing.Application.CustomersId.Queries.CustomersbyId;

public class CustomersIdHandler : IRequestHandler<CustomersIdQuery, Customers>
{
    private readonly ICustomersIdRepository _repository;
    private readonly ILogger<CustomersIdHandler> _logger;

    public CustomersIdHandler(ICustomersIdRepository repository)
    {
        _repository = repository;
    }

    public async Task<Customers> Handle(CustomersIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.id, request.Token);
        if (customer == null)
        {
            _logger?.LogWarning("Cliente con ID {Id} no encontrado.", request.id);
            return new Customers();
        }
        return customer;
    }
}