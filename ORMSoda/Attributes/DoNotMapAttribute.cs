using System;

namespace ORMSoda.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotMapAttribute : Attribute
    {
        public DoNotMapAttribute()
        {
        }
    }
}
