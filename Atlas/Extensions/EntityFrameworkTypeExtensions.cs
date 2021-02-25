// // -----------------------------------------------------------------------
// // <copyright file="EntityFrameworkTypeExtensions.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Atlas.DatabaseContext;

namespace Atlas.Extensions
{
    public static class EntityFrameworkTypeExtensions
    {
        public static List<PropertyInfo> GetDbProperties(this Type type)
        {
            // Ignore virtual properties and not mapped properties.
            return type.GetProperties()
                .Where(
                    x =>
                        (x.GetAccessors().First().IsFinal ||
                         !x.GetAccessors().First().IsFinal && !x.GetAccessors().First().IsVirtual) &&
                        !Attribute.IsDefined(x, typeof(NotMappedAttribute))).ToList();
        }

        public static List<PropertyInfo> GetEntityReferenceProperties(this Type type)
        {
            // Return only Virtual Class properties
            return type.GetProperties().Where(x => x.GetAccessors().First().IsVirtual && x.PropertyType.IsClass).ToList();
        }

        public static string GetTableName(this Type type, IContext context)
        {
            var entityName = type.Name;

            var objectContext = ((IObjectContextAdapter) context).ObjectContext;

            var storageMetadata = objectContext.MetadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);

            var databaseName = context.DatabaseAccessor.DatabaseName;

            foreach (var ecm in storageMetadata)
            {
                EntitySet entitySet;
                if (ecm.StoreEntityContainer.TryGetEntitySetByName(entityName, true, out entitySet))
                {
                    return string.Format("[{0}].[{1}].[{2}]", databaseName, entitySet.Schema, entitySet.Table);
                }
            }

            throw new ApplicationException("Could not find tablename for type: " + entityName + " in database " + databaseName);
        }

        public static List<PropertyInfo> GetPrimaryKeyProperties(this Type entityType)
        {
            // Ignore virtual properties and not mapped properties.
            return entityType.GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(KeyAttribute))).ToList();
        }

        public static PropertyInfo GetPrimaryKeyProperty(this Type entityType)
        {
            var primaryKeyProperties = entityType.GetPrimaryKeyProperties();

            if (primaryKeyProperties.Count != 1)
            {
                throw new ApplicationException(string.Format(
                    "Primary key for type {0} must consist of only a single column - GetPrimaryKeyProperty will not work for this entity type",
                    entityType.FullName));
            }

            return primaryKeyProperties.Single();
        }

        public static List<long> GetIdsFromEntities<T>(this List<T> entities)
        {
            return entities.Select(GetIdFromEntity).ToList();
        }

        public static long GetIdFromEntity<T>(this T entity)
        {
            return (long) Convert.ChangeType(entity.GetType().GetPrimaryKeyProperty().GetValue(entity), typeof(long));
        }
    }
}