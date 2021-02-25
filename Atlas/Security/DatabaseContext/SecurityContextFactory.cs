// // -----------------------------------------------------------------------
// // <copyright file="SecurityContextFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using Atlas.DatabaseContext;
using Atlas.Security.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Atlas.Security.DatabaseContext
{
    public class SecurityContextFactory : ISecurityContextFactory
    {
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _log;

        public SecurityContextFactory(IIdentityService identityService, IConfiguration configuration, ILogger log)
        {
            _identityService = identityService;
            _configuration = configuration;
            _log = log;
        }

        public virtual IContext Create()
        {
            return new SecurityDbContext(_identityService, _configuration, _log);
        }
    }
}