// // -----------------------------------------------------------------------
// // <copyright file="SecurityContextFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using Common.Logging;
using EfYouCore.DatabaseContext;
using EfYouCore.Security.User;

namespace EfYouCore.Security.DatabaseContext
{
    public class SecurityContextFactory : ISecurityContextFactory
    {
        private readonly IIdentityService _identityService;
        private readonly ILog _log;

        public SecurityContextFactory(IIdentityService identityService, ILog log)
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