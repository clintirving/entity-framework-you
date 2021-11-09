using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TimescaleCompressionPolicyAttribute : Attribute
    {
        // Intervals being assigned in should match the form of:
        //      1 day
        //      2 months
        //      28 days
        //      14 years
        public string Interval { get; set; }
    }
}