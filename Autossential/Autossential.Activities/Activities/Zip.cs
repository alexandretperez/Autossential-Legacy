using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Zip_DisplayName))]
    [LocalizedDescription(nameof(Resources.Zip_Description))]
    public class Zip : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.Zip_ToCompress_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_ToCompress_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument ToCompress { get; set; }

        [LocalizedDisplayName(nameof(Resources.Zip_ZipFilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_ZipFilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ZipFilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.Zip_Encoding_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_Encoding_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public Encoding Encoding { get; set; }

        [LocalizedDisplayName(nameof(Resources.Zip_AutoRenaming_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_AutoRenaming_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public bool AutoRenaming { get; set; } = true;

        [LocalizedDisplayName(nameof(Resources.Zip_CompressionLevel_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_CompressionLevel_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public CompressionLevel CompressionLevel { get; set; }

        [LocalizedDisplayName(nameof(Resources.Zip_FilesCount_DisplayName))]
        [LocalizedDescription(nameof(Resources.Zip_FilesCount_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<int> FilesCount { get; set; }

        #endregion Properties

        #region Constructors

        public Zip()
        {
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (ZipFilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ZipFilePath)));
            if (ToCompress == null)
            {
                metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ToCompress)));
            }
            else if (ToCompress.ArgumentType == typeof(string) || typeof(IEnumerable<string>).IsAssignableFrom(ToCompress.ArgumentType))
            {
                var arg = new RuntimeArgument(nameof(ToCompress), ToCompress.ArgumentType, ArgumentDirection.In, true);
                metadata.Bind(ToCompress, arg);
                metadata.AddArgument(arg);
            }
            else
            {
                metadata.AddValidationError(string.Format(Resources.ValidationType_StringOrStrings_Error, nameof(ToCompress)));
            }
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var toCompress = ToCompress.Get(context);
            var zipFilePath = ZipFilePath.Get(context);

            if (toCompress is string)
                toCompress = new string[] { toCompress.ToString() };

            var paths = (IEnumerable<string>)toCompress;
            var directories = paths.Where(Directory.Exists);
            var files = paths.Except(directories)
                .Concat(directories.SelectMany(path => Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)))
                .Select(path => new FileInfo(path))
                .Where(fi => fi.FullName != Path.GetFullPath(zipFilePath));

            var mode = File.Exists(zipFilePath) ? ZipArchiveMode.Update : ZipArchiveMode.Create;

            var counter = 0;
            var allInRoot = files.Select(fi => fi.Directory.FullName).Distinct().Count() == 1;
            using (var zip = ZipFile.Open(zipFilePath, mode, Encoding))
            {
                counter = CreateZip(files, allInRoot, zip, mode);
            }

            // Outputs
            return (ctx) => FilesCount.Set(ctx, counter);
        }

        private int CreateZip(IEnumerable<FileInfo> files, bool allInRoot, ZipArchive zip, ZipArchiveMode mode)
        {
            var dic = new Dictionary<string, int>();
            string Rename(string keyName, string entryName)
            {
                var entry = zip.GetEntry(entryName);
                if (entry == null)
                    return entryName;

                if (dic.ContainsKey(keyName))
                    dic[keyName]++;
                else
                    dic.Add(keyName, 1);

                return Rename(keyName, $"{Path.ChangeExtension(keyName, $"{dic[keyName]}{Path.GetExtension(keyName)}")}");
            }

            var renaming = AutoRenaming && mode == ZipArchiveMode.Update;
            var counter = 0;
            if (allInRoot)
            {
                foreach (var file in files)
                {
                    zip.CreateEntryFromFile(file.FullName, renaming ? Rename(file.Name, file.Name) : file.Name, CompressionLevel);
                    counter++;
                }
            }
            else
            {
                foreach (var file in files)
                {
                    var name = NormalizeName(file);
                    zip.CreateEntryFromFile(file.FullName, renaming ? Rename(name, name) : name, CompressionLevel);
                    counter++;
                }
            }

            return counter;
        }

        private static string NormalizeName(FileInfo file)
        {
            return file.FullName.Substring(file.Directory.Root.FullName.Length).Replace('\\', '/');
        }

        #endregion Protected Methods
    }
}