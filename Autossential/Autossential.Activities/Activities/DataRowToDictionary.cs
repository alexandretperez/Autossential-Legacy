using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.DataRowToDictionary_DisplayName))]
    [LocalizedDescription(nameof(Resources.DataRowToDictionary_Description))]
    public class DataRowToDictionary : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.DataRowToDictionary_DataRow_DisplayName))]
        [LocalizedDescription(nameof(Resources.DataRowToDictionary_DataRow_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataRow> DataRow { get; set; }

        [LocalizedDisplayName(nameof(Resources.DataRowToDictionary_Dictionary_DisplayName))]
        [LocalizedDescription(nameof(Resources.DataRowToDictionary_Dictionary_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Dictionary<string, object>> Dictionary { get; set; }

        #endregion Properties

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DataRow == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, Resources.DataRowToDictionary_DataRow_DisplayName));
            if (Dictionary == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, Resources.DataRowToDictionary_Dictionary_DisplayName));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var dataRow = DataRow.Get(context);
            var dic = new Dictionary<string, object>();

            foreach (DataColumn col in dataRow.Table.Columns)
                dic.Add(col.ColumnName, dataRow[col.ColumnName]);

            // Outputs
            return (ctx) => Dictionary.Set(ctx, dic);
        }

        #endregion Protected Methods
    }
}