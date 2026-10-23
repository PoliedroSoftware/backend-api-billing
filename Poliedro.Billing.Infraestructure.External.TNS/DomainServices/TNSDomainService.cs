using Newtonsoft.Json;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.TNS.Ports;
using Poliedro.Billing.Application.TNS.Contracts.Models;
using Poliedro.Billing.Application.TNS.Contracts.Responses;
using Poliedro.Billing.Infraestructure.External.TNS.ConfigModels;
using Poliedro.Billing.Infraestructure.External.TNS.Services;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.TNS.DomainServices
{
    public class TNSDomainService(ISettingsService settingsService) : ITNSGateway
    {

        public async Task<Result<ApiResponseTNS, Error>> CreateAsync(List<Order> request, string codigosucursal, string token, CancellationToken cancellationToken)
        {
            ApiResponseTNS responseTNS = new ApiResponseTNS()
            {
                Status = false,
                Message = string.Empty,
                Data = new List<ApiDataResponseTNS>()
            };

            AppSettings config = settingsService.GetSettings();

            foreach (var item in request)
            {
                string jsonContent = JsonConvert.SerializeObject(item);

                StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, config.mediaType);
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                UriBuilder builder = new UriBuilder($"{config.HostUrl}{config.CreateSaleEndPoint}");
                var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

                query["codigosucursal"] = codigosucursal;
                builder.Query = query.ToString();

                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, builder.Uri)
                {
                    Content = stringContent
                };

                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(config.mediaType));
                httpRequest.Headers.Add("User-Agent", "MyApp/1.0");

                HttpResponseMessage response = await client.SendAsync(httpRequest, cancellationToken);
               
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ApiRestResponseTNS responseApi = JsonConvert.DeserializeObject<ApiRestResponseTNS>(jsonResponse);

                    if (responseApi is not null && responseApi.Status)
                    {
                        responseTNS.Status = true;
                        responseTNS.Message = responseApi.Message;
                        responseTNS.Data.Add(responseApi.Data);
                    }
                }
                else
                {
                    var responseX = await response.Content.ReadAsStringAsync();
                    responseTNS.Status = false;
                    responseTNS.Message = responseX;
                }
            }
            return responseTNS;
        }
    }
}
