using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Commands.UpdateClient
{
    public record UpdateClientCommand : IRequest<Result<VoidResult, Error>>
    {
        public int CompanyId { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool Active { get; set; } = default!;
    }
}