# AzureFunctions.FirestoreBinding
<a href="https://www.nuget.org/packages/AzureFunctions.FirestoreBinding"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/AzureFunctions.FirestoreBinding"></a>
<a href="https://www.nuget.org/packages/AzureFunctions.FirestoreBinding"><img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/AzureFunctions.FirestoreBinding"></a>

## Adds support for input/output binding to your Azure functions with Firestore database.

#### Configuration Steps: 
1. Go to your firebase service account settings https://console.firebase.google.com/u/0/project/{your-project-id}/settings/serviceaccounts/adminsdk
2. Create and download the private key, a JSON file will be downloaded which having all configuration to connect to firestore.
3. Open the file and copy all the content inside the file.
4. Now convert the JSON inside the file into Base64 Encoded format string(Tip: Use any online site/local apps if you trust those).
5. The encoded string will be considered as FirebaseSecret by this library.
6. Now go to your local.settings.json and inside values object configure the FireabaseSecret. The key is same i.e FirebaseSecret & value is Base64 Encoded string.


#### Input Binding Document:
```csharp
[FunctionName("GetEmployee")]
public static IActionResult GetEmployee(
  [HttpTrigger("get", Route = "GetEmployee/{empId}")] HttpRequest req, 
  [FirestoreDB("employees", DocId = "{empId}")] Employee employee)
{
    return employee == null ? new NotFoundResult() : new OkObjectResult(employee);
}
```

#### Input Binding collection:
```csharp
[FunctionName("GetSeniorEmployees")]
public static async Task<IActionResult> GetSeniorEmployees(
  [HttpTrigger("get", Route = "GetSeniorEmployees")] HttpRequest req,
  [FirestoreDB("employees")] CollectionReference collection)
{
    var employees = await collection.WhereGreaterThanOrEqualTo("Age", "40").GetDocumentsAsync<Employee>();
    return new OkObjectResult(employees);
}
```


#### Output Binding single document:
```csharp
[FunctionName("AddSingle")]
public static IActionResult AddSingle(
  [HttpTrigger("post")] Employee employee,
  [FirestoreDB("employees")] out Employee outEmployee)
{
    outEmployee = employee;

    return new OkObjectResult(employee);
}
```

#### Output Binding multiple document:
```csharp
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
```
