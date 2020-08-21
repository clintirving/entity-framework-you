// // -----------------------------------------------------------------------
// // <copyright file="EntityExtensions.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using EfYouCore.Extensions;
using DefaultValueAttribute = EfYouCore.Model.Attributes.DefaultValueAttribute;

namespace EfYouCore.Utilities
{
    public static class EntityExtensions
    {
        public static T CreateEntityWithDefaultValues<T>() where T : new()
        {
            var entity = new T();

            WalkGraphAndSetDefaultValues(entity, new List<object>());

            return entity;
        }

        public static void SetDefaultValuesOnEntities<T>(List<T> entities)
        {
            foreach (var entity in entities)
            {
                WalkGraphAndSetDefaultValues(entity, new List<object>());
            }
        }

        private static void WalkGraphAndSetDefaultValues(object entity, ICollection<object> entitiesDone)
        {
            var enumerableEntity = entity as IEnumerable;

            if (enumerableEntity != null)
            {
                foreach (var thingItem in enumerableEntity)
                {
                    WalkGraphAndSetDefaultValues(thingItem, entitiesDone);
                }
            }
            else
            {
                if (!entitiesDone.Contains(entity))
                {
                    entitiesDone.Add(entity);

                    SetDefaultValues(entity);

                    var entityReferencePropertyInfos = entity.GetType().GetEntityReferenceProperties();

                    foreach (var propertyInfo in entityReferencePropertyInfos)
                    {
                        var entityReferenceProperty = propertyInfo.GetValue(entity);

                        if (entityReferenceProperty != null)
                        {
                            WalkGraphAndSetDefaultValues(entityReferenceProperty, entitiesDone);
                        }
                    }
                }
            }
        }

        private static void SetDefaultValues(object entity)
        {
            var dbPropertyInfos = entity.GetType().GetDbProperties();

            foreach (var propertyInfo in dbPropertyInfos)
            {
                var defaultValueAttribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValueAttribute != null && propertyInfo.GetValue(entity) == null)
                {
                    if (propertyInfo.PropertyType == typeof(DateTime?) && defaultValueAttribute.UtcNow)
                    {
                        propertyInfo.SetValue(entity, DateTime.UtcNow, null);
                    }
                    else if (defaultValueAttribute.Value != null)
                    {
                        var typeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                        var value = typeConverter.ConvertFromInvariantString(defaultValueAttribute.Value);

                        propertyInfo.SetValue(entity, value, null);
                    }
                }
            }
        }
    }
}