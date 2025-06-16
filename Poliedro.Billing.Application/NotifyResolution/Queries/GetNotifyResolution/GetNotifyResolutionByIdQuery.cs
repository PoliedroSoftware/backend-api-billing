using MediatR;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Domain.NotifyResolution.Ports;
namespace Poliedro.Billing.Application.NotifyResolution.Queries.GetNotifyResolution;
public record GetNotifyResolutionByIdQuery(NotifyResolutionParameters Parameters) : IRequest<NotifyResolutionDto>;