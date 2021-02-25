// // -----------------------------------------------------------------------
// // <copyright file="DummyFilterExtensions.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using Atlas.Model.Attributes;
using Atlas.Model.FilterExtensions;

namespace AtlasTests
{
    public class DummyFilterExtensions
    {
        [FilterExtensions(AppliedToProperty = "Start")]
        public DateTimeRange StartRange { get; set; }

        [FilterExtensions(AppliedToProperty = "Finish")]
        public DateTimeRange FinishRange { get; set; }

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
        public CollectionContains<int> FilterableInts { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableLong")]
        public CollectionContains<long> FilterableLongs { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableString")]
        public CollectionContains<string> FilterableStrings { get; set; }

        [FilterExtensions(AppliedToProperty = "FilterableNullableInt")]
        public CollectionContains<int?> FilterableNullableInts { get; set; }
    }
}