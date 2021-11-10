using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORMSoda.SourceGenerator.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendLines(this StringBuilder builder, IEnumerable<string> lines)
        {
            if (lines == null || !lines.Any()) return;
            for (int i = 0; i < lines.Count(); i++)
            {
                builder.Append(lines.ElementAt(i));
            }
        }
    }
}
