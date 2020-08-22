using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autossential.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Data;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.DictionaryToDataTable_DisplayName))]
    [LocalizedDescription(nameof(Resources.DictionaryToDataTable_Description))]
    public class DictionaryToDataTable : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.DictionaryToDataTable_Dictionary_DisplayName))]
        [LocalizedDescription(nameof(Resources.DictionaryToDataTable_Dictionary_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<Dictionary<string, object>> Dictionary { get; set; }


        [LocalizedDisplayName(nameof(Resources.DictionaryToDataTable_DataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.DictionaryToDataTable_DataTable_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> DataTable { get; set; }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Dictionary == null)
            {
                metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, Resources.DictionaryToDataTable_Dictionary_DisplayName));
            }

            if (DataTable == null)
            {
                metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, Resources.DictionaryToDataTable_DataTable_DisplayName));
            }

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var dic = Dictionary.Get(context);

            var table = new DataTable();
            if (dic.Count > 0)
            {
                foreach (var item in dic)
                    table.Columns.Add(item.Key, item.Value == null || item.Value == DBNull.Value ? typeof(object) : item.Value.GetType());

                var row = table.NewRow();
                foreach (var item in dic)
                    row[item.Key] = item.Value;

                table.Rows.Add(row);
            }

            // Outputs
            return (ctx) => DataTable.Set(ctx, table);
        }

        #endregion
    }
}

