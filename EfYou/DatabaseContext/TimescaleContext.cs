using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using EfYou.Extensions;
using EfYou.Model.Attributes;
using EfYou.Model.Models;
using EfYou.Security.User;
using Microsoft.Extensions.Logging;

namespace EfYou.DatabaseContext
{
    public class TimescaleContext : Context, ITimescaleContext
    {
        public TimescaleContext(string databaseName, IIdentityService identityService, ILogger log) 
            : base(databaseName, identityService, log)
        {
        }

        public TimescaleContext(string databaseName) : base(databaseName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<PostgresPropertyNamingConvention>();
            modelBuilder.Conventions.Add<PostgresEntityNamingConvention>();

            base.OnModelCreating(modelBuilder);
        }
        
        public void ConfigureTimescale()
        {
            var timescaleEntities = GetAllTimescaleHypertableEntities();
            
            SetupTimescaleExtension();

            foreach (var timescaleEntity in timescaleEntities)
            {
                MapObjectAttributesToHypertable(timescaleEntity);
            }
        }
        private void SetupTimescaleExtension()
        {
            Database.ExecuteSqlCommand("CREATE EXTENSION IF NOT EXISTS timescaledb;");
        }

        private List<Type> GetAllTimescaleHypertableEntities()
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            var entityTypes = ((EntityConnection)objectContext.Connection).GetMetadataWorkspace().GetItems<EntityType>(DataSpace.OSpace);
            var clrTypeNames = entityTypes.Select(x => x.FullName).ToList();
            var assemblies = clrTypeNames.Select(x => x.Split('.')[0]).Distinct().Select(Assembly.Load).ToList();
            var types = new List<Type>();

            foreach (var typeName in clrTypeNames)
            {
                var matchingAssembly = assemblies.FirstOrDefault(x => x.FullName.Contains(typeName.Split('.')[0]));

                if (matchingAssembly != null)
                {
                    types.Add(matchingAssembly.GetType(typeName));
                }
            }

            return types.Where(x => x.IsHypertable()).ToList();
        }

        private void MapObjectAttributesToHypertable(Type timescaleEntity)
        {
            InitializeHypertables(timescaleEntity);

            ConfigureCompression(timescaleEntity);

            ConfigureCompressionPolicies(timescaleEntity);

            ConfigureRetentionPolicies(timescaleEntity);

            CreateContinuousAggregates(timescaleEntity);

            CreateContinuousAggregationPolicies(timescaleEntity);
        }

        private void InitializeHypertables(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute) hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var hypertablePrimaryKeyAttribute = hypertableEntity.GetProperties().FirstOrDefault(x =>
                x.GetCustomAttribute(typeof(HypertablePrimaryKeyAttribute)) != null);

            if (hypertableAttribute == null || hypertablePrimaryKeyAttribute == null)
            {
                return;
            }

            var tableName = $"dbo.{hypertableAttribute.Name}";

            var sqlCommand = $"SELECT create_hypertable('{tableName}','{hypertablePrimaryKeyAttribute.Name.ToLower()}',if_not_exists=>TRUE);";

            Database.ExecuteSqlCommand(sqlCommand);
        }

        private void ConfigureCompression(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute) hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var compressionAttribute = (TimescaleCompressionAttribute)hypertableEntity.GetCustomAttribute(typeof(TimescaleCompressionAttribute));

            if (hypertableAttribute == null || compressionAttribute == null)
            {
                return;
            }

            var tableName = $"dbo.{hypertableAttribute.Name}";

            Database.ExecuteSqlCommand(
                $"ALTER TABLE {tableName} SET (timescaledb.compress, timescaledb.compress_segmentby='{compressionAttribute.SegmentBy}', timescaledb.compress_orderby='{compressionAttribute.OrderBy}');");
        }

        private void ConfigureCompressionPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute)hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var retentionCompressionPolicyAttribute = (TimescaleCompressionPolicyAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(TimescaleCompressionPolicyAttribute));

            if (hypertableAttribute == null || retentionCompressionPolicyAttribute == null)
            {
                return;
            }

            var tableName = $"dbo.{hypertableAttribute.Name}";
            
            Database.ExecuteSqlCommand(
                $"SELECT add_compression_policy('{tableName}', INTERVAL '{retentionCompressionPolicyAttribute.Interval}', if_not_exists => TRUE);");
        }

        private void ConfigureRetentionPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute)hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var retentionPolicyAttribute = (TimescaleRetentionPolicyAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(TimescaleRetentionPolicyAttribute));

            if (hypertableAttribute == null || retentionPolicyAttribute == null)
            {
                return;
            }

            var tableName = $"dbo.{hypertableAttribute.Name}";
            
            Database.ExecuteSqlCommand(
                $"SELECT add_retention_policy('{tableName}', INTERVAL '{retentionPolicyAttribute.Interval}', if_not_exists => TRUE);");
        }

        private void CreateContinuousAggregates(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute)hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var materializedViewAttribute = (MaterializedViewAttribute)hypertableEntity.GetCustomAttribute(typeof(MaterializedViewAttribute));

            var materializedViewBaseTableAttribute = (MaterializedViewBaseTableAttribute)hypertableEntity.GetCustomAttribute(
                typeof(MaterializedViewBaseTableAttribute));

            if (materializedViewAttribute == null || materializedViewBaseTableAttribute == null)
            {
                return;
            }

            var columns = new List<string>();

            var groupBys = new List<string>();

            foreach (var propertyInfo in hypertableEntity.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(TimeBucketAttribute)) != null)
                {
                    var timeBucketAttribute = (TimeBucketAttribute)propertyInfo.GetCustomAttribute(typeof(TimeBucketAttribute));

                    var aggregateColumn = (AggregateColumnAttribute)propertyInfo.GetCustomAttribute(typeof(AggregateColumnAttribute));

                    var columnName = (ColumnAttribute)propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));

                    if (aggregateColumn == null || columnName == null)
                    {
                        throw new ApplicationException(
                            $"AggregateColumn or Column is missing from view {hypertableEntity.Name}  for property {propertyInfo.Name}");
                    }

                    columns.Add($"time_bucket('{timeBucketAttribute.Interval}',{aggregateColumn.ColumnName}) as {columnName.Name}");

                    if (propertyInfo.GetCustomAttribute(typeof(GroupByAttribute)) != null)
                    {
                        groupBys.Add($"time_bucket('{timeBucketAttribute.Interval}',{aggregateColumn.ColumnName})");
                    }
                }
                else if (propertyInfo.GetCustomAttribute(typeof(AggregateTypeAttribute)) != null)
                {
                    var aggregateFunction = (AggregateTypeAttribute)propertyInfo.GetCustomAttribute(typeof(AggregateTypeAttribute));

                    var aggregateColumn = (AggregateColumnAttribute)propertyInfo.GetCustomAttribute(typeof(AggregateColumnAttribute));

                    var columnName = (ColumnAttribute)propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));

                    if (aggregateColumn == null || columnName == null)
                    {
                        throw new ApplicationException(
                            $"AggregateColumn or Column is missing from view {hypertableEntity.Name}  for property {propertyInfo.Name}");
                    }

                    columns.Add($"{aggregateFunction.AggregationFunction}({aggregateColumn.ColumnName}) as {columnName.Name}");

                    if (propertyInfo.GetCustomAttribute(typeof(GroupByAttribute)) != null)
                    {
                        groupBys.Add(columnName.Name);
                    }
                }
                else
                {
                    var columnName = (ColumnAttribute)propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));

                    if (columnName == null)
                    {
                        throw new ApplicationException(
                            $"Column is missing from view {hypertableEntity.Name} for property {propertyInfo.Name}");
                    }

                    columns.Add($"{columnName.Name}");

                    if (propertyInfo.GetCustomAttribute(typeof(GroupByAttribute)) != null)
                    {
                        groupBys.Add(columnName.Name);
                    }
                }

            }
            
            Database.ExecuteSqlCommand(
                $"CREATE MATERIALIZED VIEW IF NOT EXISTS {materializedViewAttribute.Name} WITH (timescaledb.continuous)" +
                $"AS SELECT {string.Join(',', columns)} FROM {materializedViewBaseTableAttribute.Table} GROUP BY {string.Join(',', groupBys)};");
            
        }

        private void CreateContinuousAggregationPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = (HypertableAttribute)hypertableEntity.GetCustomAttribute(typeof(HypertableAttribute), true);

            var materializedViewAttribute =
                (MaterializedViewAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(MaterializedViewAttribute));

            if (materializedViewAttribute == null)
            {
                return;
            }

            var timeBucketAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute(typeof(TimeBucketAttribute)) != null);

            if (timeBucketAttribute == null)
            {
                throw new ApplicationException(
                    $"Time bucket attribute required for {hypertableEntity.Name} policy configuration");
            }

            var timeBucketInterval = (TimeBucketAttribute)timeBucketAttribute.GetCustomAttribute(typeof(TimeBucketAttribute));
            
            Database.ExecuteSqlCommand($"SELECT add_continuous_aggregate_policy('{materializedViewAttribute.Name}', start_offset => NULL, " +
                                               $"end_offset => INTERVAL '{timeBucketInterval.Interval}', schedule_interval => INTERVAL '{timeBucketInterval.Interval}', if_not_exists => TRUE);");
        }

        
    }
    
}