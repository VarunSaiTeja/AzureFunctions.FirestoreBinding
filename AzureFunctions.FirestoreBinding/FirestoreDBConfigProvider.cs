using Google.Cloud.Firestore;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using System.Collections.Concurrent;
using System.Text;

namespace AzureFunctions.FirestoreBinding
{

    [Extension("FirestoreDB")]
    public class FirestoreDBConfigProvider : IExtensionConfigProvider
    {
        private FirestoreDb firestoreDb;

        private ConcurrentDictionary<string, CollectionReference> CollecttionCache { get; } = new ConcurrentDictionary<string, CollectionReference>();

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var rule = context.AddBindingRule<FirestoreDBAttribute>();
            rule.AddValidator(ValidateConnection);
            rule.BindToCollector<OpenType.Poco>(typeof(FirestoreDBOutputConverter<>), this);
            rule.BindToInput<OpenType.Poco>(typeof(FirestoreDBInputConverter<>), this);
        }

        public async Task<T> GetDocument<T>(FirestoreDBAttribute attribute)
        {
            var collection = GetCollection(attribute);
            var doc = collection.Document(attribute.DocId);
            var snap = await doc.GetSnapshotAsync();
            return snap.ConvertTo<T>();
        }

        void ValidateConnection(FirestoreDBAttribute attribute, Type paramType)
        {
            if (string.IsNullOrEmpty(attribute.FirebaseProjectId))
            {
                string attributeProperty = $"{nameof(FirestoreDBAttribute)}.{nameof(FirestoreDBAttribute.FirebaseProjectId)}";
                throw new InvalidOperationException(
                    $"The Firestore Project Id must be set via the {attributeProperty} property.");
            }

            if (string.IsNullOrEmpty(attribute.FirebaseSecret))
            {
                string attributeProperty = $"{nameof(FirestoreDBAttribute)}.{nameof(FirestoreDBAttribute.FirebaseSecret)}";
                throw new InvalidOperationException(
                    $"The Firestore Secret must be set via the {attributeProperty} property.");
            }

            if (string.IsNullOrEmpty(attribute.CollectionPath))
            {
                string attributeProperty = $"{nameof(FirestoreDBAttribute)}.{nameof(FirestoreDBAttribute.CollectionPath)}";
                throw new InvalidOperationException(
                    $"The Firestore collection must be set via the {attributeProperty} property.");
            }
        }

        public CollectionReference GetCollection(FirestoreDBAttribute attribute)
        {

            if (firestoreDb == null)
            {
                var firebaseSecret = Encoding.UTF8.GetString(Convert.FromBase64String(attribute.FirebaseSecret));
                firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = attribute.FirebaseProjectId,
                    JsonCredentials = firebaseSecret
                }.Build();
            }

            string cacheKey = $"{attribute.FirebaseProjectId}|{attribute.CollectionPath}";
            var collection = CollecttionCache.GetOrAdd(cacheKey, (c) => firestoreDb.Collection(attribute.CollectionPath));
            return collection;
        }
    }
}
