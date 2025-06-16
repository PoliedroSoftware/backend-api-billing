using Poliedro.Billing.Infraestructure.External.Siigo.ConfigModels;

namespace Poliedro.Billing.Infraestructure.External.Siigo.Services
{
    public interface ISettingsService
    {
        AppSettings GetSettings();
        string GetValue(string key);
    }
}
