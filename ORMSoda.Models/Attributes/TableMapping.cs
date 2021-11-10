using System;

namespace ORMSoda.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableMapping : Attribute
    {
        public string ForeignKey { get; protected set; }
        public string SourceTable { get; protected set; }
        public string UdtName { get; protected set; }
        public int DataSetTableNumber { get; protected set; }
        public TableMapping(
            string udtName,
            int dataSetTableNumber,
            string foreignKey = null,
            string sourceTable = null
            )
        {
            ForeignKey = foreignKey;
            SourceTable = sourceTable;
            UdtName = udtName;
            DataSetTableNumber = dataSetTableNumber;
        }
    }
}
