using Google.Cloud.Firestore;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Text;

namespace AzureFunctions.FirestoreBinding
{
    public class FirestoreContext
    {
        private FirestoreDb firestoreDb;
        private ConcurrentDictionary<string, CollectionReference> CollecttionCache { get; } = new ConcurrentDictionary<string, CollectionReference>();

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

            string cacheKey = attribute.CollectionPath;
            var collection = CollecttionCache.GetOrAdd(cacheKey, (c) => firestoreDb.Collection(attribute.CollectionPath));
            return collection;
        }
    }
}
