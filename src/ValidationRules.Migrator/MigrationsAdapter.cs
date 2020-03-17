using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NuClear.ValidationRules.Migrator
{
    internal sealed class MigrationsAdapter
    {
        private readonly Assembly _assembly;

        public MigrationsAdapter(Assembly assembly)
            => _assembly = assembly;

        public IReadOnlyCollection<IMigration> GetMigrations()
            => _assembly.GetTypes()
                .Where(IsMigration)
                .Select(Activator.CreateInstance)
                .Cast<IMigration>()
                .ToList();

        private bool IsMigration(Type type)
            => type.IsClass
                && !type.IsAbstract
                && typeof(IMigration).IsAssignableFrom(type)
                && type.GetConstructor(Array.Empty<Type>()) != null;
    }
}