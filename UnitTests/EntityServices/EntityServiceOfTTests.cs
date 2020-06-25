// // -----------------------------------------------------------------------
// // <copyright file="EntityServiceOfTTests.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EfYou.CascadeDelete;
using EfYou.DatabaseContext;
using EfYou.EntityServices;
using EfYou.Filters;
using EfYou.Permissions;
using EfYou.ScopeOfResponsibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EfYouTests.EntityServices
{
    [TestClass]
    public class EntityServiceOfTTests
    {
        private Mock<ICascadeDeleteService<DummyEntity>> _cascadeDeleteService;
        private Mock<IContext> _context;
        private Mock<IContextFactory> _contextFactory;
        private IEntityService<DummyEntity> _entityService;
        private Mock<IFilterService<DummyEntity>> _filterService;
        private Mock<DbSet<DummyEntity>> _mockDbSet;
        private Mock<IPermissionService<DummyEntity>> _permissionService;
        private Mock<IScopeOfResponsibilityService<DummyEntity>> _scopeOfResponsibilityService;

        [TestInitialize]
        public void SetupConfigurationService()
        {
            _mockDbSet = new Mock<DbSet<DummyEntity>>();

            // Default the mock data to an empty list.
            SetMockData(new List<DummyEntity>());

            _context = new Mock<IContext>();
            _context.Setup(x => x.Set<DummyEntity>()).Returns(_mockDbSet.Object);
            _context.Setup(x => x.DatabaseAccessor).Returns(new Mock<IDatabaseAccessor>().Object);
            _contextFactory = new Mock<IContextFactory>();
            _contextFactory.Setup(x => x.Create()).Returns(_context.Object);

            _filterService = new Mock<IFilterService<DummyEntity>>();
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x);
            _filterService.Setup(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()))
                .Returns<IQueryable<DummyEntity>, DummyEntity>((x, y) => _mockDbSet.Object);
            _filterService.Setup(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>()))
                .Returns<IQueryable<DummyEntity>, List<string>>((x, y) => x);
            _filterService.Setup(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<OrderBy>>()))
                .Returns<IQueryable<DummyEntity>, List<OrderBy>>((x, y) => x);
            _filterService.Setup(x => x.AddPaging(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<Paging>()))
                .Returns<IQueryable<DummyEntity>, Paging>((x, y) => x);

            _cascadeDeleteService = new Mock<ICascadeDeleteService<DummyEntity>>();

            _permissionService = new Mock<IPermissionService<DummyEntity>>();

            _scopeOfResponsibilityService = new Mock<IScopeOfResponsibilityService<DummyEntity>>();
            _scopeOfResponsibilityService.Setup(x => x.FilterResultOnCurrentPrincipal(It.IsAny<IQueryable<DummyEntity>>()))
                .Returns<IQueryable<DummyEntity>>(x => x);

            _entityService = new EntityService<DummyEntity>(_contextFactory.Object, _filterService.Object, _cascadeDeleteService.Object,
                _permissionService.Object, _scopeOfResponsibilityService.Object);
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
        public void Get_Ids_CallsGetOnPermissionService()
        {
            // Arrange

            // Act
            _entityService.Get(new List<long> {1});

            // Assert
            _permissionService.Verify(x => x.Get());
        }

        [TestMethod]
        public void Get_CallsFilterResultOnCurrentPrincipalOnScopeOfResponsibilityService()
        {
            // Arrange

            // Act
            _entityService.Get(new List<long> {1});

            // Assert
            _scopeOfResponsibilityService.Verify(x => x.FilterResultOnCurrentPrincipal(It.IsAny<IQueryable<DummyEntity>>()));
        }

        [TestMethod]
        public void Get_Ids_CallsFilterResultOnGetOnFilterServiceWithIds()
        {
            // Arrange
            var ids = new List<long> {1};

            // Act
            _entityService.Get(ids);

            // Assert
            _filterService.Verify(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<long>>(y => y == ids)));
        }

        [TestMethod]
        public void Get_CallsAddIncludesOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Get(new List<long> {1});

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>()));
        }

        [TestMethod]
        public void Get_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.Get(new List<long> {1}, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void Get_CallsAddOrderBysOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Get(new List<long> {1});

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<OrderBy>>()));
        }

        [TestMethod]
        public void Get_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.Get(new List<long> {1}, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void Get_CallsAddPagingOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Get(new List<long> {1});

            // Assert
            _filterService.Verify(x => x.AddPaging(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<Paging>()));
        }

        [TestMethod]
        public void Get_Paging_CallsAddPagingOnFilterServiceWithPaging()
        {
            // Arrange
            var paging = new Paging();

            // Act
            _entityService.Get(new List<long> {1}, null, null, paging);

            // Assert
            _filterService.Verify(x => x.AddPaging(It.IsAny<IQueryable<DummyEntity>>(), It.Is<Paging>(y => y == paging)));
        }

        [TestMethod]
        public void Get_ReturnsFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}});

            // Act
            var results = _entityService.Get(new List<long> {1});

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void GetLast_TwoDummyEntitiesExist__ReturnsFirstFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> { new DummyEntity { Id = 1 }, new DummyEntity { Id = 2 } });

            // Act
            var result = _entityService.GetLast(new List<long> { 1, 2 });

            // Assert
            Assert.AreEqual(2, result.Id);
        }

        [TestMethod]
        public void GetLast_OneDummyEntityExists__ReturnsFirstFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> { new DummyEntity { Id = 1 } });

            // Act
            var result = _entityService.GetLast(new List<long> { 1 });

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetLast_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.GetLast(new List<long> { 1 }, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void GetLast_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.GetFirst(new List<long> { 1 }, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void GetFirst_ReturnsFirstFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}});

            // Act
            var result = _entityService.GetFirst(new List<long> {1, 2});

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetFirst_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.GetFirst(new List<long> {1}, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void GetFirst_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.GetFirst(new List<long> {1}, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void Search_Filters_CallsSearchOnPermissionService()
        {
            // Arrange

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _permissionService.Verify(x => x.Search());
        }

        [TestMethod]
        public void Search_CallsFilterResultOnCurrentPrincipalOnScopeOfResponsibilityService()
        {
            // Arrange

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _scopeOfResponsibilityService.Verify(x => x.FilterResultOnCurrentPrincipal(It.IsAny<IQueryable<DummyEntity>>()));
        }

        [TestMethod]
        public void Search_FiltersCountLessThanTwenty_CallsFilterResultOnSearchOnFilterServiceWithEachFilter()
        {
            // Arrange
            var filterCount = 19;
            var filters = new List<DummyEntity>();
            for (var i = 0; i < filterCount; i++)
            {
                filters.Add(new DummyEntity());
            }

            // Act
            _entityService.Search(filters);

            // Assert
            _filterService.Verify(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()),
                Times.Exactly(filterCount));
        }

        [TestMethod]
        public void Search_FiltersCountTwentyOrMore_CallsFilterResultOnSearchOnFilterServiceWithEachFilter()
        {
            // Arrange
            var filterCount = 20;
            var filters = new List<DummyEntity>();
            for (var i = 0; i < filterCount; i++)
            {
                filters.Add(new DummyEntity());
            }

            // Act
            _entityService.Search(filters);

            // Assert
            _filterService.Verify(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()),
                Times.Exactly(filterCount));
        }

        [TestMethod]
        public void Search_CallsAddIncludesOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>()));
        }

        [TestMethod]
        public void Search_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()}, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void Search_CallsAddOrderBysOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<OrderBy>>()));
        }

        [TestMethod]
        public void Search_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()}, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void Search_CallsAddPagingOnFilterService()
        {
            // Arrange

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _filterService.Verify(x => x.AddPaging(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<Paging>()));
        }

        [TestMethod]
        public void Search_Paging_CallsAddPagingOnFilterServiceWithPaging()
        {
            // Arrange
            var paging = new Paging();

            // Act
            _entityService.Search(new List<DummyEntity> {new DummyEntity()}, null, null, paging);

            // Assert
            _filterService.Verify(x => x.AddPaging(It.IsAny<IQueryable<DummyEntity>>(), It.Is<Paging>(y => y == paging)));
        }

        [TestMethod]
        public void Search_ReturnsFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}});

            // Act
            var results = _entityService.Search(new List<DummyEntity> {new DummyEntity()});

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void SearchLast_TwoDummyEntitiesExist_ReturnsLastFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> { new DummyEntity { Id = 1 }, new DummyEntity { Id = 2 } });

            // Act
            var result = _entityService.SearchLast(new List<DummyEntity> { new DummyEntity() });

            // Assert
            Assert.AreEqual(2, result.Id);
        }

        [TestMethod]
        public void SearchLast_OneDummyEntityExists_ReturnsLastFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> { new DummyEntity { Id = 1 } });

            // Act
            var result = _entityService.SearchLast(new List<DummyEntity> { new DummyEntity() });

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void SearchLast_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.SearchLast(new List<DummyEntity> { new DummyEntity() }, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void SearchLast_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.SearchLast(new List<DummyEntity> { new DummyEntity() }, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void SearchFirst_ReturnsFirstFromFilteredContextDataSet()
        {
            // Arrange
            SetMockData(new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}});

            // Act
            var result = _entityService.SearchFirst(new List<DummyEntity> {new DummyEntity()});

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void SearchFirst_Includes_CallsAddIncludesOnFilterServiceWithIncludes()
        {
            // Arrange
            var includes = new List<string>();

            // Act
            _entityService.SearchFirst(new List<DummyEntity> {new DummyEntity()}, includes);

            // Assert
            _filterService.Verify(x => x.AddIncludes(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == includes)));
        }

        [TestMethod]
        public void SearchFirst_OrderBys_CallsAddOrderBysOnFilterServiceWithOrderBys()
        {
            // Arrange
            var orderBys = new List<OrderBy>();

            // Act
            _entityService.SearchFirst(new List<DummyEntity> {new DummyEntity()}, null, orderBys);

            // Assert
            _filterService.Verify(x => x.AddOrderBys(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<OrderBy>>(y => y == orderBys)));
        }

        [TestMethod]
        public void SearchAggregate_CallsFilterResultOnCurrentPrincipalOnScopeOfResponsibilityService()
        {
            // Arrange
            var groupBys = new List<string>();

            // Act
            _entityService.SearchAggregate(new List<DummyEntity> {new DummyEntity()}, groupBys);

            // Assert
            _scopeOfResponsibilityService.Verify(x => x.FilterResultOnCurrentPrincipal(It.IsAny<IQueryable<DummyEntity>>()));
        }

        [TestMethod]
        public void SearchAggregate_CallsAddAggregationFilter()
        {
            // Arrange
            var groupBys = new List<string> {"Name"};

            // Act
            _entityService.SearchAggregate(new List<DummyEntity> {new DummyEntity()}, groupBys);

            // Assert
            _filterService.Verify(x => x.AddAggregationFilter(It.IsAny<IQueryable<DummyEntity>>(), It.Is<List<string>>(y => y == groupBys),
                It.IsAny<Paging>(), It.IsAny<List<OrderBy>>()));
        }

        [TestMethod]
        public void SearchAggregate_ReturnsResultSetWithThreeGroups()
        {
            // Arrange
            var groupBys = new List<string> {"Name"};

            SetMockData(new List<DummyEntity>
            {
                new DummyEntity {Id = 1, Name = "Thing"},
                new DummyEntity {Id = 2, Name = "Thing"},
                new DummyEntity {Id = 3, Name = "AlsoThing"},
                new DummyEntity {Id = 4, Name = "AnotherThing"}
            });

            IQueryable<IGrouping<long, List<long>>> DummyAggregate(IQueryable<DummyEntity> x, List<string> y, Paging z, List<OrderBy> a)
            {
                return x.GroupBy(b => (object) new {b.Name}, b => (long) b.Id)
                    .GroupBy(c => c.Max(), c => c.OrderByDescending(d => d).ToList());
            }

            _filterService.Setup(x => x.AddAggregationFilter(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>(),
                    It.IsAny<Paging>(), It.IsAny<List<OrderBy>>()))
                .Returns(
                    (Func<IQueryable<DummyEntity>, List<string>, Paging, List<OrderBy>, IQueryable<IGrouping<long, List<long>>>>) DummyAggregate);

            // Act
            var results = _entityService.SearchAggregate(new List<DummyEntity> {new DummyEntity()}, groupBys);

            // Assert
            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void Add_CallsAddOnPermissionService()
        {
            // Arrange

            // Act
            _entityService.Add(new List<DummyEntity>());

            // Assert
            _permissionService.Verify(x => x.Add());
        }

        [TestMethod]
        public void Add_Entities_AllEntitiesAddedToContextDataSet()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4},
                new DummyEntity {Id = 5}
            };

            // Act
            _entityService.Add(newEntities);

            // Assert
            _mockDbSet.Verify(x => x.AddRange(It.Is<IEnumerable<DummyEntity>>(y => y == newEntities)));
        }

        [TestMethod]
        public void Add_EntitiesWithDefaultValueDatePropertyAndPropertyUnassigned_PropertyAssignedToDefaultOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, DefaultingDate = null},
                new DummyEntity {Id = 5, DefaultingDate = null}
            };

            var utcBefore = DateTime.UtcNow;

            // Act
            var added = _entityService.Add(newEntities);

            var utcAfter = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(added[0].DefaultingDate >= utcBefore && added[0].DefaultingDate <= utcAfter);
            Assert.IsTrue(added[1].DefaultingDate >= utcBefore && added[1].DefaultingDate <= utcAfter);
        }

        [TestMethod]
        public void Add_EntitiesWithEmptyStringDefaultValue_PropertyAssignedToEmptyStringOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, StringWithDefaultEmptyValue = null},
                new DummyEntity {Id = 5, StringWithDefaultEmptyValue = null}
            };

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added[0].StringWithDefaultEmptyValue == string.Empty);
            Assert.IsTrue(added[1].StringWithDefaultEmptyValue == string.Empty);
        }

        [TestMethod]
        public void Add_EntitiesWithStringValueDefaultValue_PropertyAssignedToStringValueOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, StringWithDefaultValue = null},
                new DummyEntity {Id = 5, StringWithDefaultValue = null}
            };

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added[0].StringWithDefaultValue == "Thing");
            Assert.IsTrue(added[1].StringWithDefaultValue == "Thing");
        }

        [TestMethod]
        public void Add_EntitiesWithDateTimeValueDefaultValue_PropertyAssignedToDateTimeValueOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, DateTimeWithDefaultValue = null},
                new DummyEntity {Id = 5, DateTimeWithDefaultValue = null}
            };

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added[0].DateTimeWithDefaultValue == DateTime.Parse("2016-1-30"));
            Assert.IsTrue(added[1].DateTimeWithDefaultValue == DateTime.Parse("2016-1-30"));
        }

        [TestMethod]
        public void Add_EntitiesWithTimeSpanValueDefaultValue_PropertyAssignedToTimeSpanValueOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, TimeSpanWithDefaultValue = null},
                new DummyEntity {Id = 5, TimeSpanWithDefaultValue = null}
            };

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added[0].TimeSpanWithDefaultValue == TimeSpan.Parse("23:59:59.9999999"));
            Assert.IsTrue(added[1].TimeSpanWithDefaultValue == TimeSpan.Parse("23:59:59.9999999"));
        }

        [TestMethod]
        public void Add_EntitiesWithIntValueDefaultValue_PropertyAssignedToIntValueOnAdd()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4, IntWithDefaultValue = null},
                new DummyEntity {Id = 5, IntWithDefaultValue = null}
            };

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added[0].IntWithDefaultValue == 100);
            Assert.IsTrue(added[1].IntWithDefaultValue == 100);
        }

        [TestMethod]
        public void Add_EntitiesWithChildEntitiesWithDefaultValue_ChildEntityPropertyAssignedToDefaultValue()
        {
            var dummyEntity = new DummyEntity {Id = 4, DummyChildren = new List<DummyChild>()};
            dummyEntity.DummyChildren.Add(new DummyChild {DummyEntity = dummyEntity});

            // Arrange
            var newEntities = new List<DummyEntity> {dummyEntity};

            // Act
            var added = _entityService.Add(newEntities);

            // Assert
            Assert.IsTrue(added.Single().DummyChildren.Single().IntWithDefaultValue == 100);
        }

        [TestMethod]
        public void Add_ListOfEntities_ContextDataSetSaved()
        {
            // Arrange
            var newEntities = new List<DummyEntity>
            {
                new DummyEntity {Id = 4},
                new DummyEntity {Id = 5}
            };

            // Act
            _entityService.Add(newEntities);

            // Assert
            _context.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Delete_CallsDeleteOnPermissionService()
        {
            // Arrange

            // Act
            _entityService.Delete(new List<long> {1});

            // Assert
            _permissionService.Verify(x => x.Delete());
        }

        [TestMethod]
        public void Delete_ListOfIds_CallsCascadeDeleteWithFilteredEntities()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity {Id = 3},
                new DummyEntity {Id = 4}
            };
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Delete(new List<long> {3, 4});

            // Assert
            _cascadeDeleteService.Verify(x => x.CascadeDelete(It.Is<List<long>>(y => y.Single() == 3)));
        }

        [TestMethod]
        public void Delete_NoMatchingIds_DoesNotCallCascadeDelete()
        {
            // Arrange
            var entities = new List<DummyEntity>();
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Delete(new List<long> {3, 4});

            // Assert
            _cascadeDeleteService.Verify(x => x.CascadeDelete(It.Is<List<long>>(y => y.Single() == 3)), Times.Never);
        }

        [TestMethod]
        public void Delete_ListOfIds_CallsDeleteOnContextDataSetWithFilteredEntities()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity {Id = 3},
                new DummyEntity {Id = 4}
            };
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Delete(new List<long> {3, 4});

            // Assert
            _context.Verify(x => x.SetState(It.Is<DummyEntity>(y => y.Id == 3), EntityState.Deleted));
        }

        [TestMethod]
        public void Delete_ListOfIds_CallsSaveChangesOnDataContext()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity {Id = 3},
                new DummyEntity {Id = 4}
            };
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Delete(new List<long> {3, 4});

            // Assert
            _context.Verify(x => x.SaveChanges());
        }


        [TestMethod]
        public void Update_CallsUpdateOnPermissionService()
        {
            // Arrange

            // Act
            _entityService.Update(new List<DummyEntity> {new DummyEntity()});

            // Assert
            _permissionService.Verify(x => x.Update());
        }

        [TestMethod]
        public void Update_ListOfIds_SetsStateOfFilteredEntitiesToModified()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity {Id = 3},
                new DummyEntity {Id = 4}
            };
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Update(entities);

            // Assert
            _context.Verify(x => x.SetState(It.Is<DummyEntity>(y => y.Id == 3), EntityState.Modified));
        }

        [TestMethod]
        public void Update_ListOfIds_CallsSaveChangesOnDataContext()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity {Id = 3},
                new DummyEntity {Id = 4}
            };
            var accessibleIds = new List<long> {3};
            SetMockData(entities);
            _filterService.Setup(x => x.FilterResultsOnGet(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<long>>()))
                .Returns<IQueryable<DummyEntity>, List<long>>((x, y) => x.Where(z => accessibleIds.Contains(z.Id)));

            // Act
            _entityService.Update(entities);

            // Assert
            _context.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void SearchCount_CallsFilterResultOnSearchOnFilterServiceWithEachFilter()
        {
            // Arrange
            var filterCount = 5;
            var filters = new List<DummyEntity>();
            for (var i = 0; i < filterCount; i++)
            {
                filters.Add(new DummyEntity());
            }

            // Act
            _entityService.SearchCount(filters);

            // Assert
            _filterService.Verify(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()),
                Times.Exactly(filterCount));
        }

        [TestMethod]
        public void SearchCount_ListOfFilters_ReturnsCountOfSetFilteredByIFilterServiceQuery()
        {
            // Arrange
            SetMockData(new List<DummyEntity>
            {
                new DummyEntity {Name = "Thing"},
                new DummyEntity {Name = "Thing"},
                new DummyEntity {Name = "AlsoThing"},
                new DummyEntity {Name = "NotThing"}
            });

            // Filter service is a simple dummy filter which returns DummyEntity objects where the name matches the given filter.
            IQueryable<DummyEntity> DummyFilter(IQueryable<DummyEntity> x, DummyEntity y)
            {
                return x.Where(z => z.Name == y.Name);
            }

            _filterService.Setup(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()))
                .Returns((Func<IQueryable<DummyEntity>, DummyEntity, IQueryable<DummyEntity>>) DummyFilter);

            // Act
            // get count of items where name is Thing OR AlsoThing
            var searchCount = _entityService.SearchCount(new List<DummyEntity>
                {new DummyEntity {Name = "Thing"}, new DummyEntity {Name = "AlsoThing"}});

            // Assert
            Assert.AreEqual(3, searchCount);
        }

        [TestMethod]
        public void SearchAggregateCount_OneGroupByField_CallsFilterResultOnSearchOnFilterServiceWithEachFilterAndAddAggregationFilter()
        {
            // Arrange
            var filters = new List<DummyEntity> {new DummyEntity(), new DummyEntity()};

            // Act
            var result = _entityService.SearchAggregateCount(filters, new List<string> {"Name"});

            // Assert
            _filterService.Verify(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()), Times.Exactly(2));
            _filterService.Verify(
                x => x.AddAggregationFilter(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>(), It.IsAny<Paging>(),
                    It.IsAny<List<OrderBy>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void SearchAggregateCount_ListOfFiltersAndOneGroupBy_ReturnsCountOfSetFilteredByIFilterServiceQuery()
        {
            // Arrange
            SetMockData(new List<DummyEntity>
            {
                new DummyEntity {Id = 1, Name = "Thing"},
                new DummyEntity {Id = 2, Name = "Thing"},
                new DummyEntity {Id = 3, Name = "AlsoThing"},
                new DummyEntity {Id = 4, Name = "NotThing"}
            });

            // Filter service is a simple dummy filter which returns DummyEntity objects where the name matches the given filter.
            IQueryable<DummyEntity> DummyFilter(IQueryable<DummyEntity> x, DummyEntity y)
            {
                return x.Where(z => z.Name == y.Name);
            }

            _filterService.Setup(x => x.FilterResultsOnSearch(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()))
                .Returns((Func<IQueryable<DummyEntity>, DummyEntity, IQueryable<DummyEntity>>) DummyFilter);

            IQueryable<IGrouping<long, List<long>>> DummyAggregate(IQueryable<DummyEntity> x, List<string> y, Paging z, List<OrderBy> a)
            {
                return x.GroupBy(b => (object) new {b.Name}, b => (long) b.Id)
                    .GroupBy(c => c.Max(), c => c.OrderByDescending(d => d).ToList());
            }

            _filterService.Setup(x => x.AddAggregationFilter(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<List<string>>(),
                    It.IsAny<Paging>(), It.IsAny<List<OrderBy>>()))
                .Returns(
                    (Func<IQueryable<DummyEntity>, List<string>, Paging, List<OrderBy>, IQueryable<IGrouping<long, List<long>>>>) DummyAggregate);


            // Act
            // get count of items where name is Thing OR AlsoThing
            var searchCount = _entityService.SearchAggregateCount(
                new List<DummyEntity> {new DummyEntity {Name = "Thing"}, new DummyEntity {Name = "AlsoThing"}}, new List<string> {"Name"});

            // Assert
            Assert.AreEqual(2, searchCount);
        }
    }
}