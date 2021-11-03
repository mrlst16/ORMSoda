using System;
using System.Threading.Tasks;

namespace ORMSoda.Tests.TestProgram
{
    public partial class Program
    {
        static partial void HelloFrom(string name);
        public static async Task Main(string[] args)
        {
            HelloFrom("Balls");
        }
    }
}
