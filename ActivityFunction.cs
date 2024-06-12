using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Data.Odbc;

namespace DurableOrchestratorDotnet8Linux
{
    public static class ActivityFunction
    {
        [Function(nameof(ActivityFunction))]
        public static string RetrieveDatabricksData([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("DB Warehouse Saying hello to {name}.", name);

            var connectionString = "DSN=DatabricksWarehouse";
            var query = "SELECT c.customer_id, c.customer_name, c.customer_email, o.order_id, o.product, o.amount" +
                     " FROM Customers c" +
                     " JOIN Orders o ON c.customer_id = o.customer_id";

            List<string> records = new List<string>();

            try {
                    using (OdbcConnection connection = new OdbcConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            using (OdbcCommand command = new OdbcCommand(query, connection))
                            {
                                using (OdbcDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string record = reader.GetString(0) + " " + reader.GetString(1) +
                                                        " " + reader.GetString(2) + " " + reader.GetString(3) +
                                                        " " + reader.GetString(4) + " " + reader.GetString(5);
                                        records.Add(record);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"An error occurred: {ex.Message}");
                            name = ex.Message;
                        }
                    }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                name = ex.Message;
            }
            return $"Hello {name}!";
        }
    }
}
