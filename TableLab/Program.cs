using System;
using System.Collections.Generic;
using System.Linq;
using TableExtensions;

namespace TableLab
{
    class Program
    {
        static void Main(string[] args)
        {
            var table1 = new TableNode
            {
                Title = "some str",
                Value = "test test"
            };

            var table1Plain = table1.ToPlainTable();
            Console.WriteLine(table1Plain.ToSingleRowString());

            var table2 = new List<TableNode>
            {
                new TableNode {Title = "1", Value = 11},
                new TableNode {Title = "2", Value = 22},
                new TableNode {Title = "3", Value = 33},
                new TableNode {Title = "4", Value = 44}
            };

            var table2Plain1 = table2.ToPlainTable();
            var table2Plain2 = table2.ToPlainTable(true);

            Console.WriteLine(table2Plain1.ToSingleRowString());
            Console.WriteLine(table2Plain2.ToSingleRowString());

            var table3 = new List<List<TableNode>>
            {
                new List<TableNode>
                {
                    new TableNode {Title = "ID", Value = 1}, new TableNode {Title = "path", Value = "sss\\sss\\sss"},
                    new TableNode {Title = "year", Value = 1394}
                },
                new List<TableNode>
                {
                    new TableNode {Title = "ID", Value = 2}, new TableNode {Title = "path", Value = "sss\\ttt\\sss"},
                    new TableNode {Title = "year", Value = 1394}
                },
                new List<TableNode>
                {
                    new TableNode {Title = "ID", Value = 3}, new TableNode {Title = "ID", Value = "sss\\sss\\ttt"},
                    new TableNode {Title = "year1", Value = 1339}
                },
                new List<TableNode>
                {
                    new TableNode {Title = "ID", Value = 4}, new TableNode {Title = "path", Value = "sss\\fff\\sss"},
                    new TableNode {Title = "year", Value = 1344}
                },
                new List<TableNode>
                {
                    new TableNode {Title = "ID", Value = 5}, new TableNode {Title = "path", Value = "fff\\sss\\sss"},
                    new TableNode {Title = "year", Value = 2394}, new TableNode {Title = "another", Value = 2394}
                },
            };

            table3 = table3.Add(new object[] {1, 2, 3, 4, 5, 6}, TableNodeExtender.AddMode.ByColumnsForLastRow)
                .Select(nodes => nodes.ToList()).ToList();

            var table4 = new[]
            {
                new[]
                {
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                    new TableNode {Title = "col1", Value = "sss"},
                },
                new[]
                {
                    new TableNode {Title = "col2", Value = "aaaaa"},
                    new TableNode {Title = "col2", Value = "aaaaa"},
                    new TableNode {Title = "col2", Value = "aaaaa"},
                    new TableNode {Title = "col2", Value = "aaaaa"},
                    new TableNode {Title = "col2", Value = "aaaaa"},
                }
            };

            var r  = table4.Rotate();

            var table3Plain = table3.ToPlainTable();
            Console.WriteLine(table3Plain.ToSingleRowString(15));
        }
    }
}