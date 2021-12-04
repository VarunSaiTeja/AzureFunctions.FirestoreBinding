using Microsoft.Azure.WebJobs;

namespace AzureFunctions.FirestoreBinding
{
    class FirestoreDBInputConverter<T> : IAsyncConverter<FirestoreDBAttribute, T>
    {
        readonly FirestoreDBConfigProvider _configProvider;

        public FirestoreDBInputConverter(FirestoreDBConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public async Task<T> ConvertAsync(FirestoreDBAttribute attribute, CancellationToken cancellationToken)
        {
            return await _configProvider.GetDocument<T>(attribute);
        }
    }
}
