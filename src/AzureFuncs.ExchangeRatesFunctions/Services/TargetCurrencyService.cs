namespace AzureFuncs.ExchangeRatesFunctions.Services;
public interface ITargetCurrencyService
{
    // ValueTask instead of Task for better performance
    ValueTask<bool> SeedTargetCurrencyAsync();

    // IReadOnlyList instead of List for better encapsulation
    ValueTask<IReadOnlyList<string>> GetTargetCurrencyAsync();

    ValueTask<string> GetTargetCurrencyAsCommaSeparatedAsync();

}

public class TargetCurrencyService(ITableStorageGenericRepository tableStorageGenericRepository) : ITargetCurrencyService
{
    private readonly ITableStorageGenericRepository _tableStorageGenericRepository = tableStorageGenericRepository ?? throw new ArgumentNullException(nameof(tableStorageGenericRepository));

    public async ValueTask<bool> SeedTargetCurrencyAsync()
    {
        var targetCurrencies = new[] { "USD", "GBP", "JPY" };

        // parallelize the Select operation
        var targetCurrencyEntities = targetCurrencies.AsParallel().Select(x => new TargetCurrencyEntity(TableStorageConstants.TargetCurrencyTablePartitionKey, x)).ToList();

        return await _tableStorageGenericRepository.InsertOrMergeAsync(TableStorageConstants.TargetCurrencyTableName, targetCurrencyEntities);
    }

    public async ValueTask<IReadOnlyList<string>> GetTargetCurrencyAsync()
    {
        return await GetCurrencyAsync();
    }

    public async ValueTask<string> GetTargetCurrencyAsCommaSeparatedAsync()
    {
        var result = await GetCurrencyAsync();

        return string.Join(",", result);
    }

    private async ValueTask<IReadOnlyList<string>> GetCurrencyAsync()
    {
        var result = await _tableStorageGenericRepository.GetAsync<TargetCurrencyEntity>(TableStorageConstants.TargetCurrencyTableName, ent => ent.PartitionKey == TableStorageConstants.TargetCurrencyTablePartitionKey);

        // Use ToArray instead of ToList for better performance
        return result.Select(x => x.TargetCurrencyCode).ToArray();
    }
}

