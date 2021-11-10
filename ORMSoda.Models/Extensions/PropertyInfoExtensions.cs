using System.Reflection;

namespace ORMSoda.Models.Extensions
{
    public static class PropertyInfoExtensions
    {

        public static string ORMSoda_DbFieldName(this PropertyInfo property)
            => property.GetCustomAttribute<DbFieldNameAttribute>()?.FieldName ?? property.Name;

        public static int DataSetTableNumber(this PropertyInfo property)
            => property.GetCustomAttribute<TableMapping>()?.DataSetTableNumber ?? 0;
        public static bool WantsToBeMapped(this PropertyInfo property)
            => property.GetCustomAttribute<DoNotMapAttribute>() == null;
    }
}
