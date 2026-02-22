namespace AzureFuncs.ExchangeRatesFunctions.Storage;
public class TargetCurrencyEntity : ITableEntity
{
    public TargetCurrencyEntity()
    {

    }
    public TargetCurrencyEntity(string partitionKey, string targetCurrencyCode) => (PartitionKey, RowKey, TargetCurrencyCode) = (partitionKey, targetCurrencyCode, targetCurrencyCode);

    public string PartitionKey { get; set; } = default!;

    public string RowKey { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string TargetCurrencyCode { get; set; } = default!;
}
