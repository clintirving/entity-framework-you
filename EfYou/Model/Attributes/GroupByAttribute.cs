using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupByAttribute : Attribute
    {
        public bool Descending { get; set; } = false;
    }
}