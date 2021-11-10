using System;

namespace ORMSoda.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotMapAttribute : Attribute
    {
        public DoNotMapAttribute()
        {
        }
    }
}
