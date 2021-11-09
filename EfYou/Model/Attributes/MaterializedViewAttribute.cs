using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MaterializedViewAttribute : Attribute
    {
        public string Name { get; set; }
    }
}