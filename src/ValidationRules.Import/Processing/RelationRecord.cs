using System.Collections.Generic;

namespace NuClear.ValidationRules.Import.Processing
{
    public class RelationRecord
    {
        public readonly string Type;
        public readonly string RelatedType;
        public readonly long RelatedId;

        public RelationRecord(string type, string relatedType, long relatedId)
            => (Type, RelatedType, RelatedId) = (type, relatedType, relatedId);

        public sealed class EqualityComparer : IEqualityComparer<RelationRecord>
        {
            public bool Equals(RelationRecord x, RelationRecord y)
                => x != null && y != null
                    && Equals(x.Type, y.Type)
                    && Equals(x.Type, y.Type)
                    && x.RelatedId == y.RelatedId;

            public int GetHashCode(RelationRecord obj)
                => (int) obj.RelatedId;
        }
    }
}
