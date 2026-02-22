namespace AzureFuncs.ExchangeRatesFunctions.Services;
public interface IDailyExchangeRatesService
{
    ValueTask<bool> DailyExchangeRatesInsertOrMergeAsync(DailyExchangeRatesResponseModel dailyExchangeRatesResponseModel);

    Task<List<DailyExchangeRatesModel>> GetDailyExchangeRatesAsync(DateTime dailyExchangeRatesDate);

}
public class DailyExchangeRatesService(ITableStorageGenericRepository tableStorageGenericRepository) : IDailyExchangeRatesService
{
    private readonly ITableStorageGenericRepository _tableStorageGenericRepository = tableStorageGenericRepository ?? throw new ArgumentNullException(nameof(tableStorageGenericRepository));

    public async ValueTask<bool> DailyExchangeRatesInsertOrMergeAsync(DailyExchangeRatesResponseModel dailyExchangeRatesResponseModel)
    {
        var dailyExchangeRatesEntities = dailyExchangeRatesResponseModel.Rates.AsParallel().Select(x => new DailyExchangeRatesEntity(TableStorageConstants.DailyExchangeRatesPartitionKey, x.Key, x.Value)).ToList();

        var tableName = GetDailyExchangeRatesTableName(TableStorageConstants.DailyExchangeRatesTableName, dailyExchangeRatesResponseModel.Date);

        return await _tableStorageGenericRepository.InsertOrMergeAsync(tableName, dailyExchangeRatesEntities);
    }

    public async Task<List<DailyExchangeRatesModel>> GetDailyExchangeRatesAsync(DateTime dailyExchangeRatesDate)
    {
        var tableName = GetDailyExchangeRatesTableName(TableStorageConstants.DailyExchangeRatesTableName, dailyExchangeRatesDate);

        var dailyExchangeRatesEntities = await _tableStorageGenericRepository.GetAsync<DailyExchangeRatesEntity>(tableName, ent => ent.PartitionKey == TableStorageConstants.DailyExchangeRatesPartitionKey);

        return dailyExchangeRatesEntities.Select(x => new DailyExchangeRatesModel(x.TargetCurrencyCode, x.ExchangeRate)).ToList();
    }


    /// <summary>
    /// Table names may contain only alphanumeric characters. Table names cannot begin with a numeric character. 
    /// Table names are case-insensitive. 
    /// Table names must be from 3 to 63 characters long.
    /// </summary>
    /// <param name="dailyExchangeRatesTableName"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private static string GetDailyExchangeRatesTableName(string dailyExchangeRatesTableName, DateTime prefix)
    {
        return $"{dailyExchangeRatesTableName}{prefix:yyyyMMdd}";
    }

}

