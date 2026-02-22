namespace AzureFuncs.ExchangeRatesFunctions.Functions;
public sealed class IngressTriggerFunction
{
    private readonly ILogger _logger;

    private readonly IExchangeRatesAPIHttpClientService _exchangeRatesAPIHttpClientService;

    private readonly ITargetCurrencyService _targetCurrencyService;

    private readonly IDailyExchangeRatesService _dailyExchangeRatesService;

    public IngressTriggerFunction(ILoggerFactory loggerFactory,

        IExchangeRatesAPIHttpClientService exchangeRatesAPIHttpClientService,

            ITargetCurrencyService targetCurrencyService,

            IDailyExchangeRatesService dailyExchangeRatesService)
    {
        _logger = loggerFactory.CreateLogger<IngressTriggerFunction>();

        _exchangeRatesAPIHttpClientService = exchangeRatesAPIHttpClientService;

        _targetCurrencyService = targetCurrencyService;

        _dailyExchangeRatesService = dailyExchangeRatesService; ;
    }

    /// <summary>
    /// This function ingests the daily exchange rates from a HTTP trigger
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function(nameof(Ingest_DailyExchangeRates_HttpTrigger))]
    public async Task<HttpResponseData> Ingest_DailyExchangeRates_HttpTrigger([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        // Log the request
        _logger.LogInformation("Ingest_DailyExchangeRates function processed a request.");

        // Ingest the daily exchange rates for EUR
        var retResult = await IngestDailyExchangeRates("EUR");

        // Create a response with OK status
        var response = req.CreateResponse(HttpStatusCode.OK);

        // Set the content type and encoding
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        // Write the result to the response
        response.WriteString($"Daily exchange rates  result is {retResult}");

        // Return the response
        return response;
    }

    /// <summary>
    /// This function ingests the daily exchange rates from a schedule trigger
    /// </summary>
    /// <param name="timerInfo"></param>
    /// <returns></returns>

    [Function(nameof(Ingest_DailyExchangeRates_ScheduleTrigger))]
    public async Task Ingest_DailyExchangeRates_ScheduleTrigger([TimerTrigger("0 1 * * *")] TimerInfoModel timerInfo)
    {
        // Ingest the daily exchange rates for EUR
        _ = await IngestDailyExchangeRates("EUR");
    }

    /// <summary>
    /// This function ingests the daily exchange rates from a queue trigger
    /// </summary>
    /// <param name="myQueueItem"></param>
    /// <returns></returns>
    [Function(nameof(Ingest_DailyExchangeRates_QueueTrigger))]
    public async Task Ingest_DailyExchangeRates_QueueTrigger([QueueTrigger("ingest-daily-exchange-rates-queue-trigger", Connection = "AzureWebJobsStorage")] string myQueueItem)
    {
        // Ingest the daily exchange rates for EUR
        _ = await IngestDailyExchangeRates("EUR");

    }

    /// <summary>
    /// This function ingests the daily exchange rates from a blob trigger
    /// </summary>
    /// <param name="myBlob"></param>
    /// <param name="name"></param>
    /// <returns></returns>

    [Function(nameof(Ingest_DailyExchangeRates_BlobTrigger))]
    public async Task Ingest_DailyExchangeRates_BlobTrigger([BlobTrigger("ingest-daily-exchange-rates-blob-trigger/{name}", Connection = "AzureWebJobsStorage")] string myBlob, string name)
    {
        // Ingest the daily exchange rates for EUR
        _ = await IngestDailyExchangeRates("EUR");

    }

    private async ValueTask<bool> IngestDailyExchangeRates(string baseCurrency = "EUR")
    {
        var targetCurrencies = await _targetCurrencyService.GetTargetCurrencyAsync();

        var target = new List<string>(targetCurrencies);

        var dailyExchangeRates = await _exchangeRatesAPIHttpClientService.GetLatestExchangeRatesAsync(baseCurrency, target);

        return await _dailyExchangeRatesService.DailyExchangeRatesInsertOrMergeAsync(dailyExchangeRates);
    }

}

