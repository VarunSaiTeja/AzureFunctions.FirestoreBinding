using AzureFunctions.FirestoreBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(FirestoreDBStartup))]
namespace AzureFunctions.FirestoreBinding
{
    public class FirestoreDBStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<FirestoreConfigProvider>();
        }
    }
}
