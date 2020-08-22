// // -----------------------------------------------------------------------
// // <copyright file="FilterServiceOfTTests.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using EfYouCore.Filters;
using EfYouCore.Model.FilterExtensions;
using EfYouTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EfYouCoreTests.Filters
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
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<long> { 2 });

            // Assert
            Assert.AreEqual(1, results.Count()); // Only one result should pass the above defined filter on filter on ids.
            Assert.AreEqual(2, results.Single().Id); // The single result should have Id = 2.
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsInt_ReturnsQueryableFilteredByIntIds()
        {
            // Arrange
            var filterService = GetFilterServiceMock();

            var queryable = new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<long> {2});

            // Assert
            Assert.AreEqual(1, results.Count()); // Only one result should pass the above defined filter on filter on ids.
            Assert.AreEqual(2, results.Single().Id); // The single result should have Id = 2.
        }

        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsLong_ReturnsQueryableFilteredByLongIds()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyParent>> { CallBase = true };

            var queryable = new List<DummyParent> { new DummyParent { Id = 1 }, new DummyParent { Id = 2 }, new DummyParent { Id = 3 } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<long> { 2 });

            // Assert
            Assert.AreEqual(1, results.Count()); // Only one result should pass the above defined filter on filter on ids.
            Assert.AreEqual(2, results.Single().Id); // The single result should have Id = 2.
        }

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod]
        public void FilterResultsOnGet_PrimaryKeyOfTypeIsNoneOfShortOrIntOrLong_ThrowsApplicationException()
        {
            // Arrange
            var filterService = new Mock<FilterService<DummyEntityWithInvalidIdType>> { CallBase = true };

            var queryable = new List<DummyEntityWithInvalidIdType> { new DummyEntityWithInvalidIdType { Id = true }, new DummyEntityWithInvalidIdType { Id = false }, new DummyEntityWithInvalidIdType { Id = false } }.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnGet(queryable, new List<long> { 2 });

            // Assert
            // By Exception
        }
        
        [TestMethod]
        public void FilterResultsOnSearch_CallsFilterResultsOnSearchFilter()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>().AsQueryable();
            var filter = new DummyEntity();

            // Act
            filterService.Object.FilterResultsOnSearch(queryable, filter);

            // Assert
            filterService.Verify(x => x.FilterResultsOnSearchFilter(queryable, filter));
        }

        [TestMethod]
        public void FilterResultsOnSearch_ReturnsQueryableFilteredBySearchFilter()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            filterService.Setup(x => x.FilterResultsOnSearchFilter(It.IsAny<IQueryable<DummyEntity>>(), It.IsAny<DummyEntity>()))
                .Returns<IQueryable<DummyEntity>, DummyEntity>((x, y) => x.Where(z => z.Id == 2));

            var queryable = new List<DummyEntity> {new DummyEntity {Id = 1}, new DummyEntity {Id = 2}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var results = filterService.Object.FilterResultsOnSearch(queryable, new DummyEntity());

            // Assert
            Assert.AreEqual(1, results.Count()); // Only one result should pass the above defined filter on current principal and filter on ids.
            Assert.AreEqual(2, results.Single().Id); // The single result should have Id = 2.
        }

        [TestMethod]
        public void AddOrderBys_EmptyListOfOrderBys_ReturnsQueryableOrderedById()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5}, new DummyEntity {Id = 3}}.AsQueryable();

            // Act
            var result = filterService.Object.AddOrderBys(queryable, new List<OrderBy>());

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
                new List<OrderBy> {new OrderBy {ColumnName = "Id"}});

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
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}});

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
                });

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Id = 0});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Id = 9});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = null});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = string.Empty});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = "DEF"});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Name = "Def"});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Enabled = false});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Off = null});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Off = false});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Number = null});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Number = 100});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Start = DateTime.MinValue});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Start = now});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Finish = now});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Finish = null});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {Choices = Choices.Wisely});

            // Assert
            Assert.AreEqual(1, results.Count());
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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {DummyChildren = new List<DummyChild>()});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {NotMappedProperty = "test"});

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatch_ReturnsResultsThatContainTheFilterStringCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {PartialMatchingString = "Test"}, new DummyEntity {PartialMatchingString = "teST123"},
                new DummyEntity {PartialMatchingString = "tes"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {PartialMatchingString = "test"});

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_StringPropertyOnFilterWithAttributeAllowPartialStringMatchFalse_OnlyReturnsExactMatchesCaseInsensitive()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity>
            {
                new DummyEntity {NotPartialMatchingString = "Tes"}, new DummyEntity {NotPartialMatchingString = "teST123"},
                new DummyEntity {NotPartialMatchingString = "Test"}
            }.AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {NotPartialMatchingString = "test"});

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
            var results = filterService.Object.AutoFilter(queryable, new DummyEntity {DummyFilterExtensions = new DummyFilterExtensions()});

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsDateTimeRangeWithValue_DateTimeRangeFilterApplied()
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
                });

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
                });

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
                });

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
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {ChoicesRange = new EnumRange {Min = (int) Choices.Poorly}}
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {ChoicesRange = new EnumRange {Max = (int) Choices.Wisely}}
                });

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
                        {ChoicesRange = new EnumRange {Min = (int) Choices.Wisely, Max = (int) Choices.Wisely}}
                });

            // Assert
            Assert.AreEqual(Choices.Wisely, results.Single().Choices);
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
                    DummyFilterExtensions = new DummyFilterExtensions {TimeOfDayRange = new TimeSpanRange {Min = TimeSpan.FromHours(2)}}
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {TimeOfDayRange = new TimeSpanRange {Max = TimeSpan.FromHours(2)}}
                });

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
                        {TimeOfDayRange = new TimeSpanRange {Min = TimeSpan.FromHours(1), Max = TimeSpan.FromHours(2)}}
                });

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(1), results.Single().TimeOfDay);
        }


        [TestMethod]
        public void AutoFilter_FilterExtensionsContainsCollectionContainsForIntProperty_CollectionContainsFilterApplied()
        {
            // Arrange
            var filterService = GetFilterServiceMock();
            var queryable = new List<DummyEntity> {new DummyEntity {Id = 5, FilterableInt = 7}, new DummyEntity {Id = 6, FilterableInt = 9}}
                .AsQueryable();

            // Act
            var results = filterService.Object.AutoFilter(queryable,
                new DummyEntity
                {
                    DummyFilterExtensions = new DummyFilterExtensions {FilterableInts = new CollectionContains<int> {9}}
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {FilterableLongs = new CollectionContains<long> {9}}
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {FilterableStrings = new CollectionContains<string> {"Something"}}
                });

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
                    DummyFilterExtensions = new DummyFilterExtensions {FilterableNullableInts = new CollectionContains<int?> {9}}
                });

            // Assert
            Assert.AreEqual(9, results.Single().FilterableNullableInt);
        }

        [TestMethod]
        public void AddAggregationFilter_FourDummyObjectsWithOneGroupBy_ReturnsQueryableWithCorrectExpressionTree()
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
            var results = filterService.Object.AddAggregationFilter(queryable, new List<string> {"Choices"}, new Paging {Page = 0, Count = 20},
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}});

            // Assert
            Assert.IsTrue(results.AsQueryable().ToString()
                .Contains(
                    ".GroupBy(x => new CustomType(Choices = x.Choices), x => Convert(x.Id, Int64)).GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList()).OrderByDescending(x => x.Key).Skip(0).Take(20)"));
        }

        [TestMethod]
        public void AddAggregationFilter_FourDummyObjectsWithTwoGroupBys_ReturnsQueryableWithCorrectExpressionTree()
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
                new Paging {Page = 1, Count = 12},
                new List<OrderBy> {new OrderBy {ColumnName = "Id", Descending = true}});

            // Assert
            Assert.IsTrue(results.ToString()
                .Contains(
                    ".GroupBy(x => new CustomType(Choices = x.Choices, FilterableInt = x.FilterableInt), x => Convert(x.Id, Int64)).GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList()).OrderByDescending(x => x.Key).Skip(12).Take(12)"));
        }
    }
}