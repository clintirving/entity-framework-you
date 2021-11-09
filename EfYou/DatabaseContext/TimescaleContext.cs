using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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
        private readonly List<Type> _timescaleEntities = new List<Type>();

        private List<Type> TimescaleEntities => _timescaleEntities;

        public TimescaleContext(string databaseName, IIdentityService identityService, ILogger log) 
            : base(databaseName, identityService, log)
        {
        }

        public TimescaleContext(string databaseName) : base(databaseName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entities = modelBuilder.GetAllDatabaseEntities();

            TimescaleEntities.AddRange(entities.Where(x => x.GetCustomAttributes(typeof(HypertableAttribute), true).FirstOrDefault() != null).ToList());

            ConfigureTimescale();
        }

        public void ConfigureTimescale()
        {
            foreach (var timescaleEntity in TimescaleEntities)
            {
                InitializeHypertables(timescaleEntity);

                ConfigureCompression(timescaleEntity);

                ConfigureCompressionPolicies(timescaleEntity);

                ConfigureRetentionPolicies(timescaleEntity);

                CreateContinuousAggregates(timescaleEntity);

                CreateContinuousAggregationPolicies(timescaleEntity);
            }
            
        }

        private void InitializeHypertables(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);

            var hypertablePrimaryKeyAttribute = hypertableEntity.GetProperties().FirstOrDefault(x =>
                x.GetCustomAttribute(typeof(HypertablePrimaryKeyAttribute)) != null);

            if (hypertablePrimaryKeyAttribute == null)
            {
                return;
            }

            var tableName = hypertableAttribute.Name;

            var columnAttribute = (ColumnAttribute)hypertablePrimaryKeyAttribute.GetCustomAttribute(typeof(ColumnAttribute));

            var hypertablePrimaryKeyName =
                columnAttribute != null ? columnAttribute.Name : hypertablePrimaryKeyAttribute.Name;

            Console.WriteLine($"SELECT create_hypertable('{tableName}','{hypertablePrimaryKeyName}',if_not_exists=>TRUE);");

            DatabaseAccessor.ExecuteSqlCommand($"SELECT create_hypertable('{tableName}','{hypertablePrimaryKeyName}',if_not_exists=>TRUE);");
        }

        private void ConfigureCompression(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);
            
            var compressionAttribute =
                (TimescaleCompressionAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(TimescaleCompressionAttribute));

            if (compressionAttribute == null)
            {
                return;
            }

            var tableName = hypertableAttribute.Name;

            DatabaseAccessor.ExecuteSqlCommand(
                $"ALTER TABLE {tableName} SET (timescaledb.compress, timescaledb.compress_segmentby='{compressionAttribute.SegmentBy}', timescaledb.compress_orderby='{compressionAttribute.OrderBy}');");
        }

        private void ConfigureCompressionPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);

            var retentionPolicyAttribute =
                (TimescaleCompressionPolicyAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(TimescaleCompressionPolicyAttribute));

            if (retentionPolicyAttribute == null)
            {
                return;
            }

            var tableName = hypertableAttribute.Name;

            Console.WriteLine($"SELECT add_retention_policy('{tableName}', INTERVAL '{retentionPolicyAttribute.Interval}', if_not_exists => TRUE);");

            DatabaseAccessor.ExecuteSqlCommand(
                $"SELECT add_compression_policy('{tableName}', INTERVAL '{retentionPolicyAttribute.Interval}', if_not_exists => TRUE);");
        }

        private void ConfigureRetentionPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);
            
            var retentionPolicyAttribute =
                (TimescaleRetentionPolicyAttribute)hypertableEntity.GetCustomAttribute(
                    typeof(TimescaleRetentionPolicyAttribute));

            if (retentionPolicyAttribute == null)
            {
                return;
            }

            var tableName = hypertableAttribute.Name;

            Console.WriteLine($"SELECT add_retention_policy('{tableName}', INTERVAL '{retentionPolicyAttribute.Interval}', if_not_exists => TRUE);");

            DatabaseAccessor.ExecuteSqlCommand(
                $"SELECT add_retention_policy('{tableName}', INTERVAL '{retentionPolicyAttribute.Interval}', if_not_exists => TRUE);");
        }

        private void CreateContinuousAggregates(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);
            throw new System.NotImplementedException();
        }

        private void CreateContinuousAggregationPolicies(Type hypertableEntity)
        {
            var hypertableAttribute = hypertableEntity.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(HypertableAttribute)) != null);
            throw new System.NotImplementedException();
        }
    }
}