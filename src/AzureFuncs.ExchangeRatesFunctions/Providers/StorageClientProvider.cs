namespace AzureFuncs.ExchangeRatesFunctions.Providers;

public interface IStorageClientProvider
{
    TableClient CreateTableClient(string tableName);
    TableServiceClient CreateTableServiceClient();
}

public class StorageClientProvider(string connectionString) : IStorageClientProvider
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    private static TableServiceClient? _tableServiceClient;
    public TableClient CreateTableClient(string tableName)
    {
        return new TableClient(_connectionString, tableName);
    }
    public TableServiceClient CreateTableServiceClient()
    {
        _tableServiceClient ??= new TableServiceClient(_connectionString);
        
        return _tableServiceClient;
    }
}

