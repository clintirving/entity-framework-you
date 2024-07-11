// // -----------------------------------------------------------------------
// // <copyright file="DummyFilterExtensions.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using EfYou.Model.Attributes;
using EfYou.Model.FilterExtensions;

namespace EfYouTests
{
    public class DummyFilterExtensions
    {
        [FilterExtensions(AppliedToProperty = "Start")]
        public DateTimeRange StartRange { get; set; }

        [FilterExtensions(AppliedToProperty = "StartOffset")]
        public DateTimeOffsetRange StartOffsetRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Start")]
        public InheritingDateTimeRange InheritingStartRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Finish")]
        public DateTimeRange FinishRange { get; set; }

        [FilterExtensions(AppliedToProperty = "FinishOffset")]
        public DateTimeOffsetRange FinishOffsetRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Number")]
        public NumberRange NumberRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Id")]
        public NumberRange IdRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Choices")]
        public NumberRange ChoicesRange { get; set; }

        [FilterExtensions(AppliedToProperty = "NullableChoices")]
        public NumberRange NullableChoicesRange { get; set; }

        [FilterExtensions(AppliedToProperty = "TimeOfDay")]
        public TimeSpanRange TimeOfDayRange { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableInt")]
        public CollectionContains<int> CollectionInts { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableLong")]
        public CollectionContains<long> CollectionLongs { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableString")]
        public CollectionContains<string> CollectionStrings { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableNullableInt")]
        public CollectionContains<int?> CollectionNullableInts { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableInt")]
        public ListContains<int> ListInts { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableLong")]
        public ListContains<long> ListLongs { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableString")]
        public ListContains<string> ListStrings { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableNullableInt")]
        public ListContains<int?> ListNullableInts { get; set; }
    }
}