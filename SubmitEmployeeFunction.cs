using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HelloFunctionApp
{
    public static class SubmitEmployeeFunction
    {
        [FunctionName("SubmitEmployeeFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(requestBody);

            if (employee == null || string.IsNullOrWhiteSpace(employee.Name))
            {
                return new BadRequestObjectResult("Invalid employee data.");
            }

            FakeQueue.Employees.Enqueue(employee); // Simulate sending to a Service Bus queue

            log.LogInformation($"Enqueued employee: {employee.Name}");
            return new OkObjectResult($"Employee {employee.Name} submitted.");
        }
    }

}
