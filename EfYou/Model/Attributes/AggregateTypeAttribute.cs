using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateTypeAttribute : Attribute
    {
        public string AggregationFunction { get; set; }
    }
}