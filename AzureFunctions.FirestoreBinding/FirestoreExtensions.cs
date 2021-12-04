using Google.Cloud.Firestore;

namespace AzureFunctions.FirestoreBinding
{
    public static class FirestoreExtensions
    {
        public static async Task<List<T>> GetDocumentsAsync<T>(this Query query)
        {
            var snapshot = await query.GetSnapshotAsync();
            return snapshot?.Count > 0 ? snapshot.Documents.Select(x => x.ConvertTo<T>()).ToList() : null;
        }

        public static async Task<T> GetDocumentAsync<T>(this Query query)
        {
            var snapshot = await query.Limit(1).GetSnapshotAsync();
            return snapshot?.Count > 0 ? snapshot.Documents.SingleOrDefault().ConvertTo<T>() : default;
        }
    }
}
