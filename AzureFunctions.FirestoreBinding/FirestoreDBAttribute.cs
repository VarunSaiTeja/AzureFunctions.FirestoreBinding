using Microsoft.Azure.WebJobs.Description;

namespace AzureFunctions.FirestoreBinding
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class FirestoreDBAttribute : Attribute
    {
        public FirestoreDBAttribute(string collectionPath) => CollectionPath = collectionPath;

        public string CollectionPath { get; set; }

        [AutoResolve]
        public string DocId { get; set; }

        [AutoResolve]
        internal string FirebaseSecret { get; set; }

        public string GetFirebaseSecret()
        {
            return string.IsNullOrWhiteSpace(FirebaseSecret) ? Environment.GetEnvironmentVariable(nameof(FirebaseSecret)) : FirebaseSecret;
        }
    }
}