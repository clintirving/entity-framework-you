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
using Atlas.DatabaseContext;
using Atlas.ScopeOfResponsibility;
using Atlas.Security.DatabaseContext;
using Atlas.Security.Models;
using Atlas.Security.User;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AtlasTests.ScopeOfResponsibility
{
    [TestClass]
    public class ScopeOfResponsibilityServiceOfTTests
    {
        private readonly Mock<ILogger> _log = new Mock<ILogger>();
        private Mock<IContext> _context;
        private Mock<ISecurityContextFactory> _contextFactory;
        private Mock<IIdentityService> _identityService;
        private Mock<DbSet<DummyEntity>> _mockDbSet;
        private Mock<DbSet<Login>> _mockDbSetLogins;
        private Mock<ScopeOfResponsibilityServiceOfT<DummyEntity>> _scopeOfResponsibilityService;

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

            _scopeOfResponsibilityService =
                new Mock<ScopeOfResponsibilityServiceOfT<DummyEntity>>(_contextFactory.Object, _identityService.Object, _log.Object) {CallBase = true};
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
            _scopeOfResponsibilityService.Object.GetLoginForLoggedInUser();

            // Assert
            // By Exepected Exception
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityException))]
        public void GetLoginForLoggedInUser_NoLoginWithMatchingEmailInDb_ThrowsSecurityException()
        {
            // Arrange
            SetMockLoginData(new List<Login> {new Login {Email = "NotAMatch@email.com"}});

            // Act
            _scopeOfResponsibilityService.Object.GetLoginForLoggedInUser();

            // Assert
            // By Exepected Exception
        }

        [TestMethod]
        public void GetLoginForLoggedInUser_LoginWithMatchingEmailInDb_ReturnsLogin()
        {
            // Arrange
            var login = new Login {Email = "email@email.com"};

            SetMockLoginData(new List<Login> {login});

            // Act
            var loginReturned = _scopeOfResponsibilityService.Object.GetLoginForLoggedInUser();

            // Assert
            Assert.AreEqual(login, loginReturned);
        }

        [TestMethod]
        public void FilterResultOnCurrentPrincipal_UnrestrictedScopeOfResponsibility_ReturnsQueryUnmodified()
        {
            // Arrange
            var ids = new List<int>();
            var data = new List<DummyEntity> {new DummyEntity(), new DummyEntity()};
            SetMockData(data);
            var query = data.AsQueryable();
            _scopeOfResponsibilityService.Setup(x => x.RestrictScopeOfResponsibilityOnLoginConfiguration(out ids)).Returns(false);

            // Act
            var result = _scopeOfResponsibilityService.Object.FilterResultOnCurrentPrincipal(query);

            // Assert
            Assert.AreEqual(query, result);
        }

        [TestMethod]
        public void FilterResultOnCurrentPrincipal_RestrictedScopeOfResponsibility_ReturnsEntitiesWithMatchingAllowedIds()
        {
            // Arrange
            var ids = new List<int> {1};
            var firstEntity = new DummyEntity {Id = 1};
            var secondEntity = new DummyEntity {Id = 2};
            var data = new List<DummyEntity> {firstEntity, secondEntity};
            SetMockData(data);
            var query = data.AsQueryable();
            _scopeOfResponsibilityService.Setup(x => x.RestrictScopeOfResponsibilityOnLoginConfiguration(out ids)).Returns(true);

            // Act
            var result = _scopeOfResponsibilityService.Object.FilterResultOnCurrentPrincipal(query);

            // Assert
            Assert.AreEqual(firstEntity, result.Single());
        }
    }
}