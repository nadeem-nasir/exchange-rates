using Microsoft.DurableTask;

namespace AzureFuncs.ExchangeRatesFunctions.Functions;

public static class EgressOrchestrator
{

    /// <summary>
    /// This function starts the egress after triggering
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [Function(nameof(EgressOrchestratorStartFunction))]
    public static async Task<bool> EgressOrchestratorStartFunction(
    [OrchestrationTrigger] TaskOrchestrationContext context)
    {

        ILogger logger = context.CreateReplaySafeLogger(nameof(EgressOrchestratorStartFunction));

        logger.LogInformation("Starting EgressOrchestratorStartFunction");

        //An example of fan out/fan in pattern

        LogInformation(context, logger, "Get the exchange rates to egress", "Get the exchange rates to egress");

        // Get the exchange rates to egress
        var dailyExchangeRates = await context.CallActivityAsync<List<DailyExchangeRatesModel>>(nameof(EgressActivityFunction.GetExchangeRatesToEgressAsync));

        // Egress the exchange rates to different destinations in parallel
        var egressParallelTasks = new List<Task<bool>>
        {
            context.CallActivityAsync<bool>(nameof(EgressActivityFunction.EgressExchangeRatesToAx7Async), dailyExchangeRates),
            context.CallActivityAsync<bool>(nameof(EgressActivityFunction.EgressExchangeRatesToAx2009Async), dailyExchangeRates),
            context.CallActivityAsync<bool>(nameof(EgressActivityFunction.EgressExchangeRatesToDynamics365Async), dailyExchangeRates)
        };

        // Wait for all the egress tasks to finish
        var egressResults = await Task.WhenAll(egressParallelTasks);

        // Notify the result of the egress
        return await context.CallActivityAsync<bool>(nameof(EgressActivityFunction.EgressExchangeRatesNotifyAsync), egressResults);

    }

    static void LogInformation(TaskOrchestrationContext context, ILogger logger, string message, string status)
    {
        if (!context.IsReplaying)
        {
            logger.LogInformation(message);

            context.SetCustomStatus(new { status });
        }
    }

}

