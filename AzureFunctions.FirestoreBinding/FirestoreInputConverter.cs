using Microsoft.Azure.WebJobs;

namespace AzureFunctions.FirestoreBinding
{
    class FirestoreInputConverter<T> : IAsyncConverter<FirestoreDBAttribute, T>
    {
        readonly FirestoreConfigProvider _configProvider;

        public FirestoreInputConverter(FirestoreConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public async Task<T> ConvertAsync(FirestoreDBAttribute attribute, CancellationToken cancellationToken)
        {
            return await _configProvider.GetDocument<T>(attribute);
        }
    }
}
