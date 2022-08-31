using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using EfYou.Security.DatabaseContext;
using EfYou.Security.Models;
using EfYou.Security.User;

namespace EfYou.ScopeOfResponsibility
{
    public class LoginCache : ILoginCache
    {
        private readonly ISecurityContextFactory _contextFactory;
        private readonly IIdentityService _identityService;

        private DateTime _loginCacheLastDumped = DateTime.MinValue;
        public virtual TimeSpan LoginCacheDumpInterval => TimeSpan.MaxValue;

        private readonly Dictionary<string, Login> _loginCache = new Dictionary<string, Login>();

        public LoginCache(ISecurityContextFactory contextFactory,IIdentityService identityService)
        {
            _contextFactory = contextFactory;
            _identityService = identityService;
        }

        public virtual void ClearLoginCache()
        {
            lock (_loginCache)
            {
                _loginCache.Clear();
                _loginCacheLastDumped = DateTime.UtcNow;
            }
        }

        public virtual void ClearLoginCacheForEmail(string email)
        {
            lock (_loginCache)
            {
                if (_loginCache.ContainsKey(email))
                {
                    _loginCache.Remove(email);
                }
            }
        }

        public virtual void UpdateLoginCacheForLogin(Login login)
        {
            lock (_loginCache)
            {
                if (_loginCache.ContainsKey(login.Email))
                {
                    _loginCache[login.Email] = login;
                }
                else
                {
                    _loginCache.Add(login.Email, login);
                }
            }
        }

        public virtual Login GetLoginForLoggedInUser()
        {
            var email = _identityService.GetEmail();

            Login login;

            lock (_loginCache)
            {
                if (DateTime.UtcNow.Subtract(_loginCacheLastDumped) > LoginCacheDumpInterval)
                {
                    _loginCache.Clear();
                    _loginCacheLastDumped = DateTime.UtcNow;
                }
            }

            lock (_loginCache)
            {
                if (!_loginCache.ContainsKey(email))
                {
                    login = FetchLoginFromDatabase(email);

                    if (login == null)
                    {
                        throw new SecurityException("The currently logged in user has no Login entity assigned.");
                    }

                    UpdateLoginCacheForLogin(login);
                }
                else
                {
                    login = _loginCache[email];
                }
            }

            return login;
        }

        protected  virtual Login FetchLoginFromDatabase(string email)
        {
            Login login;
            using (var context = _contextFactory.Create())
            {
                login = context.Set<Login>()
                    .Where(x => x.Email == email)
                    .Include("LoginPermissions")
                    .Include("LoginPermissions.LoginPermissionItems").FirstOrDefault();
            }

            return login;
        }
    }
}
