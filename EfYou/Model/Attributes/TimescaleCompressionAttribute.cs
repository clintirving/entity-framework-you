using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TimescaleCompressionAttribute : Attribute
    {
        public string SegmentBy { get; set; }

        public string OrderBy { get; set; }
    }
}