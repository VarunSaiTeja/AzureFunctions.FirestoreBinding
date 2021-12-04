using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Demo
{
    [FirestoreData]
    public class Employee
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Age { get; set; }

        [FirestoreProperty]
        public string Department { get; set; }

        [FirestoreProperty]
        public List<Skill> Skills { get; set; }

        [FirestoreProperty, ServerTimestamp]
        public DateTimeOffset LastUpdated { get; set; }
    }

    [FirestoreData]
    public class Skill
    {
        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public SkillLevel Level { get; set; }
    }

    [FirestoreData(ConverterType = typeof(FirestoreEnumNameConverter<SkillLevel>))]
    public enum SkillLevel
    {
        Beginner,
        Intermediate,
        Pro
    }

    public class GuidConverter : IFirestoreConverter<Guid>
    {
        public object ToFirestore(Guid value) => value.ToString("N");

        public Guid FromFirestore(object value)
        {
            return value switch
            {
                string guid => Guid.ParseExact(guid, "N"),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new ArgumentException($"Unexpected data: {value.GetType()}"),
            };
        }
    }
}
