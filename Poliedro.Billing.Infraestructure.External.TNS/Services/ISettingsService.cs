using Poliedro.Billing.Infraestructure.External.TNS.ConfigModels;

namespace Poliedro.Billing.Infraestructure.External.TNS.Services
{
    public interface ISettingsService
    {
        AppSettings GetSettings();
        string GetValue(string key);
    }
}
