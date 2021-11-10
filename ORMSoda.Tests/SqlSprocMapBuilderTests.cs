using ORMSoda.SourceGenerator;
using ORMSoda.Tests.TestClasses;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TestRequest = ORMSoda.Tests.TestClasses.TestRequest;

namespace ORMSoda.Tests
{
    public class SqlSprocMapBuilderTests
    {
        private readonly SqlSprocMapBuilder _mapBuilder;
        public SqlSprocMapBuilderTests()
        {
            _mapBuilder = new SqlSprocMapBuilder();
        }

        [Fact]
        public void CreateClass_ProperString()
        {
            StringBuilder sb = new StringBuilder();
            var str = _mapBuilder.CreateClass("TestSproc", typeof(TestClasses.TestRequest), typeof(TestClasses.TestResponse));
        }

        [Fact]
        public void CreateMethod_ProperString()
        {
            StringBuilder sb = new StringBuilder();
            _mapBuilder.Create_CallMethod(sb, "TestSproc", typeof(TestClasses.TestRequest), typeof(TestClasses.TestResponse));
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