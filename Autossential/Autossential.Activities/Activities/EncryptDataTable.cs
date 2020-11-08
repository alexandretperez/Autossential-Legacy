using Autossential.Activities.Properties;
using Autossential.Helpers;
using Autossential.Security;
using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.EncryptDataTable_DisplayName))]
    [LocalizedDescription(nameof(Resources.EncryptDataTable_Description))]
    public class EncryptDataTable : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_Algorithm_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_Algorithm_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public SymmetricAlgorithms Algorithm { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_DataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_DataTable_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<DataTable> DataTable { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_ColumnIndexes_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_ColumnIndexes_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int[]> ColumnIndexes { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_ColumnNames_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_ColumnNames_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string[]> ColumnNames { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_Key_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_Key_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Key { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_Encoding_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_Encoding_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<Encoding> Encoding { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_Iterations_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_Iterations_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> Iterations { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptDataTable_ParallelProcessing_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptDataTable_ParallelProcessing_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<bool> ParallelProcessing { get; set; }

        #endregion Properties

        #region Constructors

        public EncryptDataTable()
        {
            Encoding = new VisualBasicValue<Encoding>($"{typeof(Encoding).FullName}.{nameof(System.Text.Encoding.UTF8)}");
            Iterations = new VisualBasicValue<int>("1000");
            ParallelProcessing = new VisualBasicValue<bool>("False");
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DataTable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DataTable)));
            if (Key == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Key)));
            if (Encoding == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Encoding)));
            if (Iterations == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Iterations)));
            if (ColumnIndexes != null && ColumnNames != null) metadata.AddValidationError(string.Format(Resources.ValidationExclusiveProperties_Error, nameof(ColumnIndexes), nameof(ColumnNames)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var key = Key.Get(context);
            var encoding = Encoding.Get(context);
            var iterations = Iterations.Get(context);

            var inDt = DataTable.Get(context);
            var columnIndexes = DataTableHelper.IdentifyColumnIndexes(inDt, ColumnIndexes?.Get(context), ColumnNames?.Get(context));

            // Configs output datatable
            var outDt = inDt.Clone();
            DataTableHelper.ChangeColumnTypes(outDt, columnIndexes, typeof(object));

            using (var crypto = new Crypto(Algorithm, encoding, iterations))
            {
                outDt.BeginLoadData();
                if (ParallelProcessing?.Get(context) == true)
                {
                    Parallel.ForEach(inDt.AsEnumerable(), (DataRow row) => AddToDataTable(key, columnIndexes, outDt, crypto, row));
                }
                else
                {
                    foreach (DataRow row in inDt.Rows)
                        AddToDataTable(key, columnIndexes, outDt, crypto, row);
                }


                outDt.AcceptChanges();
                outDt.EndLoadData();
            }

            // Outputs
            return (ctx) => Result.Set(ctx, outDt);
        }

        private static void AddToDataTable(string key, HashSet<int> columnIndexes, DataTable outDt, Crypto crypto, DataRow row)
        {
            var values = new object[row.ItemArray.Length];
            Array.Copy(row.ItemArray, 0, values, 0, values.Length);

            foreach (var colIndex in columnIndexes)
            {
                var value = values[colIndex];
                if (value == null || value == DBNull.Value)
                    continue;

                values[colIndex] = crypto.Encrypt(value.ToString(), key);
            }

            outDt.LoadDataRow(values, false);
        }

        #endregion Protected Methods
    }
}