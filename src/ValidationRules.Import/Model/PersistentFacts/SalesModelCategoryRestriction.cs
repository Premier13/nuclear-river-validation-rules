using System;

namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class SalesModelCategoryRestriction
    {
        public long ProjectId { get; set; }
        public DateTime Start { get; set; }
        public long CategoryId { get; set; }

        public int SalesModel { get; set; }

        public sealed class GroupKey
        {
            public long ProjectId { get; set; }
            public DateTime Start { get; set; }

            private bool Equals(GroupKey other)
                => ProjectId == other.ProjectId && Start.Equals(other.Start);

            public override bool Equals(object obj)
                => ReferenceEquals(this, obj) || obj is GroupKey other && Equals(other);

            public override int GetHashCode()
                => HashCode.Combine(ProjectId, Start);
        }
    }
}
