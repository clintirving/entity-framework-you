using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EfYou.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static List<Type> GetAllDatabaseEntities(this DbModelBuilder modelBuilder)
        {
            var modelConfiguration = modelBuilder.GetType().GetProperty("ModelConfiguration",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            var configuration = modelConfiguration?.GetValue(modelBuilder);

            var configuredTypes = configuration?.GetType().GetProperty("ConfiguredTypes",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            var entities = (IEnumerable<Type>) configuredTypes?.GetValue(configuredTypes, null);

            return entities?.ToList();
        }
    }
}