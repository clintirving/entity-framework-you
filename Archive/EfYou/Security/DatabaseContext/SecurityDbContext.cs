﻿// // -----------------------------------------------------------------------
// // <copyright file="SecurityDbContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Entity;
using Common.Logging;
using EfYou.DatabaseContext;
using EfYou.Security.DatabaseContext.Migrations;
using EfYou.Security.Models;
using EfYou.Security.User;

namespace EfYou.Security.DatabaseContext
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