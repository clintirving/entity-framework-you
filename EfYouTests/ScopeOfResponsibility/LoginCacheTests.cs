// // -----------------------------------------------------------------------
// // <copyright file="ScopeOfResponsibilityServiceTests.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security;
using EfYou.DatabaseContext;
using EfYou.ScopeOfResponsibility;
using EfYou.Security.DatabaseContext;
using EfYou.Security.Models;
using EfYou.Security.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EfYouTests.ScopeOfResponsibility
{
    [TestClass]
    public class LoginCacheTests
    {
        private Mock<IContext> _context;
        private Mock<ISecurityContextFactory> _contextFactory;
        private Mock<IIdentityService> _identityService;
        private Mock<DbSet<DummyEntity>> _mockDbSet;
        private Mock<DbSet<Login>> _mockDbSetLogins;
        private Mock<LoginCache> _loginCache;

        [TestInitialize]
        public void Setup()
        {
            _mockDbSet = new Mock<DbSet<DummyEntity>>();
            _mockDbSetLogins = new Mock<DbSet<Login>>();

            SetMockData(new List<DummyEntity>());
            SetMockLoginData(new List<Login>());

            _context = new Mock<IContext>();
            _context.Setup(x => x.Set<DummyEntity>()).Returns(_mockDbSet.Object);
            _context.Setup(x => x.Set<Login>()).Returns(_mockDbSetLogins.Object);
            _context.As<IObjectContextAdapter>();

            _contextFactory = new Mock<ISecurityContextFactory>();
            _contextFactory.Setup(x => x.Create()).Returns(_context.Object);

            _identityService = new Mock<IIdentityService>();
            _identityService.Setup(x => x.GetEmail()).Returns("email@email.com");

            _loginCache =
                new Mock<LoginCache>(_contextFactory.Object, _identityService.Object) {CallBase = true};
        }

        private void SetMockLoginData(IEnumerable<Login> data)
        {
            var mockDataQueryable = data.AsQueryable();

            var queryable = _mockDbSetLogins.As<IQueryable<Login>>();
            queryable.Setup(x => x.Provider).Returns(mockDataQueryable.Provider);
            queryable.Setup(x => x.Expression).Returns(mockDataQueryable.Expression);
            queryable.Setup(x => x.ElementType).Returns(mockDataQueryable.ElementType);
            queryable.Setup(x => x.GetEnumerator()).Returns(mockDataQueryable.GetEnumerator());
        }

        private void SetMockData(IEnumerable<DummyEntity> data)
        {
            var mockDataQueryable = data.AsQueryable();

            var queryable = _mockDbSet.As<IQueryable<DummyEntity>>();
            queryable.Setup(x => x.Provider).Returns(mockDataQueryable.Provider);
            queryable.Setup(x => x.Expression).Returns(mockDataQueryable.Expression);
            queryable.Setup(x => x.ElementType).Returns(mockDataQueryable.ElementType);
            queryable.Setup(x => x.GetEnumerator()).Returns(mockDataQueryable.GetEnumerator());
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityException))]
        public void GetLoginForLoggedInUser_NoLoginsInDb_ThrowsSecurityException()
        {
            // Arrange

            // Act
            _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            // By Expected Exception
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityException))]
        public void GetLoginForLoggedInUser_NoLoginWithMatchingEmailInDb_ThrowsSecurityException()
        {
            // Arrange
            SetMockLoginData(new List<Login> {new Login {Email = "NotAMatch@email.com"}});

            // Act
            _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            // By Expected Exception
        }

        [TestMethod]
        public void GetLoginForLoggedInUser_LoginWithMatchingEmailInDb_ReturnsLogin()
        {
            // Arrange
            var login = new Login {Email = "email@email.com"};

            SetMockLoginData(new List<Login> {login});

            // Act
            var loginReturned = _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            Assert.AreEqual(login, loginReturned);
        }


        [TestMethod]
        public void ClearLoginCache_NextCallToGetLoginForLoggedInUserWillRefreshFromDb()
        {
            // Arrange
            var login = new Login { Email = "email@email.com" };

            SetMockLoginData(new List<Login> { login });

            // Act
            var result = _loginCache.Object.GetLoginForLoggedInUser();
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(1));   // Make sure we've hit the DB once to initially cache the login
            _loginCache.Object.GetLoginForLoggedInUser();
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(1));   // No hit to the DB unless we clear the cache
            _loginCache.Object.ClearLoginCache();                             // clear the cache
            _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(2));   // Check we hit the DB since we've cleared the cache
        }

        [TestMethod]
        public void ClearLoginCacheForEmail_NextCallToGetLoginForLoggedInUserWillRefreshFromDb()
        {
            // Arrange
            var login = new Login { Email = "email@email.com" };

            SetMockLoginData(new List<Login> { login });

            // Act
            var result = _loginCache.Object.GetLoginForLoggedInUser();
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(1));   // Make sure we've hit the DB once to initially cache the login
            _loginCache.Object.GetLoginForLoggedInUser();
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(1));   // No hit to the DB unless we clear the cache
            _loginCache.Object.ClearLoginCacheForEmail(login.Email);          // clear the cache for this email
            _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            _context.Verify(x => x.Set<Login>(), () => Times.Exactly(2));   // Check we hit the DB since we've cleared the cache
        }

        [TestMethod]
        public void UpdateLoginCacheForLogin_ConsecutiveCallsToGetLoginForLoggedInUserReturnSetLogin()
        {
            // Arrange
            var login = new Login { Email = "email@email.com", LoginPermissions = new List<LoginPermission> { new LoginPermission { FullAccess = true } } };

            SetMockLoginData(new List<Login> { login });

            // Act
            var before = _loginCache.Object.GetLoginForLoggedInUser();
            _loginCache.Object.UpdateLoginCacheForLogin(new Login { Email = "email@email.com" , LoginPermissions = new List<LoginPermission> { new LoginPermission { FullAccess = false } } });
            var after = _loginCache.Object.GetLoginForLoggedInUser();

            // Assert
            Assert.AreEqual(true, before.LoginPermissions.Single().FullAccess);
            Assert.AreEqual(false, after.LoginPermissions.Single().FullAccess);
        }
    }
}