using ORMSoda.SourceGenerator.Attributes;
using System.Collections.Generic;

namespace ORMSoda.Tests.TestClasses
{
    public class TestRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        [TableMapping("udt_TCA", 1)]
        public List<TCA> TCAs { get; set; }
    }
}
