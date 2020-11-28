using Autossential.Activities.Properties;
using Autossential.Security;
using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.EncryptText_DisplayName))]
    [LocalizedDescription(nameof(Resources.EncryptText_Description))]
    public class EncryptText : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Key_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Key_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Key { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Encoding_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Encoding_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<Encoding> TextEncoding { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Algorithm_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Algorithm_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public SymmetricAlgorithms Algorithm { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptText_Iterations_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptText_Iterations_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> Iterations { get; set; }

        #endregion Properties

        #region Constructors

        public EncryptText()
        {
            TextEncoding = new VisualBasicValue<Encoding>($"{typeof(Encoding).FullName}.{nameof(Encoding.UTF8)}");
            Iterations = new VisualBasicValue<int>("1000");
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Text == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Text)));
            if (Key == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Key)));
            if (TextEncoding == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(TextEncoding)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var text = Text.Get(context);
            var password = Key.Get(context);
            var encoding = TextEncoding.Get(context);
            var iterations = Iterations.Get(context);

            string result = text;
            if (!string.IsNullOrEmpty(text))
            {
                using (var crypto = new Crypto(Algorithm, encoding, iterations))
                    result = crypto.Encrypt(text, password);
            }
         
            return (ctx) => Result.Set(ctx, result);
        }

        #endregion Protected Methods
    }
}