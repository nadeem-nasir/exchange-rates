namespace AzureFuncs.ExchangeRatesFunctions.Extensions;
public static class StartupExtensions
{
    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        var storageAccountConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        ArgumentNullException.ThrowIfNull(storageAccountConnectionString, nameof(storageAccountConnectionString));
        services.AddSingleton<IStorageClientProvider>(c => new StorageClientProvider(storageAccountConnectionString));

        return services;
    }

    /// <summary>
    /// Add services to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExchangeRatesAPIHttpClientService, ExchangeRatesAPIHttpClientService>(c => {
            c.BaseAddress = new Uri("http://api.exchangeratesapi.io/v1/");
        });

        services.AddTransient<ITargetCurrencyService, TargetCurrencyService>();

        services.AddTransient<IDailyExchangeRatesService, DailyExchangeRatesService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ITableStorageGenericRepository, TableStorageGenericRepository>();

        return services;
    }
}
