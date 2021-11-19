using System;

namespace ORMSoda.SourceGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlSproc : Attribute
    {
        public string SprocName { get; protected set; }
        public Type ResponseType { get; protected set; }

        public SqlSproc(
            string sprocName,
            Type responseType
            )
        {
            SprocName = sprocName;
            ResponseType = responseType;
        }
    }
}
