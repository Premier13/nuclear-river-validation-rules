﻿using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class DeleteDataObjectCommand : IDeleteDataObjectCommand
    {
        public DeleteDataObjectCommand(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DeleteDataObjectCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DataObjectType?.GetHashCode() ?? 0) * 397) ^ DataObjectId.GetHashCode();
            }
        }

        private bool Equals(DeleteDataObjectCommand other)
        {
            return DataObjectType == other.DataObjectType && DataObjectId == other.DataObjectId;
        }
    }
}