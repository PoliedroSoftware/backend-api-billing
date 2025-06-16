using MediatR;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Server.Queries.GellAllServer;

public record GellAllServerQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<ServerDto>>;
