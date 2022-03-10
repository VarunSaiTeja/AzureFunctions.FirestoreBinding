using Google.Cloud.Firestore;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using System.Collections.Concurrent;

namespace AzureFunctions.FirestoreBinding
{

    [Extension("FirestoreDB")]
    public class FirestoreConfigProvider : IExtensionConfigProvider
    {
        internal readonly FirestoreContext context;

        public FirestoreConfigProvider(FirestoreContext context)
        {
            this.context = context;
        }

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
    }
}
