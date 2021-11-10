using System;

namespace ORMSoda.SourceGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotMapAttribute : Attribute
    {
        public DoNotMapAttribute()
        {
        }
    }
}
