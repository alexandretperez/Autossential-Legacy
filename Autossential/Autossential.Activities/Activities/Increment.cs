using Autossential.Activities.Properties;
using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Increment_DisplayName))]
    [LocalizedDescription(nameof(Resources.Increment_Description))]
    public class Increment : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.Increment_Variable_DisplayName))]
        [LocalizedDescription(nameof(Resources.Increment_Variable_Description))]
        [LocalizedCategory(nameof(Resources.InputOutput_Category))]
        public InOutArgument<int> Variable { get; set; }

        [LocalizedDisplayName(nameof(Resources.Increment_Value_DisplayName))]
        [LocalizedDescription(nameof(Resources.Increment_Value_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> Value { get; set; }

        #endregion Properties

        #region Constructors

        public Increment()
        {
            Value = new VisualBasicValue<int>("1");
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Variable == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Variable)));
            if (Value == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Value)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var variable = Variable.Get(context);
            var value = Value.Get(context);

            if (value < 1)
                throw new InvalidOperationException(Resources.Increment_Value_Error);

            // Outputs
            return (ctx) => Variable.Set(ctx, variable + value);
        }

        #endregion Protected Methods
    }
}