using Poliedro.Billing.Domain.NotifyResolution.Entities;
using Poliedro.Billing.Domain.NotifyResolution.Services;

namespace Poliedro.Billing.Application.NotifyResolution.Services;

public class NotifyResolutionAlertService : INotifyResolutionAlertService
{
    public ExpirationAlert GenerateAlert(DateTime expirationDate, int expirationDays , int finalRange, int currentlyNumber)
    {
        var alertDate = expirationDate.AddDays(-expirationDays);
        var today = DateTime.Today;
        var daysToExpire = (expirationDate - today).Days;

        var remainingNumeration = finalRange - currentlyNumber;

        var statusNumeration = remainingNumeration > 0 ? true : false;

        var alert = new ExpirationAlert();

        if (today >= alertDate && today <= expirationDate)
        {
            alert.IsExpired = false;
            alert.IsCloseToExpire = true;
            alert.Message = $"¡The resolution will expire on {expirationDate:yyyy-MM-dd}!";
            alert.DaysToExpire = daysToExpire;
            alert.RemainingNumeration = remainingNumeration;
        }
        else if (today > expirationDate)
        {
            alert.IsExpired = true;
            alert.IsCloseToExpire = false;
            alert.Message = $"¡The resolution has already expired on {expirationDate:yyyy-MM-dd}!";
            alert.DaysToExpire = null;
            alert.RemainingNumeration = remainingNumeration;
        }
        else
        {
            alert.IsExpired = false;
            alert.IsCloseToExpire = false;
            alert.Message = "Current";
            alert.DaysToExpire = daysToExpire;
            alert.RemainingNumeration = remainingNumeration;
        }

        return alert;
    }
}