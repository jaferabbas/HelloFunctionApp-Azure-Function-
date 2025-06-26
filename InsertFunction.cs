using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HelloFunctionApp
{
    public static class InsertFunction
    {
        [FunctionName("InsertFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("InsertFunction triggered via POST.");

            // Read and parse JSON from request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<Person>(requestBody);

            if (string.IsNullOrWhiteSpace(data?.Name))
            {
                return new BadRequestObjectResult("Name is required.");
            }

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                string insertQuery = "INSERT INTO tblAzureTest (Name) VALUES (@Name)";
                using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Name", data.Name);

                int rows = await cmd.ExecuteNonQueryAsync();

                return new OkObjectResult($"Inserted {rows} row(s) successfully.");
            }
            catch (SqlException ex)
            {
                log.LogError(ex, "SQL Error occurred.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
