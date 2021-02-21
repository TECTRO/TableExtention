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
                new TableNode { Title = "1",Value = 11 }, 
                new TableNode { Title = "2", Value = 22 },
                new TableNode { Title = "3", Value = 33 },
                new TableNode { Title = "4", Value = 44 }
            };

            var table2Plain1 = table2.ToPlainTable();
            var table2Plain2 = table2.ToPlainTable(true);

            Console.WriteLine(table2Plain1.ToSingleRowString());
            Console.WriteLine(table2Plain2.ToSingleRowString());

            var table3 = new List<List<TableNode>>
            {
                new List<TableNode>{ new TableNode { Title = "ID", Value = 1 }, new TableNode { Title = "path", Value = "sss\\sss\\sss" }, new TableNode { Title = "year", Value = 1394 } },
                new List<TableNode>{ new TableNode { Title = "ID", Value = 2 }, new TableNode { Title = "path", Value = "sss\\ttt\\sss" }, new TableNode { Title = "year", Value = 1394 } },
                new List<TableNode>{ new TableNode { Title = "ID", Value = 3 }, new TableNode { Title = "ID", Value = "sss\\sss\\ttt" }, new TableNode { Title = "year1", Value = 1339 } },
                new List<TableNode>{ new TableNode { Title = "ID", Value = 4 }, new TableNode { Title = "path", Value = "sss\\fff\\sss" }, new TableNode { Title = "year", Value = 1344 } },
                new List<TableNode>{ new TableNode { Title = "ID", Value = 5 }, new TableNode { Title = "path", Value = "fff\\sss\\sss" }, new TableNode { Title = "year", Value = 2394 }, new TableNode { Title = "another", Value = 2394 } },
            };

            table3 = table3.Add(new object[] { 1,2,3,4,5,6 },TableNodeExtender.AddMode.ByColumnsForLastRow).Select(nodes => nodes.ToList()).ToList();

            var table3Plain = table3.ToPlainTable();
            Console.WriteLine(table3Plain.ToSingleRowString(15));

        }
    }
}

/*


    using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TableLab
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

    public static class TableNodeExtender
    {
        #region Add
        public static IEnumerable<IEnumerable<TableNode>> Add(this IEnumerable<IEnumerable<TableNode>> table, IEnumerable<TableNode> nodes)
        {
            return table.Append(nodes);
        }
        public static IEnumerable<TableNode> Add(this IEnumerable<TableNode> table, IEnumerable<TableNode> nodes)
        {
            return table.Concat(nodes);
        }
        public static IEnumerable<TableNode> Add(this IEnumerable<TableNode> table, TableNode node)
        {
            return table.Append(node);
        }
        /////////////////////
        public static IEnumerable<IEnumerable<TableNode>> Add(this IEnumerable<IEnumerable<TableNode>> table, IEnumerable<(string, object)> nodes)
        {
            return table.Append(nodes.Select(node => new TableNode { Title = node.Item1, Value = node.Item2 }));
        }

        public enum AddMode
        {
            ByAllColumns,
            ByColumnsForLastRow
        }
        public static IEnumerable<IEnumerable<TableNode>> Add(this IEnumerable<IEnumerable<TableNode>> table, IEnumerable<object> nodes, AddMode mode = AddMode.ByAllColumns)
        {
            var tableOfNodes = table.ToList();
            IEnumerable<string> header = null;

            if (tableOfNodes.Count > 0)
            {
                switch (mode)
                {
                    case AddMode.ByAllColumns:
                        header = tableOfNodes.GetTitles();
                        break;
                    case AddMode.ByColumnsForLastRow:
                        header = tableOfNodes[tableOfNodes.Count - 1].Select(node => node.Title);
                        break;
                }

                if (!(header is null))
                {
                    var lineReady = nodes.Zip(header, (value, title) => new TableNode { Title = title, Value = value }).ToList();
                    return tableOfNodes.Append(lineReady);
                }
            }

            throw new ArgumentException("Возможно исходная таблица пуста. Не удалось получить заголовки");
        }

        #endregion

        #region GetTitles
        public static IEnumerable<string> GetTitles(this IEnumerable<TableNode> table)
        {
            return table.Select(node => node.Title).Distinct();
        }
        public static IEnumerable<string> GetTitles(this IEnumerable<IEnumerable<TableNode>> table)
        {
            return table.SelectMany(row => row.GetTitles()).Distinct();
        }
        #endregion

        #region ToPlainTable
        public static string[][] ToPlainTable(this TableNode table)
        {
            var result = new List<string[]> { new[] { table.Title }, new[] { table.Value.ToString() } };

            return result.ToArray();
        }
        public static string[][] ToPlainTable(this IEnumerable<TableNode> table, bool joinBySplitting = false)
        {
            var tableNodes = table.ToList();
            var result = new List<string[]>();

            var header = tableNodes.GetTitles().ToList();
            result.Add(header.ToArray());

            if (joinBySplitting)
            {
                var source = new Queue<TableNode>(tableNodes);

                var buildingLine = new List<TableNode>();

                string[] FormatRawToReady()
                {
                    return header.Select(title => buildingLine.Any(node => node.Title == title) ? buildingLine.First(node => node.Title == title).Value.ToString() : "").ToArray();
                }

                while (source.Any())
                {
                    var tempNode = source.Dequeue();

                    if (buildingLine.Any(node => node.Title == tempNode.Title))
                    {
                        result.Add(FormatRawToReady());
                        buildingLine.Clear();
                    }
                    buildingLine.Add(tempNode);
                }
                if (buildingLine.Any())
                    result.Add(FormatRawToReady());
            }
            else
            {
                foreach (var node in tableNodes)
                {
                    var line = header.Select(title => node.Title.Equals(title) ? node.Value.ToString() : "");
                    result.Add(line.ToArray());
                }
            }



            return result.ToArray();
        }

        public static string[][] ToPlainTable(this IEnumerable<IEnumerable<TableNode>> table, string joinSymb = "; ")
        {
            var tableNodes = table.ToList();
            var result = new List<string[]>();

            var header = tableNodes.GetTitles().ToList();
            result.Add(header.ToArray());

            foreach (var nodeRow in tableNodes)
            {
                var clearRow = nodeRow.GroupBy(node => node.Title).Select(nodes => nodes.Count() > 1 ? new TableNode { Title = nodes.Key, Value = string.Join(joinSymb, nodes.Select(node => node.Value)) } : nodes.First()).ToList();
                result.Add(header.Select(title => clearRow.Any(node => node.Title.Equals(title)) ? clearRow.First(node => node.Title.Equals(title)).Value.ToString() : "").ToArray());
            }

            return result.ToArray();
        }
        #endregion

        #region ToSingleRowString

        public static string ToSingleRowString(this string[][] table, int append = 13)
        {
            string TryFormat(string node)
            {
                var spacersCount = Math.Max(0, append - node.Length);
                var spacer = new string(new char[spacersCount].Select(c => ' ').ToArray());
                return spacer + node;
            }

            //string AdvancedTryFormat(string node) =>string.Join("", node.Split('\n').Select(TryFormat));

            return string.Join("\n", table.Select(row => string.Join("", row.Select(TryFormat))));
        }

        #endregion

        #region GetBy

        public static IEnumerable<TableNode> GetBy(this IEnumerable<TableNode> row, string title) => row.Where(node => node.Title.Equals(title));
        public static IEnumerable<IEnumerable<TableNode>> GetBy(this IEnumerable<IEnumerable<TableNode>> table, string title)=> table.Select(row=>row.GetBy(title));
        public static IEnumerable<TableNode> GetByFlat(this IEnumerable<IEnumerable<TableNode>> table, string title)=> table.SelectMany(row=>row.GetBy(title));

        #endregion

    }
}


 */
