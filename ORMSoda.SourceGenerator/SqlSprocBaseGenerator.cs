using Microsoft.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

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
            string source = $@"
            class Two{{
            
                public Two(){{
                    {Dump(context)}
                }}
            }}
";
            //string fileContents = sqlProcMapBuilder.CreateClass("TestSproc", typeof(TestRequest), typeof(TestResponse));
            context.AddSource("Blah", source);
            //context.AddSource("TestRequestMapper", fileContents);

        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //No code required yet

        }

        public string Dump(GeneratorExecutionContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in context.Compilation.Assembly.TypeNames)
            {
                stringBuilder.AppendLine($"System.Console.WriteLine(\"{item}\");");
            }
            return stringBuilder.ToString();
        }
    }
}