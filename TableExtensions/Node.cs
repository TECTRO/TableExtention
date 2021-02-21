using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TableExtensions
{
    public class TableNode
    {
        public string Title { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return string.Join("; ", GetType().GetRuntimeProperties().Select(info => $"{info.Name}: {info.GetValue(this)}"));
        }
    }
}
