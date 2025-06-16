using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Application.NotifyResolution.Exceptions;
using Poliedro.Billing.Application.NotifyResolution.Queries.GetNotifyResolution;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.NotifyResolution.Services;

public class GetNotifyResolutionByIdHandler(
    IClientDomainService clientDomainService,
    IMapper mapper,
    INotifyResolutionAlertService notifyResolutionAlertService
    ) : IRequestHandler<GetNotifyResolutionByIdQuery, NotifyResolutionDto>
{
    public async Task<NotifyResolutionDto> Handle(GetNotifyResolutionByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByIdAsync(request.Parameters.ApiKey, cancellationToken);

        if (client?.Value?.DianResolution == null)
        {
            throw new NotifyResolutionNotFoundException(request.Parameters.ApiKey);
        }

        var resolutionDto = mapper.Map<NotifyResolutionDto>(client.Value.DianResolution);

        var alert = notifyResolutionAlertService.GenerateAlert(
            client.Value.DianResolution.ExpirationDate,
            client.Value.DianResolution.ExpirationDay,
            client.Value.DianResolution.FinalRange,
            client.Value.DianResolution.CurrentlyNumber
        );
        var alertDto = new ExpirationAlertDto(alert.IsExpired, alert.IsCloseToExpire, alert.Message, alert.DaysToExpire, alert.RemainingNumeration, alert.StatusNumeration);

        return resolutionDto with { ExpirationAlert = alertDto };
    }
}
