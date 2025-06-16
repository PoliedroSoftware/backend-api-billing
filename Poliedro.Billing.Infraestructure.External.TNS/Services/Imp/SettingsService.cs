using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Infraestructure.External.TNS.ConfigModels;

namespace Poliedro.Billing.Infraestructure.External.TNS.Services.Imp
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
                CreateSaleEndPoint = _config["ApiTNS:createSaleEndPoint"],
                HostUrl = _config["ApiTNS:hostUrl"],
                mediaType = _config["ApiTNS:mediaType"],
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
