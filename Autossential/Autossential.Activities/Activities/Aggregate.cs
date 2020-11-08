using Autossential.Activities.Properties;
using Autossential.Enums;
using Autossential.Helpers;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Aggregate_DisplayName))]
    [LocalizedDescription(nameof(Resources.Aggregate_Description))]
    public class Aggregate : ContinuableAsyncCodeActivity
    {
        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.Aggregate_ColumnIndexes_DisplayName))]
        [LocalizedDescription(nameof(Resources.Aggregate_ColumnIndexes_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int[]> ColumnIndexes { get; set; }

        [LocalizedDisplayName(nameof(Resources.Aggregate_ColumnNames_DisplayName))]
        [LocalizedDescription(nameof(Resources.Aggregate_ColumnNames_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<String[]> ColumnNames { get; set; }

        [LocalizedDisplayName(nameof(Resources.Aggregate_DataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.Aggregate_DataTable_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataTable> DataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.Aggregate_DataRow_DisplayName))]
        [LocalizedDescription(nameof(Resources.Aggregate_DataRow_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataRow> DataRow { get; set; }

        [LocalizedDisplayName(nameof(Resources.Aggregate_Function_DisplayName))]
        [LocalizedDescription(nameof(Resources.Aggregate_Function_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public AggregationFunction Function { get; set; } = AggregationFunction.Sum;

        public bool IgnoreEmptyValues { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DataTable == null) metadata.AddValidationError("DataTable cannot be null");
            if (ColumnIndexes != null && ColumnNames != null) metadata.AddValidationError(string.Format(Resources.ValidationExclusiveProperties_Error, nameof(ColumnIndexes), nameof(ColumnNames)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            var source = DataTable.Get(context);
            var result = source.NewRow();

            if (source.Rows.Count > 0)
            {
                var columnIndexes = DataTableHelper.IdentifyColumnIndexes(source, ColumnIndexes?.Get(context), ColumnNames?.Get(context));
                var convertibleValues = IdentifyConvertibleValues(source, columnIndexes);
                Compute(source.AsEnumerable(), result, convertibleValues);
            }

            // Outputs
            return (ctx) => DataRow.Set(ctx, result);
        }

        private void Compute(IEnumerable<DataRow> source, DataRow dr, Dictionary<int, AggregationFunction[]> convertibleValues)
        {
            foreach (var match in convertibleValues)
            {
                var values = source.Select(row => (dynamic)row[match.Key]);
                if (IgnoreEmptyValues)
                    values = values.Where(value => Equals(value, null) || Equals(value, ""));

                if (match.Value.Contains(Function))
                {
                    switch (Function)
                    {
                        case AggregationFunction.Average:
                            dr[match.Key] = values.Average(v => (double)v);
                            break;

                        case AggregationFunction.Max:
                            dr[match.Key] = (dynamic)values.Max(v => v);
                            break;

                        case AggregationFunction.Min:
                            dr[match.Key] = (dynamic)values.Min(v => v);
                            break;

                        case AggregationFunction.Sum:
                            dr[match.Key] = values.Sum(v => (double)v);
                            break;

                        case AggregationFunction.Median:
                            var arr = values.ToArray();
                            Array.Sort(arr);
                            int index = arr.Length / 2;
                            if (arr.Length % 2 == 0)
                                dr[match.Key] = (arr[index] + arr[index - 1]) / 2;
                            else
                                dr[match.Key] = arr[index];
                            break;

                        case AggregationFunction.DistinctCount:
                            dr[match.Key] = values.Distinct().Count();
                            break;
                    }
                }
            }
        }

        private Dictionary<int, AggregationFunction[]> IdentifyConvertibleValues(DataTable source, HashSet<int> columnIndexes)
        {
            var available = new Dictionary<int, AggregationFunction[]>();
            var rowIndex = 0;
            DataRow dr = source.Rows[rowIndex];
            foreach (var colIndex in columnIndexes)
            {
                switch (dr[colIndex])
                {
                    case int _:
                    case double _:
                    case byte _:
                    case float _:
                    case decimal _:
                        available.Add(colIndex, new[] {
                                AggregationFunction.Sum,
                                AggregationFunction.Average,
                                AggregationFunction.Min,
                                AggregationFunction.Max,
                                AggregationFunction.Median,
                                AggregationFunction.DistinctCount
                            });
                        break;

                    case DateTime _:
                        available.Add(colIndex, new[]
                        {
                                AggregationFunction.Min,
                                AggregationFunction.Max,
                                AggregationFunction.DistinctCount
                            });
                        break;

                    default:
                        available.Add(colIndex, new[] { AggregationFunction.DistinctCount });
                        break;
                }
            }
            return available;
        }
    }
}