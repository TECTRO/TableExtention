using System;
using System.Collections.Generic;
using System.Linq;

namespace TableExtensions
{
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

        public static string[][] ToPlainTable(this IEnumerable<IEnumerable<TableNode>> table, string joinSymbol = "; ")
        {
            var tableNodes = table.ToList();
            var result = new List<string[]>();

            var header = tableNodes.GetTitles().ToList();
            result.Add(header.ToArray());

            foreach (var nodeRow in tableNodes)
            {
                var clearRow = nodeRow.GroupBy(node => node.Title).Select(nodes => nodes.Count() > 1 ? new TableNode { Title = nodes.Key, Value = string.Join(joinSymbol, nodes.Select(node => node.Value)) } : nodes.First()).ToList();
                result.Add(header.Select(title => clearRow.Any(node => node.Title.Equals(title)) ? clearRow.First(node => node.Title.Equals(title)).Value.ToString() : "").ToArray());
            }

            return result.ToArray();
        }
        #endregion

        #region Rotate

        public static IEnumerable<IEnumerable<TableNode>> Rotate(this IEnumerable<IEnumerable<TableNode>> table)
        {
            var result = new List<List<TableNode>>();
            var queueCollection = table.Select(nodes => new Queue<TableNode>(nodes)).ToList();
            while (queueCollection.Any(nodes => nodes.Any()))
                result.Add(
                    queueCollection.Select(nodes => 
                        nodes.Count>0? nodes.Dequeue():null
                    ).Where(node => node!=null)
                        .ToList()
                    );
            return result;
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
        public static IEnumerable<IEnumerable<TableNode>> GetBy(this IEnumerable<IEnumerable<TableNode>> table, string title) => table.Select(row => row.GetBy(title));
        public static IEnumerable<TableNode> GetByFlat(this IEnumerable<IEnumerable<TableNode>> table, string title) => table.SelectMany(row => row.GetBy(title));

        #endregion

    }
}
