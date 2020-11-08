using System;
using System.Collections.Generic;
using System.Data;

namespace Autossential.Helpers
{
    public static class DataTableHelper
    {
        public static HashSet<int> IdentifyColumnIndexes(DataTable dataTable, int[] indexes, string[] names)
        {
            var result = new HashSet<int>();

            if (names != null)
            {
                foreach (var name in names)
                    result.Add(dataTable.Columns.IndexOf(name));
            }

            if (indexes != null)
            {
                foreach (var index in indexes)
                    result.Add(index);
            }

            if (result.Count == 0)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                    result.Add(i);
            }

            return result;
        }

        public static void ChangeColumnTypes(DataTable dataTable, IEnumerable<int> columnIndexes, Type dataType)
        {
            foreach (var colIndex in columnIndexes)
                dataTable.Columns[colIndex].DataType = dataType;
        }
    }
}