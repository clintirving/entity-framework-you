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
        }

        public void ConfigureTimescale(DbModelBuilder modelBuilder)
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

        private void InitializeHypertables(DbModelBuilder modelBuilder, Type hypertableEntity)
        {
            var hypertablePrimaryKeyAttribute = hypertableEntity.GetProperties().FirstOrDefault(x =>
                x.GetCustomAttribute(typeof(HypertablePrimaryKeyAttribute)) != null);

            if (hypertablePrimaryKeyAttribute == null)
            {
                return;
            }

            var tableName = hypertablePrimaryKeyAttribute.Name;

            var columnAttribute = (ColumnAttribute)hypertablePrimaryKeyAttribute.GetCustomAttribute(typeof(ColumnAttribute));

            var hypertablePrimaryKeyName =
                columnAttribute != null ? columnAttribute.Name : hypertablePrimaryKeyAttribute.Name;

            Console.WriteLine($"SELECT create_hypertable('{tableName}','{hypertablePrimaryKeyName}',if_not_exists=>TRUE);");

            DatabaseAccessor.ExecuteSqlCommand($"SELECT create_hypertable('{tableName}','{hypertablePrimaryKeyName}',if_not_exists=>TRUE);");
        }

        private void ConfigureCompression(Type hypertableEntity)
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureCompressionPolicies(Type hypertableEntity)
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureRetentionPolicies(Type hypertableEntity)
        {
            throw new System.NotImplementedException();
        }

        private void CreateContinuousAggregates(Type hypertableEntity)
        {
            throw new System.NotImplementedException();
        }

        private void CreateContinuousAggregationPolicies(Type hypertableEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}