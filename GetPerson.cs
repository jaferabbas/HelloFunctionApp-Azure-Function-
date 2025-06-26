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
    public static class GetPerson
    {
        [FunctionName("GetPerson")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string id = req.Query["id"];
            string name = req.Query["name"];

            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name))
            {
                return new BadRequestObjectResult("Please provide 'id' or 'name' as query parameter.");
            }

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            var results = new List<Dictionary<string, object>>();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                string query = "SELECT ID, Name FROM tblAzureTest WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(id))
                {
                    query += " AND ID = @Id";
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query += " AND Name = @Name";
                }

                using SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    cmd.Parameters.AddWithValue("@Id", int.Parse(id));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                }

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>
                    {
                        ["ID"] = reader["ID"],
                        ["Name"] = reader["Name"]
                    };
                    results.Add(row);
                }

                return new OkObjectResult(results);
            }
            catch (SqlException ex)
            {
                log.LogError(ex, "SQL Error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
