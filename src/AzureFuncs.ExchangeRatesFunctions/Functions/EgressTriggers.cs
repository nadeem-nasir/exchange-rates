using Microsoft.DurableTask.Client;

namespace AzureFuncs.ExchangeRatesFunctions.Functions;

public static class EgressTriggers
{
    /// <summary>
    /// starting point for the egress
    /// </summary>
    /// <param name="timerInfo"></param>
    /// <param name="client"></param>
    /// <param name="executionContext"></param>
    /// <returns></returns>
    [Function(nameof(EgressOrchestratorFunction_TimerStart))]
    [FixedDelayRetry(5, "00:00:10")]
    public static async Task<string> EgressOrchestratorFunction_TimerStart(
    [TimerTrigger("0 2 * * *", RunOnStartup = true)] TimerInfo timerInfo,
    [DurableClient] DurableTaskClient client,
    FunctionContext executionContext)
    {
        // more descriptive name for the instance id
        string orchestrationId = string.Empty;
        ILogger logger = executionContext.GetLogger(nameof(EgressOrchestratorFunction_TimerStart));
        // try-finally block to ensure the instance is purged regardless of the outcome
        try
        {
            // Function triggered by a timer trigger and now starts the orchestration
            orchestrationId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(EgressOrchestrator.EgressOrchestratorStartFunction));

            logger.LogInformation("Started orchestration with ID = '{orchestrationId}'.", orchestrationId);

            // Return the instance id without purging the instance
            return orchestrationId;
        }
        catch (Exception ex)
        {
            // Log the exception details
            logger.LogError(ex, "An error occurred while starting the orchestration.");

            // Rethrow the exception to trigger the retry policy
            throw;
        }
        finally
        {
            // Purge the instance in the finally block
            await client.PurgeInstanceAsync(orchestrationId);
        }
    }

}
