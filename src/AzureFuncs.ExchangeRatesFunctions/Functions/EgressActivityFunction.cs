namespace AzureFuncs.ExchangeRatesFunctions.Functions;
public sealed class EgressActivityFunction(ILoggerFactory loggerFactory, IDailyExchangeRatesService dailyExchangeRatesService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EgressActivityFunction>();

    private readonly IDailyExchangeRatesService _dailyExchangeRatesService = dailyExchangeRatesService;

    /// <summary>
    /// // This function get the rates from table storage asynchronously
    /// </summary>
    /// <param name="exchangeRates"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(GetExchangeRatesToEgressAsync))]
    public async Task<List<DailyExchangeRatesModel>> GetExchangeRatesToEgressAsync([ActivityTrigger] FunctionContext executionContext)
    {
        return await _dailyExchangeRatesService.GetDailyExchangeRatesAsync(DateTime.UtcNow);
    }


    /// <summary>
    /// // This function egresses the exchange rates to Ax7 asynchronously
    /// </summary>
    /// <param name="exchangeRates"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(EgressExchangeRatesToAx7Async))]
    public async Task<bool> EgressExchangeRatesToAx7Async([ActivityTrigger] List<DailyExchangeRatesModel> exchangeRates, FunctionContext executionContext)
    {
        // Validate the exchange rates
        ArgumentNullException.ThrowIfNull(exchangeRates, nameof(exchangeRates));

        //Todo: implement the logic here

        return await Task.FromResult(true);
    }

    /// <summary>
    /// // This function egresses the exchange rates to Ax2009 asynchronously
    /// </summary>
    /// <param name="exchangeRates"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(EgressExchangeRatesToAx2009Async))]
    public async Task<bool> EgressExchangeRatesToAx2009Async([ActivityTrigger] List<DailyExchangeRatesModel> exchangeRates, FunctionContext executionContext)
    {
        // Validate the exchange rates
        ArgumentNullException.ThrowIfNull(exchangeRates, nameof(exchangeRates));

        //Todo: implement the logic here

        return await Task.FromResult(true);
    }

    /// <summary>
    /// // This function egresses the exchange rates to Dynamics365 asynchronously
    /// </summary>
    /// <param name="exchangeRates"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(EgressExchangeRatesToDynamics365Async))]
    public async Task<bool> EgressExchangeRatesToDynamics365Async([ActivityTrigger] List<DailyExchangeRatesModel> exchangeRates, FunctionContext executionContext)
    {
        // Validate the exchange rates
        ArgumentNullException.ThrowIfNull(exchangeRates, nameof(exchangeRates));

        //Todo: implement the logic here

        return await Task.FromResult(true);
    }

    /// <summary>
    /// // This function send the notification about exchange rates egress asynchronously
    /// </summary>
    /// <param name="exchangeRates"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(EgressExchangeRatesNotifyAsync))]
    public async Task<bool> EgressExchangeRatesNotifyAsync([ActivityTrigger] bool[] egressResults, FunctionContext executionContext)
    {
        // Validate the exchange rates
        ArgumentNullException.ThrowIfNull(egressResults);

        //Todo: implement the logic here

        return await Task.FromResult(true);
    }

}


