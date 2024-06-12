using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.IO;

namespace DurableOrchestratorDotnet8Linux
{
    public static class Orchestrator
    {
        [Function(nameof(Orchestrator))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(Orchestrator));
            logger.LogInformation("Invoking activity function.");
            var outputs = new List<string>();
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Databricks"));
        
            return outputs;
        }

        
    }
}
