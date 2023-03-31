// // -----------------------------------------------------------------------
// // <copyright file="FilterServiceOfTTests.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EfYou.DatabaseContext;
using EfYou.Filters;
using EfYou.Model.FilterExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EfYouTests.Filters
{
    [TestClass]
    public class FilterServiceTests
    {
        private Mock<FilterService<DummyEntity>> GetFilterServiceMock()
        {
            var mock = new Mock<FilterService<DummyEntity>> {CallBase = true};
            return mock;
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsShort_ReturnsQueryableFilteredByShortIds()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyChild>> { CallBase = true };

            var queryable = new List<DummyChild> { new DummyChild { Id = 1 }, new DummyChild { Id = 2 }, new DummyChild { Id = 3 } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<dynamic> { 2 }, null);

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(2, results.Single().Id);
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsInt_ReturnsQueryableFilteredByIntIds()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<dynamic> {2}, null);

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(2, results.Single().Id);
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsLong_ReturnsQueryableFilteredByLongIds()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyParent>> { CallBase = true };

            var queryable = new List<DummyParent> { new DummyParent { Id = 1 }, new DummyParent { Id = 2 }, new DummyParent { Id = 3 } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<dynamic> { 2 }, null);

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(2, results.Single().Id);
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsGuid_ReturnsQueryableFilteredByGuidIds()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyEntityWithGuidId>> { CallBase = true };
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();
            var third = Guid.NewGuid();

            var queryable = new List<DummyEntityWithGuidId> { new DummyEntityWithGuidId { Id = first }, new DummyEntityWithGuidId { Id = second }, new DummyEntityWithGuidId { Id = third } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<dynamic> { second }, null);

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(second, results.Single().Id);
        }

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsNoneOfShortOrIntOrLongOrGuid_ThrowsApplicationException()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyEntityWithInvalidIdType>> { CallBase = true };

            var queryable = new List<DummyEntityWithInvalidIdType> { new DummyEntityWithInvalidIdType { Id = true }, new DummyEntityWithInvalidIdType { Id = false }, new DummyEntityWithInvalidIdType { Id = false } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<dynamic> { 2 }, null);

            // Assert
            // By Exception
        }

        [TestMethod]
        public void FilterResultsOnSearch_CallsAutoSearchFilter()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}, new DummyEntity {Id = 3}}.AsQueryable();

            var filter = new DummyEntity();

            // Act
            filterService.Object.FilterResultsOnSearch(queryable, filter, null);

            // Assert
            filterService.Verify(x => x.AutoFilter(queryable, filter, It.IsAny<IContext>()), Times.Once);
        }

        [TestMethod]
        public void AddIncludes_SingleInclude_CallsIncludeOnTheQueryableWithTheSingleInclude()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyParent>> { CallBase = true };

            var mockDbSet = new Mock<DbSet<DummyParent>>();

            mockDbSet.Setup(x => x.Include(It.IsAny<string>()))
                .Returns<string>(x => mockDbSet.Object);

            // Act
            filterService.Object.AddIncludes(mockDbSet.Object, new List<string> { "DummyEntity" }, null);

            // Assert
            mockDbSet.Verify(mock => mock.Include("DummyEntity"), Times.Once);
        }

        [TestMethod]
        public void AddIncludes_TwoIncludes_CallsIncludeOnTheQueryableWithBothIncludes()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyParent>> { CallBase = true };

            var mockDbSet = new Mock<DbSet<DummyParent>>();

            mockDbSet.Setup(x => x.Include(It.IsAny<string>()))
                .Returns<string>(x => mockDbSet.Object);

            // Act
            filterService.Object.AddIncludes(mockDbSet.Object, new List<string> { "DummyEntity", "DummyEntity.DummyChild" }, null);

            // Assert
            mockDbSet.Verify(mock => mock.Include("DummyEntity"), Times.Once);
            mockDbSet.Verify(mock => mock.Include("DummyEntity.DummyChild"), Times.Once);
        }

        [TestMethod]
        public void AddIncludes_IncludesIsNull_DoesNotCallIncludeOnTheQueryable()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyParent>> { CallBase = true };

            var mockDbSet = new Mock<DbSet<DummyParent>>();

            mockDbSet.Setup(x => x.Include(It.IsAny<string>()))
                .Returns<string>(x => mockDbSet.Object);

            // Act
            filterService.Object.AddIncludes(mockDbSet.Object, null, null);

            // Assert
            mockDbSet.Verify(mock => mock.Include(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void AddPaging_Count1Page0_ReturnsFirstElementOnly()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5 }, new DummyEntity { Id = 3 }, new DummyEntity { Id = 4 } }.AsQueryable();

            // Act
            var result = filterService.Object.AddPaging(queryable, new Paging { Count = 1, Page = 0 }, null);

            // Assert
            Assert.AreEqual(5, result.Single().Id);
        }

        [TestMethod]
        public void AddPaging_TotalOf3ElementsInListWithPagingCount2Page1_ReturnsOnlyTheThirdElement()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5 }, new DummyEntity { Id = 3 }, new DummyEntity { Id = 4 } }.AsQueryable();

            // Act
            var result = filterService.Object.AddPaging(queryable, new Paging { Count = 2, Page = 1 }, null);

            // Assert
            Assert.AreEqual(4, result.Single().Id);
        }

        [TestMethod]
        public void AddPaging_PagingCount2Page0_ReturnsOnlyTheFirstTwoElements()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5 }, new DummyEntity { Id = 3 }, new DummyEntity { Id = 4 } }.AsQueryable();

            // Act
            var result = filterService.Object.AddPaging(queryable, new Paging { Count = 2, Page = 0 }, null);

            // Assert
            Assert.AreEqual(5, result.First().Id);
            Assert.AreEqual(3, result.Last().Id);
        }

        [TestMethod]
        public void AddPaging_PagingIsNull_ReturnsAllRecords()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5 }, new DummyEntity { Id = 3 }, new DummyEntity { Id = 4 } }.AsQueryable();

            // Act
            var result = filterService.Object.AddPaging(queryable, null, null);

            // Assert
            Assert.AreEqual(queryable.Count(), result.Count());
        }

        [TestMethod]
        public void AddOrderBys_EmptyListOfOrderBys_ReturnsQueryableOrderedById()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var result = filterService.Object.AddOrderBys(queryable, new List<OrderBy>(), null);

            // Assert
            Assert.AreEqual(3, result.First().Id);
            Assert.AreEqual(5, result.Last().Id);
        }

        [TestMethod]
        public void AddOrderBys_ListWithOneOrderBy_ReturnsQueryableWithOrderBy()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var result = filterService.Object.AddOrderBys(queryable,
                new List<OrderBy> {new OrderBy {ColumnName = "Id"}}, null);

            // Assert
            Assert.AreEqual(3, result.First().Id);
            Assert.AreEqual(5, result.Last().Id);
        }

        [TestMethod]
        public void AddOrderBys_ListWithOneOrderByDescending_ReturnsQueryableWithOrderByDescending()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var result = filterService.Object.AddOrderBys(queryable,
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}}, null);

            // Assert
            Assert.AreEqual(5, result.First().Id);
            Assert.AreEqual(3, result.Last().Id);
        }

        [TestMethod]
        public void AddOrderBys_ListWithTwoOrderBys_ReturnsQueryableWithOrderBysApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEF", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var result = filterService.Object.AddOrderBys(queryable,
                new List<OrderBy>
                {
                    new OrderBy {ColumnName = "Name", Descending = true},
                    new OrderBy {ColumnName = "Id"}
                }, null);

            // Assert
            Assert.AreEqual("XYZ", result.First().Name);
            Assert.AreEqual("DEF", result.Last().Name);
            Assert.AreEqual(9, result.Last().Id);
        }

        [TestMethod]
        public void AutoFilter_IntegerPropertySetTo0_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Id = 0}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_IntegerPropertySetToNon0Value_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Id = 9}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().Id);
        }

        [TestMethod]
        public void AutoFilter_StringPropertySetToNullValue_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = null}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertySetToEmptyString_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = string.Empty}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = "DEF"}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }


        [TestMethod]
        public void AutoFilter_StringPropertySetToValue_FilterOnPropertyEqualsValueCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Name = "DEF", Id = 5}, new DummyEntity {Name = "DEFG", Id = 9}, new DummyEntity {Name = "XYZ"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = "DEF" }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_BooleanPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Enabled = true}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Enabled = false}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableBooleanPropertySetToNull_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Off = true}, new DummyEntity {Name = "DEFG", Id = 9, Off = false},
                new DummyEntity {Name = "XYZ", Off = false}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Off = null}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableBooleanPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Off = true}, new DummyEntity {Name = "DEFG", Id = 9, Off = false},
                new DummyEntity {Name = "XYZ", Off = false}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Off = false}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableIntPropertySetToNull_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Number = 100}, new DummyEntity {Name = "DEFG", Id = 9, Off = false, Number = 101},
                new DummyEntity {Name = "XYZ", Off = false}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Number = null}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableIntPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Number = 100}, new DummyEntity {Name = "DEFG", Id = 9, Off = false, Number = 101},
                new DummyEntity {Name = "XYZ", Off = false}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Number = 100}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_DateTimePropertySetToDateTimeMin_NoFilterAppliedForProperty()
        {
            var now = DateTime.UtcNow;
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Start = now}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Start = DateTime.MinValue}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_DateTimePropertySetToNonDateTimeMinValue_FilterOnPropertyEqualsValue()
        {
            var now = DateTime.UtcNow;
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Start = now}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Start = now}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableDateTimePropertySetToValue_FilterOnPropertyEqualsValue()
        {
            var now = DateTime.UtcNow;
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Finish = now}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Finish = now}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableDateTimePropertySetToNull_NoFilterAppliedForProperty()
        {
            var now = DateTime.UtcNow;
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Finish = now}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Finish = null}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_EnumPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Choices = Choices.Wisely}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Choices = Choices.Wisely}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableEnumPropertySetToValue_FilterOnPropertyEqualsValue()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, NullableChoices = Choices.Wisely}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false, NullableChoices = Choices.Poorly},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { NullableChoices = Choices.Wisely }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NullableEnumPropertySetToNull_NoFilterAppliedForProperty()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, NullableChoices = Choices.Wisely}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false, NullableChoices = Choices.Poorly},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { NullableChoices = null }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_VirtualPropertiesOnFilterAreIgnored()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Choices = Choices.Wisely}, new DummyEntity {Name = "DEFG", Id = 9, Enabled = false},
                new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {DummyChildren = new List<DummyChild>()}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_NotMappedAttributesOnFilterAreIgnored()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Name = "DEF", Id = 5, Choices = Choices.Wisely, NotMappedProperty = "test"},
                new DummyEntity {Name = "DEFG", Id = 9, Enabled = false}, new DummyEntity {Name = "XYZ"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {NotMappedProperty = "test"}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatchAndForceCaseInsensitiveMatch_ReturnsResultsThatContainTheFilterStringCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {PartialMatchingAndForceCaseInsensitiveMatchString = "Test"}, new DummyEntity {PartialMatchingAndForceCaseInsensitiveMatchString = "teST123"},
                new DummyEntity {PartialMatchingAndForceCaseInsensitiveMatchString = "tes"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { PartialMatchingAndForceCaseInsensitiveMatchString = "test"}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatchFalseAndForceCaseInsensitiveMatch_OnlyReturnsExactMatchesCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {NotPartialMatchingAndForceCaseInsensitiveMatchString = "Tes"}, new DummyEntity {NotPartialMatchingAndForceCaseInsensitiveMatchString = "teST123"},
                new DummyEntity {NotPartialMatchingAndForceCaseInsensitiveMatchString = "Test"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { NotPartialMatchingAndForceCaseInsensitiveMatchString = "test"}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatchAndForceCaseInsensitiveMatchFalse_ReturnsResultsThatContainTheFilterStringCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {PartialMatchingAndNotForceCaseInsensitiveMatchString = "Test"}, new DummyEntity {PartialMatchingAndNotForceCaseInsensitiveMatchString = "tesT123"},
                new DummyEntity {PartialMatchingAndNotForceCaseInsensitiveMatchString = "test"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { PartialMatchingAndNotForceCaseInsensitiveMatchString = "tes" }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatchFalseAndForceCaseInsensitiveMatchFalse_OnlyReturnsExactMatchesCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {NotPartialMatchingAndNotForceCaseInsensitiveMatchString = "Test"}, new DummyEntity {NotPartialMatchingAndNotForceCaseInsensitiveMatchString = "teST123"},
                new DummyEntity {NotPartialMatchingAndNotForceCaseInsensitiveMatchString = "test"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity { NotPartialMatchingAndNotForceCaseInsensitiveMatchString = "test" }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsDateTimeRangeWithNullValue_NoDateTimeRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Finish = DateTime.UtcNow}, new DummyEntity {Finish = DateTime.UtcNow.AddDays(5)}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {DummyFilterExtensions = new DummyFilterExtensions()}, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNullableDateTimeRangeWithValue_DateTimeRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Finish = DateTime.UtcNow}, new DummyEntity {Finish = DateTime.UtcNow.AddDays(5)}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                        {FinishRange = new DateTimeRange {After = DateTime.UtcNow.AddDays(3), Before = DateTime.UtcNow.AddDays(6)}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }


        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsDateTimeRangeWithValue_DateTimeRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Start = DateTime.UtcNow }, new DummyEntity { Start = DateTime.UtcNow.AddDays(5) } }
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                        { StartRange = new DateTimeRange { After = DateTime.UtcNow.AddDays(3), Before = DateTime.UtcNow.AddDays(6) } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsClassInheritingDateTimeRangeWithValue_DateTimeRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Start = DateTime.UtcNow }, new DummyEntity { Start = DateTime.UtcNow.AddDays(5) } }
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                        { InheritingStartRange = new InheritingDateTimeRange { After = DateTime.UtcNow.AddDays(3), Before = DateTime.UtcNow.AddDays(6) } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }


        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsDateTimeRangeWithValue_DateTimeRangeFilterIncludesBoundaries()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var dateTimeNow = DateTime.UtcNow;
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Finish = dateTimeNow}, new DummyEntity {Finish = dateTimeNow.AddDays(5)},
                new DummyEntity {Finish = dateTimeNow.AddDays(3)}, new DummyEntity {Finish = dateTimeNow.AddDays(4)}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions =
                        new DummyFilterExtensions
                        {
                            FinishRange = new DateTimeRange {After = dateTimeNow.AddDays(3), Before = dateTimeNow.AddDays(4)}
                        }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNumberRangeWithValueForNullableInt_NumberRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Number = 5}, new DummyEntity {Number = 3}, new DummyEntity {Number = null}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {NumberRange = new NumberRange {Min = 2, Max = 4}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNumberRangeWithValueForInt_NumberRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {IdRange = new NumberRange {Min = 2, Max = 4}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(1, results.Count());
        }
        
        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMinValueForEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, Choices = Choices.Poorly}, new DummyEntity {Id = 3, Choices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {ChoicesRange = new NumberRange {Min = (int) Choices.Poorly}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Poorly, results.Single().Choices);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMaxValueForEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, Choices = Choices.Poorly}, new DummyEntity {Id = 3, Choices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {ChoicesRange = new NumberRange {Max = (int) Choices.Wisely}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Wisely, results.Single().Choices);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMinAndMaxValueForEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, Choices = Choices.Poorly}, new DummyEntity {Id = 3, Choices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                        {ChoicesRange = new NumberRange {Min = (int) Choices.Wisely, Max = (int) Choices.Wisely}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Wisely, results.Single().Choices);
        }


        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMinValueForNullableEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, NullableChoices = Choices.Poorly}, new DummyEntity {Id = 3, NullableChoices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { NullableChoicesRange = new NumberRange { Min = (int)Choices.Poorly } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Poorly, results.Single().NullableChoices);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMaxValueForNullableEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, NullableChoices = Choices.Poorly}, new DummyEntity {Id = 3, NullableChoices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { NullableChoicesRange = new NumberRange { Max = (int)Choices.Wisely } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Wisely, results.Single().NullableChoices);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsEnumRangeWithMinAndMaxValueForNullableEnum_EnumRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, NullableChoices = Choices.Poorly}, new DummyEntity {Id = 3, NullableChoices = Choices.Wisely}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                    {
                        NullableChoicesRange = new NumberRange {Min = (int) Choices.Wisely, Max = (int) Choices.Wisely}
                    }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(Choices.Wisely, results.Single().NullableChoices);
        }


        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNullableTimeSpanRangeWithMinValueForTimeSpan_TimeSpanRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                    {new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(1)}, new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(3)}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { TimeOfDayRange = new TimeSpanRange { Min = TimeSpan.FromHours(2) } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(3), results.Single().TimeOfDay);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNullableTimeSpanRangeWithMaxValueForTimeSpan_TimeSpanRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                    {new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(1)}, new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(3)}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { TimeOfDayRange = new TimeSpanRange { Max = TimeSpan.FromHours(2) } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(1), results.Single().TimeOfDay);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsNullableTimeSpanRangeWithMinAndMaxValueForTimeSpan_TimeSpanRangeFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                    {new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(1)}, new DummyEntity {Id = 5, TimeOfDay = TimeSpan.FromHours(3)}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions
                    { TimeOfDayRange = new TimeSpanRange { Min = TimeSpan.FromHours(1), Max = TimeSpan.FromHours(2) } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(1), results.Single().TimeOfDay);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsCollectionContainsForIntProperty_CollectionContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5, FilterableInt = 7 }, new DummyEntity { Id = 6, FilterableInt = 9 } }
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { CollectionInts = new CollectionContains<int> { 9 } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableInt);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsCollectionContainsForLongProperty_CollectionContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5, FilterableLong = 7}, new DummyEntity {Id = 6, FilterableLong = 9}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {CollectionLongs = new CollectionContains<long> {9}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableLong);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsCollectionContainsForStringProperty_CollectionContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, FilterableString = "Something"}, new DummyEntity {Id = 6, FilterableString = "Else"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {CollectionStrings = new CollectionContains<string> {"Something"}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual("Something", results.Single().FilterableString);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsCollectionContainsForNullableIntProperty_CollectionContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, FilterableNullableInt = 7}, new DummyEntity {Id = 6, FilterableNullableInt = 9}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {CollectionNullableInts = new CollectionContains<int?> {9}}
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableNullableInt);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsListContainsForIntProperty_ListContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5, FilterableInt = 7 }, new DummyEntity { Id = 6, FilterableInt = 9 } }
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { ListInts = new ListContains<int> { 9 } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableInt);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsListContainsForLongProperty_ListContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> { new DummyEntity { Id = 5, FilterableLong = 7 }, new DummyEntity { Id = 6, FilterableLong = 9 } }
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { ListLongs = new ListContains<long> { 9 } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableLong);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsListContainsForStringProperty_ListContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, FilterableString = "Something"}, new DummyEntity {Id = 6, FilterableString = "Else"}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { ListStrings = new ListContains<string> { "Something" } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual("Something", results.Single().FilterableString);
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsListContainsForNullableIntProperty_ListContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
                {new DummyEntity {Id = 5, FilterableNullableInt = 7}, new DummyEntity {Id = 6, FilterableNullableInt = 9}}.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions { ListNullableInts = new ListContains<int?> { 9 } }
                }, It.IsAny<IContext>());

            // Assert
            Assert.AreEqual(9, results.Single().FilterableNullableInt);
        }

        [TestMethod]
        public void AddAggregationFilter_SingleGroupByAndPagingLargeEnoughToReturnAllResults_ReturnsAllResultsGroupedIntoMatchingGroups()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Id = 1, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5},
                new DummyEntity {Id = 2, Choices = Choices.Wisely, Enabled = true, FilterableInt = 4},
                new DummyEntity {Id = 3, Choices = Choices.Undefined, Enabled = false, FilterableInt = 4},
                new DummyEntity {Id = 4, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AddAggregationFilter(queryable, new List<string> { "FilterableInt" }, new Paging {Page = 0, Count = 20},
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}});

            // Assert
            Assert.IsTrue(results.AsQueryable().ToString()
                .Contains(
                    ".GroupBy(x => new CustomType(FilterableInt = x.FilterableInt), x => Convert(x.Id, Int64)).GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList()).OrderByDescending(x => x.Key).Skip(0).Take(20)"));
            Assert.AreEqual(2, results.Count()); // 2 Groups
            Assert.AreEqual(2, results.ToList()[0].SelectMany(x => x).Count()); // 1st Group contains 2 items [4,1] ordered desc
            Assert.AreEqual(4, results.ToList()[0].SelectMany(x => x).ToList()[0]);
            Assert.AreEqual(1, results.ToList()[0].SelectMany(x => x).ToList()[1]);
            Assert.AreEqual(2, results.ToList()[1].SelectMany(x => x).Count()); // 2nd Group contains 2 items [4,1] ordered desc
            Assert.AreEqual(3, results.ToList()[1].SelectMany(x => x).ToList()[0]);
            Assert.AreEqual(2, results.ToList()[1].SelectMany(x => x).ToList()[1]);
            Assert.AreEqual(4, results.ToList()[0].Key); // Groups are ordered in descending order of key
            Assert.AreEqual(3, results.ToList()[1].Key);
        }

        [TestMethod]
        public void AddAggregationFilter_TwoGroupBysAndPagingToReturnOnlyTwoResults_ReturnsFirstTwoGroupedResultsOnly()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Id = 1, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5},
                new DummyEntity {Id = 2, Choices = Choices.Wisely, Enabled = true, FilterableInt = 4},
                new DummyEntity {Id = 3, Choices = Choices.Undefined, Enabled = false, FilterableInt = 4},
                new DummyEntity {Id = 4, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AddAggregationFilter(queryable, new List<string> {"Choices", "FilterableInt"},
                new Paging {Page = 0, Count = 2},
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}});

            // Assert
            Assert.IsTrue(results.ToString()
                .Contains(
                    ".GroupBy(x => new CustomType(Choices = x.Choices, FilterableInt = x.FilterableInt), x => Convert(x.Id, Int64)).GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList()).OrderByDescending(x => x.Key).Skip(0).Take(2)"));
            Assert.AreEqual(2, results.Count()); // 2 Groups
            Assert.AreEqual(2, results.ToList()[0].SelectMany(x => x).Count()); // 1st Group contains 2 items [4,1] ordered desc
            Assert.AreEqual(4, results.ToList()[0].SelectMany(x => x).ToList()[0]);
            Assert.AreEqual(1, results.ToList()[0].SelectMany(x => x).ToList()[1]);
            Assert.AreEqual(4, results.ToList()[0].Key); // Groups are ordered in descending order of key
            Assert.AreEqual(3, results.ToList()[1].Key);
        }

        [TestMethod]
        public void AddAggregationFilter_TwoGroupBysAndPagingIsNull_ReturnsAllResultsGroupedIntoMatchingGroups()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity>
            {
                new DummyEntity {Id = 1, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5},
                new DummyEntity {Id = 2, Choices = Choices.Wisely, Enabled = true, FilterableInt = 4},
                new DummyEntity {Id = 3, Choices = Choices.Undefined, Enabled = false, FilterableInt = 4},
                new DummyEntity {Id = 4, Choices = Choices.Poorly, Enabled = true, FilterableInt = 5}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AddAggregationFilter(queryable, new List<string> { "Choices", "FilterableInt" },
                null,
                new List<OrderBy> { new OrderBy { ColumnName = "Id", Descending = true } });

            // Assert
            // Note that we cannot simply compare the results in C# because the default equality comparer will compare whether the CustomType objects
            // are the same instance.  Instead, we have to compare the query string
            Assert.IsTrue(results.ToString()
                .Contains(".GroupBy(x => new CustomType(Choices = x.Choices, FilterableInt = x.FilterableInt), x => Convert(x.Id, Int64)).GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList()).OrderByDescending(x => x.Key)"));
            Assert.AreEqual(3, results.Count()); // 3 Groups
            Assert.AreEqual(2, results.ToList()[0].SelectMany(x => x).Count()); // 1st Group contains 2 items [4,1] ordered desc
            Assert.AreEqual(4, results.ToList()[0].SelectMany(x => x).ToList()[0]);
            Assert.AreEqual(1, results.ToList()[0].SelectMany(x => x).ToList()[1]);
            Assert.AreEqual(4, results.ToList()[0].Key); // Groups are ordered in descending order of key
            Assert.AreEqual(3, results.ToList()[1].Key);
            Assert.AreEqual(2, results.ToList()[2].Key);
        }
    }
}