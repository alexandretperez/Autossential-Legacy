using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Validation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.WaitFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.WaitFile_Description))]
    public class WaitFile : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 30000;

        [LocalizedDisplayName(nameof(Resources.WaitFile_FilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.WaitFile_FilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.WaitFile_WaitForExist_DisplayName))]
        [LocalizedDescription(nameof(Resources.WaitFile_WaitForExist_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public bool WaitForExist { get; set; }

        [LocalizedDisplayName(nameof(Resources.WaitFile_ThrowFileException_DisplayName))]
        [LocalizedDescription(nameof(Resources.WaitFile_ThrowFileException_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public bool ThrowFileException { get; set; }

        [LocalizedDisplayName(nameof(Resources.WaitFile_FileInfo_DisplayName))]
        [LocalizedDescription(nameof(Resources.WaitFile_FileInfo_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<FileInfo> FileInfo { get; set; }

        [LocalizedDisplayName(nameof(Resources.WaitFile_Interval_DisplayName))]
        [LocalizedDescription(nameof(Resources.WaitFile_Interval_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public int Interval { get; set; } = 500;

        private const int MinimumInterval = 100;

        private const int MaximumInterval = 30000;

        private Exception _latestFileException;

        #endregion Properties

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (FilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FilePath)));
            if (Interval < MinimumInterval || Interval > MaximumInterval)
                metadata.AddValidationError(new ValidationError(string.Format(Resources.WaitFile_Interval_Range_Error, MinimumInterval, MaximumInterval), true));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var filePath = FilePath.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, filePath, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false) != task)
            {
                if (ThrowFileException && _latestFileException != null)
                {
                    throw _latestFileException;
                }

                throw new TimeoutException(Resources.Timeout_Error);
            }

            await task.ConfigureAwait(false);

            // Outputs
            return (ctx) => FileInfo.Set(ctx, new FileInfo(filePath));
        }

        private Task ExecuteWithTimeout(AsyncCodeActivityContext _, string filePath, CancellationToken cancellationToken = default)
        {
            var interval = GetInterval();

            return Task.Run(() =>
            {
                var done = false;

                if (!WaitForExist && !File.Exists(filePath))
                    throw new IOException(Resources.WaitFile_FileDoesNotExist_Error);

                do
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                            {
                                done = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _latestFileException = e;
                        Thread.Sleep(interval);
                    }
                } while (!done);
            }, cancellationToken);
        }

        #endregion Protected Methods

        private int GetInterval()
        {
            return Interval < MinimumInterval ? MinimumInterval : Math.Min(Interval, MaximumInterval);
        }
    }
}