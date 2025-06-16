using Poliedro.Billing.Domain.NotifyResolution.Entities;
namespace Poliedro.Billing.Domain.NotifyResolution.Services
{
        public interface INotifyResolutionAlertService
        {
        ExpirationAlert GenerateAlert(DateTime expirationDate, int expirationDays, int finalRange, int currentlyNumber);
        }
}