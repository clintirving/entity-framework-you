using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HypertableAttribute : Attribute
    {
        public string Name { get; set; }
    }
}