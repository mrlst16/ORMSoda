using System;

namespace ORMSoda.Models
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