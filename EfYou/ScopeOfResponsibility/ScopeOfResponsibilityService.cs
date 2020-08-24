// // -----------------------------------------------------------------------
// // <copyright file="ScopeOfResponsibilityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security;
using Common.Logging;
using EfYou.Extensions;
using EfYou.Security.DatabaseContext;
using EfYou.Security.Models;
using EfYou.Security.User;

namespace EfYou.ScopeOfResponsibility
{
    public abstract class ScopeOfResponsibilityService<T> : IScopeOfResponsibilityService<T> where T : class, new()
    {
        private readonly ISecurityContextFactory _contextFactory;
        private readonly IIdentityService _identityService;
        private readonly ILog _log;

        private readonly Dictionary<string, Login> _loginCache = new Dictionary<string, Login>();

        private DateTime _loginCacheLastDumped = DateTime.MinValue;

        protected ScopeOfResponsibilityService(ISecurityContextFactory contextFactory, IIdentityService identityService, ILog log)
        {
            _contextFactory = contextFactory;
            _identityService = identityService;
            _log = log;
        }

        protected virtual TimeSpan LoginCacheDumpInterval => TimeSpan.FromMinutes(5);

        public virtual IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query)
        {
            List<int> restrictedToIds;
            if (RestrictScopeOfResponsibilityOnLoginConfiguration(out restrictedToIds))
            {
                var primaryKeyProperty = typeof(T).GetPrimaryKeyProperty();

                if (primaryKeyProperty.PropertyType != typeof(int))
                {
                    throw new ApplicationException(
                        string.Format("Primary Key property of {0} is not valid for filtering on scope of responsibility, it must be an int",
                            primaryKeyProperty.PropertyType));
                }

                return query.Where(string.Format("{0} in @0", primaryKeyProperty.Name), restrictedToIds);
            }

            return query;
        }

        public abstract bool RestrictScopeOfResponsibilityOnLoginConfiguration(out List<int> ids);

        public virtual Login GetLoginForLoggedInUser()
        {
            var email = _identityService.GetEmail();

            Login login;


            lock (_loginCache)
            {
                if (_loginCacheLastDumped.Add(LoginCacheDumpInterval) < DateTime.UtcNow)
                {
                    _loginCache.Clear();
                    _loginCacheLastDumped = DateTime.UtcNow;
                }
            }

            lock (_loginCache)
            {
                if (_loginCache.ContainsKey(email))
                {
                    login = _loginCache[email];
                }
                else
                {
                    using (var context = _contextFactory.Create())
                    {
                        login = context.Set<Login>()
                            .Where(x => x.Email == email)
                            .Include("LoginPermissions")
                            .Include("LoginPermissions.LoginPermissionItems").FirstOrDefault();

                        _loginCache.Add(email, login);
                    }
                }
            }

            if (login == null)
            {
                throw new SecurityException("The currently logged in user has no Login entity assigned.");
            }

            return login;
        }
    }
}