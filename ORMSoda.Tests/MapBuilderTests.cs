using ORMSoda.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ORMSoda.Tests
{
    public class TCA
    {
        public int MyProperty { get; set; }
    }

    public class TestRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        [TableMapping("udt_TCA", 1)]
        public List<TCA> TCAs { get; set; }
    }

    public class TestResponse : TestRequest
    {
    }

    public class Mapper_FromTestRequestToTestResponse : SqlSprocBase<TestRequest, TestResponse>
    {
        public Mapper_FromTestRequestToTestResponse(string connectionString) : base(connectionString) { }
        public Mapper_FromTestRequestToTestResponse() { }
        public override async Task<TestResponse> Call(TestRequest request)
        {
            var parameters = new SqlParameter[]{
        new SqlParameter("@Id", request.Id),
        new SqlParameter("@Name", request.Name),
        new SqlParameter("@Password", request.Password),
        Map_TCAs_ToSqlParameters(request.TCAs),
};
            var dataSet = new DataSet();
            if (TryCallingSproc(dataSet, "TestSproc", parameters))
            {
                return await Map(dataSet);
            }
            return null;
        }
        protected override async Task<TestResponse> Map(DataSet dataSet)
        {
            var response = new TestResponse();
            var dataTable = dataSet.Tables[0];
            var row = dataTable.Rows[0];
            response.Id = GetValue<Int32>(row, "Id");
            response.Name = GetValue<String>(row, "Name");
            response.Password = GetValue<String>(row, "Password");
            response.TCAs = await MapTCAs_FromDataSet(dataSet.Tables[1]);
            return response;
        }
        protected SqlParameter Map_TCAs_ToSqlParameters(IEnumerable<TCA> enumeration)
        {
            var result = new SqlParameter();
            result.Value = Map_TCAs_ToDataTable(enumeration);
            result.ParameterName = "TCAs";
            result.TypeName = "TCAs_udt";
            return result;
        }
        protected DataTable Map_TCAs_ToDataTable(IEnumerable<TCA> enumeration)
        {
            var result = new DataTable();
            result.Columns.Add("MyProperty", typeof(Int32));
            foreach (var item in enumeration)
            {
                DataRow row = result.NewRow();
                row["MyProperty"] = item.MyProperty;
                result.Rows.Add(row);
            }
            return result;
        }
        public async Task<List<TCA>> MapTCAs_FromDataSet(DataTable table)
        {
            var response = new List<TCA>();
            foreach (DataRow row in table.Rows)
            {
                var item = new TCA();
                item.MyProperty = GetValue<Int32>(row, "MyProperty");

            }
            return response;
        }
    }


    public class MapBuilderTests
    {
        private readonly MapBuilder _mapBuilder;
        public MapBuilderTests()
        {
            _mapBuilder = new MapBuilder();
        }

        [Fact]
        public void CreateClass_ProperString()
        {
            StringBuilder sb = new StringBuilder();
            var str = _mapBuilder.CreateClass("TestSproc", typeof(TestRequest), typeof(TestResponse));
        }

        [Fact]
        public void CreateMethod_ProperString()
        {
            StringBuilder sb = new StringBuilder();
            _mapBuilder.Create_CallMethod(sb, "TestSproc", typeof(TestRequest), typeof(TestResponse));
            var str = sb.ToString();
        }

        [Fact]
        public void CreateDataTableUdt_ProperString()
        {
            StringBuilder sb = new StringBuilder();
            var dbToPropertyMap = new Dictionary<string, string>()
            {
                { "Id_db", "Id"},
                { "Name_db", "Name"},
                { "Password_db", "Password"}
            };

            _mapBuilder.Create_CreateUdtData(ref sb, typeof(TestRequest), "TestTable", dbToPropertyMap);
            var str = sb.ToString();
        }


        [Fact]
        public void CreateUdtSqlParameter_ProperString()
        {
            StringBuilder sb = new StringBuilder();

            _mapBuilder.Create_CreateUdtSqlParameters(ref sb, typeof(TestRequest), "TestTable");
            var str = sb.ToString();
        }


    }


}