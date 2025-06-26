using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HelloFunctionApp
{
    public static class ProcessEmployeeFunction
    {
        [FunctionName("ProcessEmployeeFunction")]
        public static void Run(
            [TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            while (FakeQueue.Employees.Count > 0)
            {
                var employee = FakeQueue.Employees.Dequeue();
                log.LogInformation($"[Processed] Employee: {employee.Name} | Email: {employee.Email}");
            }
        }
    }

}
