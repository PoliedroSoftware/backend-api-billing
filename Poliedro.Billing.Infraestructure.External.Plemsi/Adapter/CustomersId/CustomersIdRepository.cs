using Poliedro.Billing.Domain.CustomersId.Ports;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Headers;
using Poliedro.Billing.Domain.CustomersId.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CustomersId;

public class CustomersIdRepository(IConfiguration configuration) : ICustomersIdRepository
{
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Customers> GetByIdAsync(string id, string token)
    {

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string baseUrl = configuration["ApiPlemsi:BaseUrl"];
        string url = $"{baseUrl}/customer/{id}";
        HttpResponseMessage response = await client.GetAsync(url);

        string responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<Customers>>(responseData, jsonOptions);
            return apiResponse?.Data!;

        }

        return null!;
    }
}