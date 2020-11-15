using System.Collections.Generic;
using System.Data;

namespace Autossential.Helpers
{
    public static class DataTableHelper
    {
        public static HashSet<DataColumn> IdentifyDataColumns(DataTable dataTable, int[] indexes, string[] names)
        {
            var result = new HashSet<DataColumn>();

            if (indexes != null)
            {
                foreach (int index in indexes)
                    result.Add(dataTable.Columns[index]);
            }

            if (names != null)
            {
                foreach (string name in names)
                    result.Add(dataTable.Columns[name]);
            }

            if (result.Count == 0)
            {
                foreach (DataColumn col in dataTable.Columns)
                    result.Add(col);
            }

            return result;
        }


        public static DataTable NewCryptoDataTable(DataTable sourceDataTable, HashSet<DataColumn> cryptoColumns)
        {
            var result = new DataTable();
            foreach (DataColumn col in sourceDataTable.Columns)
            {
                if (cryptoColumns.Contains(col))
                    result.Columns.Add(col.ColumnName, typeof(string));
                else
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            return result;
        }

        public static Dictionary<string, object> ColumnsToDictionary(DataTable dataTable)
        {
            var result = new Dictionary<string, object>();
            foreach (DataColumn col in dataTable.Columns)
                result.Add(col.ColumnName, null);

            return result;
        }
    }
}