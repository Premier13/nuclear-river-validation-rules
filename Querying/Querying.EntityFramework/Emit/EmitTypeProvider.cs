﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Querying.EntityFramework.Building;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.EntityFramework.Emit
{
    public sealed class EmitTypeProvider : ITypeProvider
    {
        private const string CustomCodeName = "CustomCode";

        private readonly Lazy<AssemblyBuilder> _assemblyBuilder;
        private readonly Lazy<ModuleBuilder> _moduleBuilder;
        private readonly Dictionary<IMetadataElementIdentity, Type> _typesById = new Dictionary<IMetadataElementIdentity, Type>();

        public EmitTypeProvider()
        {
            _assemblyBuilder = new Lazy<AssemblyBuilder>(() => EmitHelper.DefineAssembly(CustomCodeName));
            _moduleBuilder = new Lazy<ModuleBuilder>(() => _assemblyBuilder.Value.DefineModule(CustomCodeName));
        }

        public IReadOnlyDictionary<IMetadataElementIdentity, Type> RegisteredTypes => new ReadOnlyDictionary<IMetadataElementIdentity, Type>(_typesById);

        public Type Resolve(EntityElement entityElement)
        {
            if (entityElement == null)
            {
                throw new ArgumentNullException(nameof(entityElement));
            }

            Type type;
            if (!_typesById.TryGetValue(entityElement.Identity, out type))
            {
                _typesById.Add(entityElement.Identity, type = CreateType(entityElement));
            }

            return type;
        }

        private Type CreateType(EntityElement entityElement)
        {
            var typeName = entityElement.ResolveFullName();
            var typeBuilder = _moduleBuilder.Value.DefineType(typeName, TypeAttributes.Public);

            foreach (var propertyElement in entityElement.Properties)
            {
                var propertyName = propertyElement.ResolveName();
                var propertyType = ResolveType(propertyElement);

                typeBuilder.DefineProperty(propertyName, propertyType);
            }

            foreach (var relationElement in entityElement.Relations)
            {
                var relationTarget = relationElement.Target;
                var relationCardinality = relationElement.Cardinality;

                var propertyName = relationElement.ResolveName();
                var propertyType = CreateRelationType(Resolve(relationTarget), relationCardinality);

                typeBuilder.DefineProperty(propertyName, propertyType);
            }

            return typeBuilder.CreateType();
        }

        private Type ResolveType(EntityPropertyElement propertyElement)
        {
            var propertyType = propertyElement.PropertyType;

            Type resolvedType;
            if (propertyType.TypeKind == StructuralModelTypeKind.Primitive)
            {
                resolvedType = ConvertType(((PrimitiveTypeElement)propertyType).PrimitiveType);
            }
            else if (propertyType.TypeKind == StructuralModelTypeKind.Enum)
            {
                if (!_typesById.TryGetValue(propertyType.Identity, out resolvedType))
                {
                    _typesById.Add(propertyType.Identity, resolvedType = CreateEnum((EnumTypeElement)propertyType));
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            if (propertyElement.IsNullable && resolvedType.IsValueType)
            {
                return typeof(Nullable<>).MakeGenericType(resolvedType);
            }

            return resolvedType;
        }

        private Type CreateEnum(EnumTypeElement element)
        {
            var typeName = element.ResolveName();
            var underlyingType = ConvertType(element.UnderlyingType);

            var typeBuilder = _moduleBuilder.Value.DefineEnum(typeName, underlyingType);

            foreach (var member in element.Members)
            {
                typeBuilder.DefineLiteral(member.Key, Convert.ChangeType(member.Value, underlyingType));
            }

            return typeBuilder.CreateType();
        }

        private static Type ConvertType(ElementaryTypeKind propertyType)
        {
            switch (propertyType)
            {
                case ElementaryTypeKind.Byte:
                    return typeof(byte);
                case ElementaryTypeKind.Int16:
                case ElementaryTypeKind.Int32:
                    return typeof(int);
                case ElementaryTypeKind.Int64:
                    return typeof(long);
                case ElementaryTypeKind.Single:
                    return typeof(float);
                case ElementaryTypeKind.Double:
                    return typeof(double);
                case ElementaryTypeKind.Decimal:
                    return typeof(decimal);
                case ElementaryTypeKind.Boolean:
                    return typeof(bool);
                case ElementaryTypeKind.String:
                    return typeof(string);
                case ElementaryTypeKind.DateTimeOffset:
                    return typeof(DateTimeOffset);
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyType));
            }
        }

        private static Type CreateRelationType(Type entityType, EntityRelationCardinality cardinality)
        {
            switch (cardinality)
            {
                case EntityRelationCardinality.One:
                case EntityRelationCardinality.OptionalOne:
                    return entityType;
                case EntityRelationCardinality.Many:
                    return typeof(ICollection<>).MakeGenericType(entityType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinality));
            }
        }
    }
}