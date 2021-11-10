using Microsoft.Extensions.Hosting;
using ORMSoda.SourceGenerator.Extensions;
using System.Threading.Tasks;
using System.Data;
using System;
using ORMSoda;
using System.Collections.Generic;
using ORMSoda.SourceGenerator;


namespace ORMSoda.Tests.TestProgram
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var host = Setup();
        }

        public static IHost Setup()
            => Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.RegisterORMSoda();
            })
            .Build();
    }
}
