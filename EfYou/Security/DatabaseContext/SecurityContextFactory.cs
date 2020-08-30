// // -----------------------------------------------------------------------
// // <copyright file="SecurityContextFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using EfYou.DatabaseContext;
using EfYou.Security.User;
using Microsoft.Extensions.Logging;

namespace EfYou.Security.DatabaseContext
{
    public class SecurityContextFactory : ISecurityContextFactory
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger _log;

        public SecurityContextFactory(IIdentityService identityService, ILogger log)
        {
            _identityService = identityService;
            _log = log;
        }

        public virtual IContext Create()
        {
            return new SecurityDbContext(_identityService, _log);
        }
    }
}