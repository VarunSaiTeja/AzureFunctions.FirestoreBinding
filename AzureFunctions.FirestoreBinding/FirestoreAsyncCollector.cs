using Google.Cloud.Firestore;
using Microsoft.Azure.WebJobs;

namespace AzureFunctions.FirestoreBinding
{
    public class FirestoreAsyncCollector<T> : IAsyncCollector<T>
    {
        private readonly FirestoreDBAttribute _attribute;
        private readonly CollectionReference _collection;

        public FirestoreAsyncCollector(FirestoreDBAttribute attribute, CollectionReference collection)
        {
            _attribute = attribute;
            _collection = collection;
        }

        public async Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            var docIdProperty = typeof(T).GetProperties().SingleOrDefault(x => Attribute.IsDefined(x, typeof(FirestoreDocumentIdAttribute)));
            var docId = docIdProperty?.GetValue(item)?.ToString();

            if (!string.IsNullOrWhiteSpace(docId))
                await _collection.Document(docId).SetAsync(item, cancellationToken: cancellationToken);
            else
            {
                var response = await _collection.AddAsync(item, cancellationToken);
                if (docIdProperty is not null)
                {
                    docIdProperty.SetValue(item, response.Id);
                }
            }
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }
}
