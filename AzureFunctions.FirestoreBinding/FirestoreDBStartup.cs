using AzureFunctions.FirestoreBinding;
using Google.Cloud.Firestore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Text;

[assembly: WebJobsStartup(typeof(FirestoreDBStartup))]
namespace AzureFunctions.FirestoreBinding
{
    public class FirestoreDBStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<FirestoreConfigProvider>();
            var firebaseSecret = Environment.GetEnvironmentVariable("FirebaseSecret");
            if (!string.IsNullOrEmpty(firebaseSecret))
            {
                try
                {
                    firebaseSecret = Encoding.UTF8.GetString(Convert.FromBase64String(firebaseSecret));
                    var projectId = JObject.Parse(firebaseSecret).Property("project_id").Value.ToString();

                    var firestoreDb = new FirestoreDbBuilder
                    {
                        ProjectId = projectId,
                        JsonCredentials = firebaseSecret
                    }.Build();
                    builder.Services.AddSingleton(firestoreDb);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Problem with configuring firestore, please check your firebase secret format.");
                    Console.WriteLine($"Refer this link for config steps: https://github.com/VarunSaiTeja/AzureFunctions.FirestoreBinding");
                    Console.ResetColor();
                }
            }
        }
    }
}
