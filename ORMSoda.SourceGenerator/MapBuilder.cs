using ORMSoda.Attributes;
using ORMSoda.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ORMSoda
{
    public class MapBuilder
    {
        public MapBuilder()
        {
        }

        public string CreateClass(string sprocName, Type requestType, Type responseType)
        {
            var className = $"Mapper_From{requestType.Name}To{responseType.Name}";
            StringBuilder result = new StringBuilder();
            result.AppendLine($"public class {className}:SqlSprocBase<{requestType.Name}, {responseType.Name}>{{");
            Create_InheritedConstructors(result, className);
            Create_CallMethod(result, sprocName, requestType, responseType);
            Create_MapToResponseMethod(result, responseType);

            var properties = requestType.GetProperties();
            foreach (var property in properties)
            {
                var propertyGenericTypes = property.PropertyType.GenericTypeArguments;
                var firstGenericType = propertyGenericTypes?.FirstOrDefault();
                if (!property.WantsToBeMapped()) continue;

                switch (property.PropertyType)
                {
                    case Type t when t.GetInterface(nameof(IEnumerable)) != null
                                && (!firstGenericType?.IsValueType ?? false)
                                && firstGenericType != typeof(string)
                                && t != typeof(string)
                                && propertyGenericTypes.Length == 1:

                        var mapToTableAttribute = property.GetCustomAttribute<TableMapping>();

                        if (mapToTableAttribute != null)
                        {
                            Create_CreateUdtSqlParameters(ref result, firstGenericType, property.Name);
                            Create_CreateUdtData(ref result, firstGenericType, property.Name, GetPropertiesPrimitives(firstGenericType));
                            Create_MapToListOfPropertyMethod(result, firstGenericType, property);
                        }
                        break;
                    case Type t when t.IsClass
                                && (!firstGenericType?.IsValueType ?? false)
                                && firstGenericType != typeof(string)
                                && t != typeof(string):
                        Create_MapToListOfPropertyMethod(result, firstGenericType, property);
                        break;
                    default:
                        break;
                }
            }

            result.AppendLine($"}}");//End class
            return result.ToString();
        }

        private void IterateMappableProperties(
            Type type,
            Action<int, PropertyInfo> onEnumerable = null,
            Action<int, PropertyInfo> onObject = null,
            Action<int, PropertyInfo> onPrimitiveOrString = null,
            Action<int, PropertyInfo> onDefault = null
            )
        {
            var properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var propertyGenericTypes = property.PropertyType.GenericTypeArguments;
                var firstGenericType = propertyGenericTypes?.FirstOrDefault();
                if (!property.WantsToBeMapped()) continue;

                switch (property.PropertyType)
                {
                    case Type t when t.GetInterface(nameof(IEnumerable)) != null
                                && (!firstGenericType?.IsValueType ?? false)
                                && firstGenericType != typeof(string)
                                && t != typeof(string)
                                && propertyGenericTypes.Length == 1
                                && onEnumerable != null:
                        onEnumerable(i, property);
                        break;
                    case Type t when t.IsClass
                                && (!t.GenericTypeArguments?.FirstOrDefault()?.IsValueType ?? false)
                                && t.GenericTypeArguments?.FirstOrDefault() != typeof(string)
                                && t != typeof(string)
                                && onObject != null:
                        onObject(i, property);
                        break;
                    case Type t when (t.IsPrimitive || t == typeof(string)) && onPrimitiveOrString != null:
                        onPrimitiveOrString(i, property);
                        break;
                    default:
                        onPrimitiveOrString(i, property);
                        break;
                }
            }
        }

        public void Create_InheritedConstructors(StringBuilder result, string className)
        {
            result.AppendLine($"public {className}(string connectionString): base(connectionString){{ }}");
            result.AppendLine($"public {className}(){{ }}");
        }

        public string Create_CallMethod(StringBuilder result, string sprocName, Type requestType, Type responseType)
        {
            result.AppendLine($"public override async Task<{responseType.Name}> Call({requestType.Name} request){{");
            Create_SqlParameterArrayForSproc(result, requestType);
            result.AppendLine($"var dataSet = new DataSet();");
            result.AppendLine($"if(TryCallingSproc(dataSet, \"{sprocName}\", parameters)){{");
            result.AppendLine($"\treturn await Map(dataSet);");
            result.AppendLine($"}}");//End ifTryCallingSproc
            result.AppendLine($"\treturn null;");
            result.AppendLine("\t}");//End method
            return result.ToString();
        }

        public void Create_MapToResponseMethod(StringBuilder result, Type responseType)
        {
            result.AppendLine($"protected override async Task<{responseType.Name}> Map(DataSet dataSet){{");
            result.AppendLine($"var response = new {responseType.Name}();");
            result.AppendLine($"var dataTable = dataSet.Tables[0];");
            result.AppendLine($"var row = dataTable.Rows[0];");

            IterateMappableProperties(
                responseType,
                onPrimitiveOrString: (i, property) =>
                {
                    result.AppendLine($"response.{property.Name} = GetValue<{property.PropertyType.Name}>(row, \"{property.ORMSoda_DbFieldName()}\");");
                },
                onEnumerable: (i, property) =>
                {
                    result.AppendLine($"response.{property.Name} = await Map{property.Name}_FromDataSet(dataSet.Tables[{property.DataSetTableNumber()}]);");
                });

            result.AppendLine($"return response;");
            result.AppendLine("\t}");//End method
            result.ToString();
        }

        public void Create_MapToListOfPropertyMethod(StringBuilder result, Type genericType, PropertyInfo property)
        {
            result.AppendLine($"public async Task<List<{genericType.Name}>> Map{property.Name}_FromDataSet(DataTable table){{");
            result.AppendLine($"var response = new List<{genericType.Name}>();");
            result.AppendLine("foreach(DataRow row in table.Rows){");
            result.AppendLine($"var item = new {genericType.Name}();");
            IterateMappableProperties(
                genericType,
                onPrimitiveOrString: (i, property) =>
                {
                    result.AppendLine($"item.{property.Name} = GetValue<{property.PropertyType.Name}>(row, \"{property.ORMSoda_DbFieldName()}\");");
                    result.AppendLine();
                });
            result.AppendLine("\t\t}");//End foreach
            result.AppendLine("return response;");
            result.AppendLine("\t}");//End method
        }

        public void Create_SqlParameterArrayForSproc(StringBuilder result, Type type)
        {
            result.AppendLine("var parameters = new SqlParameter[]{");
            PropertyInfo[] propertyInfos = type.GetProperties();

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo property = propertyInfos[i];
                var propertyGenericTypes = property.PropertyType.GenericTypeArguments;
                var firstGenericType = propertyGenericTypes?.FirstOrDefault();

                switch (property.PropertyType)
                {
                    case Type t when t.GetInterface(nameof(IDictionary)) != null:
                        throw new NotSupportedException("Dictionaries are currently not supported");
                    case Type t when t.IsValueType || t == typeof(string):
                        result.AppendLine($"\t\tnew SqlParameter(\"{GetDbFieldName(property)}\", request.{property.Name}),");
                        break;
                    case Type t when t.GetInterface(nameof(IEnumerable)) != null:
                        result.AppendLine($"\t\tMap_{property.Name}_ToSqlParameters(request.{property.Name}),");
                        break;
                    case Type t when t == typeof(object):
                        result.AppendLine($"\t\tMap_{property.Name}_ToSqlParameters(new List<{firstGenericType.Name}>(){{request.{property.Name})}},");
                        break;
                    default:
                        break;
                }
            }
            result.Remove(result.Length - 1, 1);
            result.AppendLine("};");
        }

        public IDictionary<string, string> GetPropertiesPrimitives(Type type)
        {
            Dictionary<string, string> result = new();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                {
                    var dbFieldNameAttribute = property.PropertyType.GetCustomAttribute<DbFieldNameAttribute>();
                    var key = dbFieldNameAttribute?.FieldName ?? property.Name;
                    result[key] = property.Name;
                }
            }
            return result;
        }

        public void Create_CreateUdtSqlParameters(
            ref StringBuilder result,
            Type enumerationType,
            string tablename
            )
        {
            result.AppendLine($"protected SqlParameter Map_{tablename}_ToSqlParameters(IEnumerable<{enumerationType.Name}> enumeration)");
            result.AppendLine($"{{");
            result.AppendLine("var result = new SqlParameter();");
            result.AppendLine($"result.Value = Map_{tablename}_ToDataTable(enumeration);");
            result.AppendLine($"result.ParameterName = \"{tablename}\";");
            result.AppendLine($"result.TypeName = \"{tablename}_udt\";");
            result.AppendLine($"return result;");
            result.AppendLine($"}}"); // End method
        }


        public void Create_CreateUdtSqlParameters(
            StringBuilder result,
            string enumerationType,
            string parameterName,
            string tablename,
            string udtName
            )
        {
            result.AppendLine($"protected SqlParameter Map_{tablename}_ToSqlParameters(IEnumerable<{enumerationType}> enumeration)");
            result.AppendLine($"{{");
            result.AppendLine("var result = new SqlParameter();");
            result.AppendLine($"result.Value = Map_{tablename}_ToDataTable(enumeration);");
            result.AppendLine($"result.ParameterName = \"{parameterName}\";");
            result.AppendLine($"result.TypeName = \"{udtName}\";");
            result.AppendLine($"return result;");
            result.AppendLine($"}}"); // End method
        }

        public void Create_CreateUdtData(
            ref StringBuilder result,
            Type enumerationType,
            string tablename,
            IDictionary<string, string> dbToObjectFields
            )
        {
            result.AppendLine($"protected DataTable Map_{tablename}_ToDataTable(IEnumerable<{enumerationType.Name}> enumeration)");
            result.AppendLine($"{{");
            result.AppendLine("var result = new DataTable();");

            foreach (var key in dbToObjectFields.Keys)
            {
                var property = enumerationType.GetProperty(dbToObjectFields[key]);
                switch (property.PropertyType)
                {
                    case Type type when type == typeof(string):
                    case Type type2 when type2.IsValueType:
                        result.AppendLine($"result.Columns.Add(\"{key}\", typeof({property.PropertyType.Name}));");
                        break;
                    default:
                        break;
                }
            }

            result.AppendLine($"foreach(var item in enumeration){{");
            result.AppendLine($"DataRow row = result.NewRow();");

            foreach (var key in dbToObjectFields.Keys)
            {
                var property = enumerationType.GetProperty(dbToObjectFields[key]);
                switch (property.PropertyType)
                {
                    case Type type when type == typeof(string):
                    case Type type2 when type2.IsValueType:
                        result.AppendLine($"row[\"{key}\"]=item.{property.Name};");
                        break;
                    default:
                        break;
                }
            }

            result.AppendLine($"result.Rows.Add(row);");
            result.AppendLine($"}}"); //End foreach
            result.AppendLine($"return result;");
            result.AppendLine($"}}"); // End method
        }

        private string GetDbFieldName(PropertyInfo property)
            => $"@{property.GetCustomAttribute<DbFieldNameAttribute>()?.FieldName ?? property.Name}";
    }
}