using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution
{
    public record DeleteDianResolutionCommand : IRequest<Result<VoidResult, Error>>
    {
        public int Id { get; init; }
    }
}
