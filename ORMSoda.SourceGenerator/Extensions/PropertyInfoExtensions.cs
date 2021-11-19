using ORMSoda.SourceGenerator.Attributes;
using System;
using System.Reflection;

namespace ORMSoda.SourceGenerator.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static string ORMSoda_DbFieldName(this PropertyInfo property)
            => property.GetCustomAttribute<DbFieldNameAttribute>()?.FieldName ?? property.Name;

        public static int DataSetTableNumber(this PropertyInfo property)
            => property.GetCustomAttribute<TableMapping>()?.DataSetTableNumber ?? 0;

        public static bool WantsToBeMapped(this PropertyInfo property)
            => property.GetCustomAttribute<DoNotMapAttribute>() == null;

        public static (string, Type) SprocNameAndResponseType(this Type classType)
        {
            if (!classType.IsClass) return (null, null);
            var attr = classType.GetCustomAttribute<SqlSproc>();
            return (attr.SprocName, attr.ResponseType);
        }
    }
}