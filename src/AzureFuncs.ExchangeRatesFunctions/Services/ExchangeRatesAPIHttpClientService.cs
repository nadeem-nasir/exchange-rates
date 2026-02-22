using System.Net.Http.Json;

namespace AzureFuncs.ExchangeRatesFunctions.Services;

public interface IExchangeRatesAPIHttpClientService
{
    Task<DailyExchangeRatesResponseModel> GetLatestExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies);
}

public class ExchangeRatesAPIHttpClientService(HttpClient httpClient) : IExchangeRatesAPIHttpClientService
{
    private readonly HttpClient _httpClient = httpClient;

    private readonly string _exchangeratesAPIAccesskey = Environment.GetEnvironmentVariable("ExchangeRatesAPIAccesskey") ?? string.Empty;

    public async Task<DailyExchangeRatesResponseModel> GetLatestExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies)
    {
        var url = AddQueryString(endpointUrl: "latest", accesskey: _exchangeratesAPIAccesskey, baseCurrency: baseCurrency, targetCurrencies: string.Join(",", targetCurrencies));

        var httpResponse = await _httpClient.GetAsync(url);

        httpResponse.EnsureSuccessStatusCode();

        //  deserialize the content directly
        var results = await httpResponse.Content.ReadFromJsonAsync<DailyExchangeRatesResponseModel>();

        return results ?? new DailyExchangeRatesResponseModel();
    }

    private static string AddQueryString(string endpointUrl, string accesskey, string baseCurrency, string targetCurrencies)
    {
        return $"{endpointUrl}?access_key={accesskey}&base={baseCurrency}&symbols={targetCurrencies}";
    }

}

