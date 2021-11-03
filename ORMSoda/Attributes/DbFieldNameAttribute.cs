using System;

namespace ORMSoda.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbFieldNameAttribute : Attribute
    {
        public string FieldName { get; protected set; }

        public DbFieldNameAttribute(
            string fieldName
            )
        {
            FieldName = fieldName;
        }
    }
}