﻿using System.Collections.Generic;

namespace NuClear.Replication.Core.DataObjects
{
    public interface IBulkRepository<in TDataObject>
    {
        void Create(IEnumerable<TDataObject> objects);
        void Update(IEnumerable<TDataObject> objects);
        void Delete(IEnumerable<TDataObject> objects);
    }
}