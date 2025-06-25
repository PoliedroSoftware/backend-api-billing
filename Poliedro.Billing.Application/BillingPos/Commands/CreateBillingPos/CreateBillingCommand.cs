using MediatR;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public record CreateBillingCommand(CreateBilling) : IRequest<Result<ApiResponseBillingPos, Error>>;




