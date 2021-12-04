using Google.Cloud.Firestore;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Text;

namespace AzureFunctions.FirestoreBinding
{

    [Extension("FirestoreDB")]
    public class FirestoreConfigProvider : IExtensionConfigProvider
    {
        private FirestoreDb firestoreDb;

        private ConcurrentDictionary<string, CollectionReference> CollecttionCache { get; } = new ConcurrentDictionary<string, CollectionReference>();

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var rule = context.AddBindingRule<FirestoreDBAttribute>();
            rule.AddValidator(ValidateConnection);
            rule.BindToCollector<OpenType.Poco>(typeof(FirestoreOutputConverter<>), this);
            rule.BindToInput<OpenType.Poco>(typeof(FirestoreInputConverter<>), this);
        }

        public async Task<T> GetDocument<T>(FirestoreDBAttribute attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute.DocId))
            {
                if (typeof(T) == typeof(CollectionReference))
                    return (T)Convert.ChangeType(GetCollection(attribute), typeof(T));

                string attributeProperty = $"{nameof(FirestoreDBAttribute)}.{nameof(FirestoreDBAttribute.DocId)}";
                throw new InvalidOperationException(
                    $"The Firestore collection must be set via the {attributeProperty} property.");
            }

            var collection = GetCollection(attribute);
            var doc = collection.Document(attribute.DocId);
            var snap = await doc.GetSnapshotAsync();
            return snap.ConvertTo<T>();
        }

        void ValidateConnection(FirestoreDBAttribute attribute, Type paramType)
        {
            if (string.IsNullOrEmpty(attribute.GetFirebaseSecret()))
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
                var firebaseSecret = Encoding.UTF8.GetString(Convert.FromBase64String(attribute.GetFirebaseSecret()));
                var projectId = JObject.Parse(firebaseSecret).Property("project_id").Value.ToString();

                firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    JsonCredentials = firebaseSecret
                }.Build();
            }

            string cacheKey = $"{firestoreDb.ProjectId}|{attribute.CollectionPath}";
            var collection = CollecttionCache.GetOrAdd(cacheKey, (c) => firestoreDb.Collection(attribute.CollectionPath));
            return collection;
        }
    }
}
