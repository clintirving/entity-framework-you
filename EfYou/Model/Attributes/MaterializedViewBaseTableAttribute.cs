using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MaterializedViewBaseTableAttribute : Attribute
    {
        public string Table { get; set; }
    }
}