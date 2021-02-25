// // -----------------------------------------------------------------------
// // <copyright file="SecurityDbContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Entity;
using Atlas.DatabaseContext;
using Atlas.Security.DatabaseContext.Migrations;
using Atlas.Security.Models;
using Atlas.Security.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Atlas.Security.DatabaseContext
{
    public class SecurityDbContext : Context
    {
        private static IConfiguration _configuration;

        // Required for entity framework migrations.
        public SecurityDbContext()
            : base(_configuration.GetConnectionString("SecurityDb"))
        {
        }

        public SecurityDbContext(IIdentityService identityService, IConfiguration configuration, ILogger log)
            : base(_configuration.GetConnectionString("SecurityDb"), identityService, log)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SecurityDbContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
            _configuration = configuration;
        }

        public virtual DbSet<Login> Logins { get; set; }
    }
}