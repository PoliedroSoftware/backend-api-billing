using MediatR;
using Poliedro.Billing.Domain.CustomersId.Entities;

namespace Poliedro.Billing.Application.CustomersId.Queries.CustomersbyId;

public record CustomersIdQuery(string id, string Token) : IRequest<Customers>;
