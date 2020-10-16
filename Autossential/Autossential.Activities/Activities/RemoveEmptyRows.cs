using Autossential.Activities.Properties;
using Autossential.Enums;
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
    [LocalizedDisplayName(nameof(Resources.RemoveEmptyRows_DisplayName))]
    [LocalizedDescription(nameof(Resources.RemoveEmptyRows_Description))]
    public class RemoveEmptyRows : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.RemoveEmptyRows_DataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.RemoveEmptyRows_DataTable_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataTable> DataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.RemoveEmptyRows_OutputDataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.RemoveEmptyRows_OutputDataTable_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> OutputDataTable { get; set; }

        [LocalizedCategory(nameof(Resources.Options_Category))]
        public DataRowValuesMode Mode { get; set; }

        [LocalizedCategory(nameof(Resources.RemoveEmptyRows_CustomOptions_Category))]
        public InArgument<int[]> ColumnIndexes { get; set; }

        [LocalizedCategory(nameof(Resources.RemoveEmptyRows_CustomOptions_Category))]
        public InArgument<string[]> ColumnNames { get; set; }

        [LocalizedCategory(nameof(Resources.RemoveEmptyRows_CustomOptions_Category))]
        public ConditionOperator Operator { get; set; }

        #endregion Properties

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DataTable)));
            if (OutputDataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(OutputDataTable)));
            if (ColumnIndexes != null && ColumnNames != null) metadata.AddValidationError(string.Format(Resources.ValidationExclusiveProperties_Error, nameof(ColumnIndexes), nameof(ColumnNames)));
            if (Mode == DataRowValuesMode.Custom && ColumnIndexes == null && ColumnNames == null) metadata.AddValidationError(string.Format(Resources.RemoveEmptyRows_CustomMode_Error, nameof(ColumnIndexes), nameof(ColumnNames)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // The logic here is inverted from workflow design.
            // In design time, we are asking for the rows to be removed.
            // Here the logic is to keep the rows in data table.
            // All becomes Any and Any becomes All.

            // Inputs
            var dt = DataTable.Get(context);

            bool predicate(object value) => value != DBNull.Value && !string.IsNullOrWhiteSpace(value?.ToString());

            Func<DataRow, bool> handler = (DataRow dr) => dr.ItemArray.Any(predicate);

            if (Mode == DataRowValuesMode.Any)
            {
                handler = (DataRow dr) => dr.ItemArray.All(predicate);
            }
            else if (Mode == DataRowValuesMode.Custom)
            {
                handler = GetCustomModeHandler(context, dt, predicate);
            }

            var rows = dt.AsEnumerable().Where(handler);
            var dtResult = rows.Any() ? rows.CopyToDataTable() : dt.Clone();

            // Outputs
            return (ctx) => OutputDataTable.Set(ctx, dtResult);
        }

        private Func<DataRow, bool> GetCustomModeHandler(AsyncCodeActivityContext context, DataTable dt, Func<object, bool> predicate)
        {
            var indexes = new HashSet<int>();
            var colIndexes = ColumnIndexes.Get(context);
            if (colIndexes != null)
            {
                foreach (var index in colIndexes)
                    indexes.Add(index);
            }
            else
            {
                var index = 0;
                var names = ColumnNames.Get(context);
                foreach (DataColumn col in dt.Columns)
                {
                    if (names.Contains(col.ColumnName))
                        indexes.Add(index);

                    index++;
                }
            }

            IEnumerable<object> filter(DataRow dr) => dr.ItemArray.Where((_, index) => indexes.Contains(index));

            if (Operator == ConditionOperator.And)
                return (DataRow dr) => filter(dr).Any(predicate);

            return (DataRow dr) => filter(dr).All(predicate);
        }

        #endregion Protected Methods
    }
}