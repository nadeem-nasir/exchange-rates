using System.Linq.Expressions;

namespace AzureFuncs.ExchangeRatesFunctions.Storage;

/// <summary>
/// This interface defines the methods for a generic repository for table storage
/// </summary>
public interface ITableStorageGenericRepository
{
    Task<bool> InsertOrMergeAsync(string tableName, IEnumerable<ITableEntity> entityToInsertOrMerge);
    Task<Response> UpdateAsync(string tableName, ITableEntity entityToUpdate);
    Task UpdateAsync(string tableName, IEnumerable<ITableEntity> entitiesToUpdate);
    Task<IEnumerable<T>> GetAsync<T>(string tableName, Expression<Func<T, bool>> filter, int? maxPerPage = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new();
    Task DeleteTableAsync(string tableName);

}
public class TableStorageGenericRepository(IStorageClientProvider storageClientProvider) : ITableStorageGenericRepository
{
    private readonly IStorageClientProvider _storageClientProvider = storageClientProvider;

    /// <summary>
    /// This method inserts or merges the given entities into the table asynchronously
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="entityToInsertOrMerge"></param>
    /// <returns></returns>
    public async Task<bool> InsertOrMergeAsync(string tableName, IEnumerable<ITableEntity> entityToInsertOrMerge)
    {
        // Create a table client for the given table name
        var tableClient = _storageClientProvider.CreateTableClient(tableName);
        // Create the table if it doesn't exist
        await tableClient.CreateIfNotExistsAsync();

        // Create a batch of update merge actions for the entities
        var batch = entityToInsertOrMerge.Select(entity => new TableTransactionAction(TableTransactionActionType.UpdateMerge, entity));
        var limitedBatch = batch.Chunk(50);
        var response = new List<bool>();
        // Submit the batch transaction asynchronously
        foreach (var batchAction in limitedBatch)
        {
            var limitedBatchResponse = await tableClient.SubmitTransactionAsync(batchAction);
            response.Add(limitedBatchResponse.Value.All(x => x.Status == 204));
        }

        return !response.Any(x => x == false);

    }

    /// <summary>
    /// // This method updates the given entity in the table asynchronously and returns the response
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="entityToUpdate"></param>
    /// <returns></returns>
    public async Task<Response> UpdateAsync(string tableName, ITableEntity entityToUpdate)
    {
        // Create a table client for the given table name
        var tableClient = _storageClientProvider.CreateTableClient(tableName);
        // Ensure the table exists or create it if not
        await tableClient.CreateIfNotExistsAsync();

        // Upsert the entity asynchronously and return the response
        var updateResult = await tableClient.UpsertEntityAsync(entityToUpdate);

        return updateResult;

    }

    /// <summary>
    /// This method updates the given entities in the table asynchronously
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="entitiesToUpdate"></param>
    /// <returns></returns>
    public async Task UpdateAsync(string tableName, IEnumerable<ITableEntity> entitiesToUpdate)
    {
        // Create a table client for the given table name
        var tableClient = _storageClientProvider.CreateTableClient(tableName);
        // Ensure the table exists or create it if not
        await tableClient.CreateIfNotExistsAsync();

        // Create a batch of upsert replace actions for the entities
        var batch = entitiesToUpdate.Select(entity => new TableTransactionAction(TableTransactionActionType.UpsertReplace, entity));

        // Submit the batch transaction asynchronously
        await tableClient.SubmitTransactionAsync(batch).ConfigureAwait(false);

    }

    /// <summary>
    ///This method gets the table entities asynchronously by applying a filter, a max per page limit and a select clause
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <param name="filter"></param>
    /// <param name="maxPerPage"></param>
    /// <param name="select"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetAsync<T>(string tableName, Expression<Func<T, bool>> filter, int? maxPerPage = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new()
    {
        // Create a table client for the given table name
        var tableClient = _storageClientProvider.CreateTableClient(tableName);

        // Ensure the table exists or create it if not
        await tableClient.CreateIfNotExistsAsync();

        // Query the table entities asynchronously with the given parameters
        var queryResultsMaxPerPage = tableClient.QueryAsync<T>(filter, maxPerPage, select);

        // Return the query results as a list of T objects

        return await Query(queryResultsMaxPerPage);
    }

    public async Task DeleteTableAsync(string tableName)
    {
        // Create a table client for the given table name
        var tableClient = _storageClientProvider.CreateTableClient(tableName);

        // Delete the table 
        await tableClient.DeleteAsync();

    }
    /// <summary>
    /// This method queries the table entities asynchronously and returns them as a list of T objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryResultsMaxPerPage"></param>
    /// <returns></returns>
    private static async Task<IEnumerable<T>> Query<T>(AsyncPageable<T> queryResultsMaxPerPage) where T : class, ITableEntity, new()
    {
        // Use a list to store the table entities
        var entities = new List<T>();
        // Iterate over the pages asynchronously
        await foreach (var page in queryResultsMaxPerPage.AsPages())
        {
            // Iterate over the values in each page
            foreach (var qEntity in page.Values)
            {
                // Cast and add the table entity to the list
                entities.Add(qEntity ?? new T());
            }
        }

        return entities;
    }

}
