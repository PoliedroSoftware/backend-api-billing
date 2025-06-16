using MediatR;
using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Commands.CreateClient
{
    public record CreateClientCommand : IRequest<Result<VoidResult, Error>>
    {
        public string Name { get; set; } = string.Empty;
        public int ResolutionId { get; set; } = default!;
        public int ServerId { get; set; } = default!;
        public ProviderType ProviderId { get; set; } = default!;
        public bool Active { get; set; } = default!;
        public int Iterations { get; set; } = default!;
        public DateTime Date { get; set; } = default!;
    }
}
