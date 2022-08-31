using System;
using EfYou.Security.Models;

namespace EfYou.ScopeOfResponsibility
{
    public interface ILoginCache
    {
        TimeSpan LoginCacheDumpInterval { get; }
        void ClearLoginCache();
        void ClearLoginCacheForEmail(string email);
        void UpdateLoginCacheForLogin(Login login);
        Login GetLoginForLoggedInUser();
    }
}