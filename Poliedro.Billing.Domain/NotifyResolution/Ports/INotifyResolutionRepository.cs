using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Domain.NotifyResolution.Ports;
public interface INotifyResolutionRepository
{
    Task<DianResolutionEntity?> GetNotifyResolution(NotifyResolutionParameters Parameters);
}