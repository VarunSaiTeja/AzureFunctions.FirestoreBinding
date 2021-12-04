using Microsoft.Azure.WebJobs;

namespace AzureFunctions.FirestoreBinding
{
    class FirestoreDBOutputConverter<T> : IConverter<FirestoreDBAttribute, IAsyncCollector<T>>
    {
        readonly FirestoreDBConfigProvider _configProvider;

        public FirestoreDBOutputConverter(FirestoreDBConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public IAsyncCollector<T> Convert(FirestoreDBAttribute attribute)
        {
            var collectionRef = _configProvider.GetCollection(attribute);
            return new FirestoreDBBindingAsyncCollector<T>(attribute, collectionRef);
        }
    }
}
