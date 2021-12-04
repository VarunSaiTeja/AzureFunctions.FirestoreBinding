using AzureFunctions.FirestoreBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo
{
    public static class EmployeeFunctions
    {
        [FunctionName("AddBulk")]
        public static async Task<IActionResult> AddBulk(
            [HttpTrigger("post")] List<Employee> employees,
            [FirestoreDB("employees")] IAsyncCollector<Employee> outEmployees)
        {
            foreach (var emp in employees)
            {
                await outEmployees.AddAsync(emp);
            }
            return new OkObjectResult("Done");
        }

        [FunctionName("AddSingle")]
        public static IActionResult AddSingle(
            [HttpTrigger("post")] Employee employee,
            [FirestoreDB("employees")] out Employee outEmployee)
        {
            outEmployee = employee;

            return new OkObjectResult(employee);
        }

        [FunctionName("GetEmployee")]
        public static IActionResult GetEmployee(
            [HttpTrigger("get", Route = "GetEmployee/{empId}")] HttpRequest req,
            [FirestoreDB("employees", DocId = "{empId}")] Employee employee)
        {
            return employee == null ? new NotFoundResult() : new OkObjectResult(employee);
        }
    }
}
