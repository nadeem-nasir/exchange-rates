namespace AzureFuncs.ExchangeRatesFunctions.Models;

public class DailyExchangeRatesResponseModel
{
    public bool Success { get; set; }

    public int Timestamp { get; set; }

    public string Base { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public Dictionary<string, double> Rates { get; set; } = default!;

}

