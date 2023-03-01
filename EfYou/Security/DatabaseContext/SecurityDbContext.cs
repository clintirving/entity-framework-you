// // -----------------------------------------------------------------------
// // <copyright file="SecurityDbContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using EfYou.DatabaseContext;
using EfYou.Security.Models;
using EfYou.Security.User;
using Microsoft.Extensions.Logging;

namespace EfYou.Security.DatabaseContext
{
    public class SecurityDbContext : Context
    {
        // Required for entity framework migrations.
        public SecurityDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public SecurityDbContext(DbContextOptions dbContextOptions, IIdentityService identityService, ILogger log)
            : base(dbContextOptions, identityService, log)
        {
        }

        public virtual DbSet<Login> Logins { get; set; }
    }
}