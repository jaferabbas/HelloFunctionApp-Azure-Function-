using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloFunctionApp
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("Function2 triggered and querying database.");
            var results = new List<Dictionary<string, object>>();
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return new BadRequestObjectResult("Connection string is not configured.");
                }

                using SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                string query = "SELECT TOP 10 * FROM tblAzureTest";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                results = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }
            }
            catch (Exception ex)
            {

            }
            return new OkObjectResult(results);
        }
    }
}
