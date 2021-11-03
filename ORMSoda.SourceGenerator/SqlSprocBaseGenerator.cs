using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.IO;

namespace ORMSoda.SourceGenerator
{
    [Generator]
    public class SqlSprocBaseGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //System.Diagnostics.Debug.WriteLine("Here");
            //SqlSprocMapBuilder sqlProcMapBuilder = new SqlSprocMapBuilder();
            //string fileContents = sqlProcMapBuilder.CreateClass("TestSproc", typeof(TestRequest), typeof(TestResponse));
            //context.AddSource("TestRequestMapper", fileContents);
            //File.WriteAllText("c:\\nuget", fileContents);

            string source = $@"
    namespace ORMSoda.Tests.TestProgram {{
    public static partial class Program
    {{
        static partial void HelloFrom(string name)
        {{
            System.Console.WriteLine($""Generator says: Hi from '{{name}}'"");
        }}
    }}
}}
";
            context.AddSource("generatedSourceCode", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //No code required yet
        }
    }
}