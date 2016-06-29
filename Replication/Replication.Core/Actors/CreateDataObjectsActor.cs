﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Actors
{
    public sealed class CreateDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public CreateDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
            : base(query, storageBasedDataObjectAccessor)
        {
            _bulkRepository = bulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _dataChangesHandler = dataChangesHandler;
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var commandsToExecute = commands.OfType<ICreateDataObjectCommand>()
                                            .Where(x => x.DataObjectType == typeof(TDataObject))
                                            .Distinct()
                                            .ToArray();

            if (!commandsToExecute.Any())
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();

            var changes = DetectChanges(commandsToExecute, _equalityComparerFactory.CreateIdentityComparer<TDataObject>());

            var toCreate = changes.Difference.ToArray();

            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));
            events.AddRange(_dataChangesHandler.HandleRelates(toCreate));

            return events;
        }
    }
}