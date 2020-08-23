using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Autossential.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Decrement_DisplayName))]
    [LocalizedDescription(nameof(Resources.Decrement_Description))]
    public class Decrement : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.Decrement_Variable_DisplayName))]
        [LocalizedDescription(nameof(Resources.Decrement_Variable_Description))]
        [LocalizedCategory(nameof(Resources.InputOutput_Category))]
        public InOutArgument<int> Variable { get; set; }

        [LocalizedDisplayName(nameof(Resources.Decrement_Value_DisplayName))]
        [LocalizedDescription(nameof(Resources.Decrement_Value_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public int Value { get; set; } = 1;

        #endregion


        #region Constructors

        public Decrement()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Variable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Variable)));
            if (Value < 1) metadata.AddValidationError(Resources.Decrement_Value_Error);

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var variable = Variable.Get(context);

            // Outputs
            return (ctx) => Variable.Set(ctx, variable - Value);
        }

        #endregion
    }
}
