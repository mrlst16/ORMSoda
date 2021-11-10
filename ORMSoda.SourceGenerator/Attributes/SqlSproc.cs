using System;

namespace ORMSoda.SourceGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlSproc : Attribute
    {
        public string SprocName { get; set; }
        public Type ResponseType { get; set; }
    }
}
