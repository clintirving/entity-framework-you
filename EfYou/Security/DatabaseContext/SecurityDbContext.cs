// // -----------------------------------------------------------------------
// // <copyright file="SecurityDbContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Entity;
using Common.Logging;
using EfYouCore.DatabaseContext;
using EfYouCore.Security.DatabaseContext.Migrations;
using EfYouCore.Security.Models;
using EfYouCore.Security.User;

namespace EfYouCore.Security.DatabaseContext
{
    public class SecurityDbContext : Context
    {
        // Required for entity framework migrations.
        public SecurityDbContext()
            : base("SecurityDb")
        {
        }

        public SecurityDbContext(IIdentityService identityService, ILog log)
            : base("SecurityDb", identityService, log)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SecurityDbContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Login> Logins { get; set; }
    }
}