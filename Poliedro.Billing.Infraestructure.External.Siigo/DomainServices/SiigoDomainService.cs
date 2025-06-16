using Newtonsoft.Json;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Siigo.DomainServices;
using Poliedro.Billing.Domain.Siigo.Models;
using Poliedro.Billing.Domain.Siigo.ResponseModels;
using Poliedro.Billing.Infraestructure.External.Siigo.ConfigModels;
using Poliedro.Billing.Infraestructure.External.Siigo.Services;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Siigo.DomainServices
{
    public class SiigoDomainService(ISettingsService settingsService) : ISiigoDomainService
    {

        public async Task<Result<ApiResponseSiigo, Error>> CreateAndSendDianAsync(List<Invoice> request, string token, CancellationToken cancellationToken)
        {
            ApiResponseSiigo responseSiigo = new ApiResponseSiigo()
            {
                Status = false,
                Message = string.Empty,
                Data = new List<dynamic>()
            };

            AppSettings config = settingsService.GetSettings();

            foreach (var item in request)
            {
                string jsonContent = JsonConvert.SerializeObject(item);

                StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, config.mediaType);
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                UriBuilder builder = new UriBuilder($"{config.HostUrl}{config.CreateInvoceAndSendDianEndPoint}");
                
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, builder.Uri)
                {
                    Content = stringContent
                };

                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(config.mediaType));
                httpRequest.Headers.Add("User-Agent", "MyApp/1.0");
                httpRequest.Headers.Add("Partner-ID", "Test");

                HttpResponseMessage response = await client.SendAsync(httpRequest);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ApiRestSuccessResponseSiigo responseApi = JsonConvert.DeserializeObject<ApiRestSuccessResponseSiigo>(jsonResponse);

                    if (responseApi is not null)
                    {
                        responseSiigo.Data.Add(responseApi);
                        responseSiigo.Status = true;
                    }
                    else
                    {
                        responseSiigo.Status = false;

                    }
                }//ApiRestErrorResponseSiigo
                else
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ApiRestErrorResponseSiigo responseApi = JsonConvert.DeserializeObject<ApiRestErrorResponseSiigo>(jsonResponse);
                    responseSiigo.Status = false;
                    responseSiigo.Message = jsonResponse;
                    responseSiigo.Data.Add(responseApi);
                }
            }
            return responseSiigo;
        }
    }
}
