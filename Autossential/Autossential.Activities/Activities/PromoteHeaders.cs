using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Autossential.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Collections.Generic;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.PromoteHeaders_DisplayName))]
    [LocalizedDescription(nameof(Resources.PromoteHeaders_Description))]
    public class PromoteHeaders : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.PromoteHeaders_InputDataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.PromoteHeaders_InputDataTable_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataTable> InputDataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.PromoteHeaders_OutputDataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.PromoteHeaders_OutputDataTable_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> OutputDataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.PromoteHeaders_AutoRename_DisplayName))]
        [LocalizedDescription(nameof(Resources.PromoteHeaders_AutoRename_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public bool AutoRename { get; set; }

        #endregion


        #region Constructors

        public PromoteHeaders()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (InputDataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InputDataTable)));
            if (OutputDataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(OutputDataTable)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            var names = new Dictionary<string, int>();

            // Inputs
            var inputDT = InputDataTable.Get(context);
            if (inputDT.Rows.Count == 0)
                throw new InvalidOperationException(Resources.PromoteHeaders_EmptyRows_Error);

            var outputDT = inputDT.Copy();

            var row = outputDT.Rows[0];

            if (AutoRename)
            {
                foreach (DataColumn col in outputDT.Columns)
                {
                    var name = row[col.ColumnName].ToString();
                    if (names.ContainsKey(name))
                    {
                        names[name]++;
                        name += names[name].ToString();
                    }
                    else
                    {
                        names.Add(name, 0);
                    }

                    col.ColumnName = name;
                }
            }
            else
            {
                foreach (DataColumn col in outputDT.Columns)
                    col.ColumnName = row[col.ColumnName].ToString();
            }

            outputDT.Rows.Remove(row);

            // Outputs
            return (ctx) => OutputDataTable.Set(ctx, outputDT);
        }

        #endregion
    }
}

