using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Infraestructure.External.Siigo.ConfigModels;

namespace Poliedro.Billing.Infraestructure.External.Siigo.Services.Imp
{
    public class SettingsService : ISettingsService
    {
        private readonly IConfiguration _config;
        private readonly AppSettings _settings;

        public SettingsService(IConfiguration config)
        {
            _config = config;
            _settings = new AppSettings()
            {
                CreateInvoceAndSendDianEndPoint = _config["ApiSiigo:createInvoceAndSendDianEndPoint"],
                HostUrl = _config["ApiSiigo:hostUrl"],
                mediaType = _config["ApiSiigo:mediaType"],
            };
        }

        public AppSettings GetSettings()
        {
            return _settings;
        }

        public string GetValue(string key)
        {
            return _settings.GetType().GetProperty(key)?.GetValue(_settings)?.ToString();
        }
    }
}
