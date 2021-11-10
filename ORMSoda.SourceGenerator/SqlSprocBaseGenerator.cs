using Microsoft.CodeAnalysis;
using System.Linq;

namespace ORMSoda.SourceGenerator
{
    [Generator]
    public class SqlSprocBaseGenerator : ISourceGenerator
    {

        public static void Main(string[] args)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            SqlSprocMapBuilder sqlProcMapBuilder = new SqlSprocMapBuilder();



            //string fileContents = sqlProcMapBuilder.CreateClass("TestSproc", typeof(TestRequest), typeof(TestResponse));
            //context.AddSource("TestRequestMapper", fileContents);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //No code required yet
        }
    }
}