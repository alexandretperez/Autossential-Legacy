using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Unzip_DisplayName))]
    [LocalizedDescription(nameof(Resources.Unzip_Description))]
    public class Unzip : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.Unzip_ZipFilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.Unzip_ZipFilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ZipFilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.Unzip_ExtractTo_DisplayName))]
        [LocalizedDescription(nameof(Resources.Unzip_ExtractTo_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ExtractTo { get; set; }

        [LocalizedDisplayName(nameof(Resources.Unzip_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.Unzip_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public bool Overwrite { get; set; }

        #endregion Properties

        #region Constructors

        public Unzip()
        {
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ZipFilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ZipFilePath)));
            if (ExtractTo == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ExtractTo)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var zipFilePath = ZipFilePath.Get(context);
            var extractTo = ExtractTo.Get(context);

            using (var zip = ZipFile.OpenRead(zipFilePath))
            {
                var dir = Directory.CreateDirectory(extractTo);
                var dirPath = dir.FullName;

                foreach (var entry in zip.Entries)
                {
                    var fullPath = Path.GetFullPath(Path.Combine(dirPath, entry.FullName));

                    if (!fullPath.StartsWith(dirPath, StringComparison.OrdinalIgnoreCase))
                        throw new IOException(Resources.Unzip_ExtractingResultsInOutside_Error);

                    if (Path.GetFileName(fullPath).Length == 0)
                    {
                        if (entry.Length != 0L)
                            throw new IOException(Resources.Unzip_DirectoryNameWithData_Error);

                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        entry.ExtractToFile(fullPath, Overwrite);
                    }
                }
            }

            // Outputs
            return (_) => { };
        }

        #endregion Protected Methods
    }
}