// // -----------------------------------------------------------------------
// // <copyright file="SecurityContextFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using EfYou.DatabaseContext;
using EfYou.Security.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EfYou.Security.DatabaseContext
{
    public class SecurityContextFactory : ISecurityContextFactory
    {
        private readonly DbContextOptions _dbContextOptions;
        private readonly IIdentityService _identityService;
        private readonly ILogger _log;

        public SecurityContextFactory(DbContextOptions dbContextOptions, IIdentityService identityService, ILogger log)
        {
            _dbContextOptions = dbContextOptions;
            _identityService = identityService;
            _log = log;
        }

        public virtual IContext Create()
        {
            return new SecurityDbContext(_dbContextOptions, _identityService, _log);
        }
    }
}