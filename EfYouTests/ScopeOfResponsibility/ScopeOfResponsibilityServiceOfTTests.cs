using System.Collections.Generic;
using System.Linq;
using EfYou.ScopeOfResponsibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EfYouTests.ScopeOfResponsibility
{
    [TestClass]
    public class ScopeOfResponsibilityServiceOfTTests
    {
        private Mock<ScopeOfResponsibilityServiceOfT<DummyEntity>> _scopeOfResponsibilityService;

        [TestInitialize]
        public void Setup()
        {
            _scopeOfResponsibilityService =
                new Mock<ScopeOfResponsibilityServiceOfT<DummyEntity>>() { CallBase = true };
        }

        [TestMethod]
        public void FilterResultOnCurrentPrincipal_UnrestrictedScopeOfResponsibility_ReturnsQueryUnmodified()
        {
            // Arrange
            var ids = new List<int>();
            var data = new List<DummyEntity> { new DummyEntity(), new DummyEntity() };
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
            var ids = new List<int> { 1 };
            var firstEntity = new DummyEntity { Id = 1 };
            var secondEntity = new DummyEntity { Id = 2 };
            var data = new List<DummyEntity> { firstEntity, secondEntity };
            var query = data.AsQueryable();
            _scopeOfResponsibilityService.Setup(x => x.RestrictScopeOfResponsibilityOnLoginConfiguration(out ids)).Returns(true);

            // Act
            var result = _scopeOfResponsibilityService.Object.FilterResultOnCurrentPrincipal(query);

            // Assert
            Assert.AreEqual(firstEntity, result.Single());
        }
    }
}
