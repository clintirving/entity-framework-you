// // -----------------------------------------------------------------------
// // <copyright file="SecurityDbContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Entity;
using EfYou.DatabaseContext;
using EfYou.Security.DatabaseContext.Migrations;
using EfYou.Security.Models;
using EfYou.Security.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EfYou.Security.DatabaseContext
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
            : base(configuration.GetConnectionString("SecurityDb"), identityService, log)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SecurityDbContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
            _configuration = configuration;
        }

        public virtual DbSet<Login> Logins { get; set; }
    }
}