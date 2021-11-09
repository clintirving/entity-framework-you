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


        public void ConfigureTimescale()
        {
            InitializeHypertables();

            ConfigureCompression();

            ConfigureCompressionPolicies();

            ConfigureRetentionPolicies();

            CreateContinuousAggregates();

            CreateContinuousAggregationPolicies();
        }

        private void InitializeHypertables()
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureCompression()
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureCompressionPolicies()
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureRetentionPolicies()
        {
            throw new System.NotImplementedException();
        }

        private void CreateContinuousAggregates()
        {
            throw new System.NotImplementedException();
        }

        private void CreateContinuousAggregationPolicies()
        {
            throw new System.NotImplementedException();
        }
    }
}