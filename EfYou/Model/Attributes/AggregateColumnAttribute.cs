using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
    }
}