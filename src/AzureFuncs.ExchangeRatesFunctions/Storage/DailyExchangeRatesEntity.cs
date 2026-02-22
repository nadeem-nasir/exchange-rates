namespace AzureFuncs.ExchangeRatesFunctions.Storage;
public class DailyExchangeRatesEntity : ITableEntity
{
    public DailyExchangeRatesEntity()
    {

    }
    public DailyExchangeRatesEntity(string partitionKey, string targetCurrencyCode, double exchangeRate) => (PartitionKey, RowKey, TargetCurrencyCode, ExchangeRate) = (partitionKey, targetCurrencyCode, targetCurrencyCode, exchangeRate);

    public string PartitionKey { get; set; } = default!;

    public string RowKey { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string TargetCurrencyCode { get; set; } = default!;

    public double ExchangeRate { get; set; }
}
