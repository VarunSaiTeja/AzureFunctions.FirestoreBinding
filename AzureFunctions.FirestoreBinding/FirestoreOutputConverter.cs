using Microsoft.Azure.WebJobs;

namespace AzureFunctions.FirestoreBinding
{
    class FirestoreOutputConverter<T> : IConverter<FirestoreDBAttribute, IAsyncCollector<T>>
    {
        readonly FirestoreConfigProvider _configProvider;

        public FirestoreOutputConverter(FirestoreConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public IAsyncCollector<T> Convert(FirestoreDBAttribute attribute)
        {
            var collectionRef = _configProvider.GetCollection(attribute);
            return new FirestoreAsyncCollector<T>(attribute, collectionRef);
        }
    }
}
