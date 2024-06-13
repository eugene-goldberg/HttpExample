using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Data.Odbc;

namespace HttpExample
{
    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }

        string name = "";
        [Function("HttpExample")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
             _logger.LogInformation("C# HTTP trigger function processed a request.");
            // return new OkObjectResult("Welcome to Azure Functions!");

            string newLibraryPath = "/lib:/usr/lib:/usr/lib/x86_64-linux-gnu:/home/site/wwwroot/runtimes/linux-x64/native:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/";
            Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", newLibraryPath);

            string directoryPath = "/home/site/wwwroot/runtimes/linux-x64/native"; // Specify the directory to check
            string libraryFileName = "libodbc.so.2"; // Specify the library file name

            string filePath = Path.Combine(directoryPath, libraryFileName);

           // string directoryPath = "/home/site/wwwroot/runtimes/linux-x64/native"; // Specify the directory to check
            string[] libraryFileNames = { "libodbc.so.2", "libltdl.so.7" }; // Specify the library file names

            foreach (string fileName in libraryFileNames)
            {

                if (File.Exists(filePath))
                {
                    Console.WriteLine($"The library {fileName} is present in the directory {directoryPath}.");
                }
                else
                {
                    Console.WriteLine($"The library {fileName} is NOT present in the directory {directoryPath}.");
                }
            }

            string sourcePath = "/home/site/wwwroot/libltdl.so.7";
            string destinationPath = "/home/site/wwwroot/runtimes/linux-x64/native/libltdl.so.7";

            try
            {
                // Ensure the destination directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                // Copy the file
                File.Copy(sourcePath, destinationPath, overwrite: true);

                _logger.LogInformation($"File copied successfully from {sourcePath} to {destinationPath}");

                // Verify the copy
                if (File.Exists(destinationPath))
                {
                    Console.WriteLine($"The library {Path.GetFileName(destinationPath)} is present in the directory {Path.GetDirectoryName(destinationPath)}.");
                }
                else
                {
                    Console.WriteLine($"The library {Path.GetFileName(destinationPath)} is NOT present in the directory {Path.GetDirectoryName(destinationPath)}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while copying the file: {ex.Message}");
            }
            
            _logger.LogInformation("The value of LD_LIBRARY_PATH:  " + Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"));

             
            _logger.LogInformation("DB Warehouse Saying hello to {name}.", name);

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
                            _logger.LogError($"An error occurred: {ex.Message}");
                            name = ex.Message;
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                name = ex.Message;
            }
            name=records.Count.ToString();
             return new OkObjectResult("Number of records retrieved:  " +  name + "\n\n");
        }

    }
}
