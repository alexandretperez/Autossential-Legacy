using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Autossential.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

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

        #endregion


        #region Constructors

        public DictionaryToDataTable()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
    
            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////

            // Outputs
            return (ctx) => {
            };
        }

        #endregion
    }
}

