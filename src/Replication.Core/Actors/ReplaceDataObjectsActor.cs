﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.Replication.Core.Actors
{
    public sealed class ReplaceDataObjectsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly InMemoryEntityChangesProvider<TDataObject> _changesProvider;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public ReplaceDataObjectsActor(
            InMemoryEntityChangesProvider<TDataObject> changesProvider,
            IBulkRepository<TDataObject> bulkRepository,
            IDataChangesHandler<TDataObject> dataChangesHandler)
        {
            _changesProvider = changesProvider;
            _bulkRepository = bulkRepository;
            _dataChangesHandler = dataChangesHandler;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var commandsToExecute = commands.OfType<IReplaceDataObjectCommand>()
                                            .Where(x => x.DataObjectType == typeof(TDataObject))
                                            .ToHashSet();
            if (commandsToExecute.Count == 0)
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();
            
            using var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero });
            
            var changes = _changesProvider.GetChanges(commandsToExecute);

            var toDelete = changes.Complement.ToArray();
            if (toDelete.Length != 0)
            {
                events.AddRange(_dataChangesHandler.HandleRelates(toDelete));
                events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));
                _bulkRepository.Delete(toDelete);
            }

            var toCreate = changes.Difference.ToArray();
            if (toCreate.Length != 0)
            {
                _bulkRepository.Create(toCreate);
                events.AddRange(_dataChangesHandler.HandleCreates(toCreate));
                events.AddRange(_dataChangesHandler.HandleRelates(toCreate));
            }

            var toUpdate = changes.Intersection.ToArray();
            if (toUpdate.Length != 0)
            {
                events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
                _bulkRepository.Update(toUpdate);
                events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
                events.AddRange(_dataChangesHandler.HandleUpdates(toUpdate));
            }

            transaction.Complete();

            return events;
        }
    }
}