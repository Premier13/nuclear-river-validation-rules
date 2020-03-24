using System;
using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.Replication.Core.Actors
{
    public sealed class DeleteInMemoryDataObjectsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IdentityInMemoryChangesProvider<TDataObject> _changesProvider;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public DeleteInMemoryDataObjectsActor(
            IdentityInMemoryChangesProvider<TDataObject> changesProvider,
            IBulkRepository<TDataObject> bulkRepository,
            IDataChangesHandler<TDataObject> dataChangesHandler)
        {
            _changesProvider = changesProvider;
            _bulkRepository = bulkRepository;
            _dataChangesHandler = dataChangesHandler;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var commandsToExecute = commands.OfType<IDeleteInMemoryDataObjectCommand>()
                .Where(x => x.DataObjectType == typeof(TDataObject))
                .Distinct()
                .ToArray();

            if (!commandsToExecute.Any())
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();

            var changes = _changesProvider.GetChanges(commandsToExecute);

            // Тут Intersection, а не Complement
            // Я так понял что DeleteDataObjectsActor там условие поиска по ParentId идёт, поэтому там Complement
            // а здесь условие поиска по Id, соответственно тут Intersection
            var toDelete = changes.Intersection.ToArray();
            if (toDelete.Length != 0)
            {
                events.AddRange(_dataChangesHandler.HandleRelates(toDelete));
                events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));
                _bulkRepository.Delete(toDelete);
            }

            return events;
        }
    }
}