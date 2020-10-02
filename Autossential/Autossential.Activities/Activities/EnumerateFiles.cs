using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.EnumerateFiles_DisplayName))]
    [LocalizedDescription(nameof(Resources.EnumerateFiles_Description))]
    public class EnumerateFiles : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.EnumerateFiles_Path_DisplayName))]
        [LocalizedDescription(nameof(Resources.EnumerateFiles_Path_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument Path { get; set; }

        [LocalizedDisplayName(nameof(Resources.EnumerateFiles_SearchPattern_DisplayName))]
        [LocalizedDescription(nameof(Resources.EnumerateFiles_SearchPattern_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument SearchPattern { get; set; }

        [LocalizedDisplayName(nameof(Resources.EnumerateFiles_SearchOption_DisplayName))]
        [LocalizedDescription(nameof(Resources.EnumerateFiles_SearchOption_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public SearchOption SearchOption { get; set; }

        [LocalizedDisplayName(nameof(Resources.EnumerateFiles_Exclusions_DisplayName))]
        [LocalizedDescription(nameof(Resources.EnumerateFiles_Exclusions_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public FileAttributes Exclusions { get; set; } = FileAttributes.Hidden
                                                        | FileAttributes.System
                                                        | FileAttributes.Temporary
                                                        | FileAttributes.Device
                                                        | FileAttributes.Offline;

        [LocalizedDisplayName(nameof(Resources.EnumerateFiles_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.EnumerateFiles_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<IEnumerable<string>> Result { get; set; }

        #endregion Properties

        #region Constructors

        public EnumerateFiles()
        {
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (Path == null)
            {
                metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Path)));
            }
            else if (IsStringOrCollectionOfString(Path))
            {
                var argument = new RuntimeArgument(nameof(Path), Path.ArgumentType, ArgumentDirection.In, true);
                metadata.Bind(Path, argument);
                metadata.AddArgument(argument);
            }
            else
            {
                metadata.AddValidationError(string.Format(Resources.ValidationType_StringOrStrings_Error, nameof(Path)));
            }

            if (SearchPattern != null)
            {
                if (IsStringOrCollectionOfString(SearchPattern))
                {
                    var argument = new RuntimeArgument(nameof(SearchPattern), SearchPattern.ArgumentType, ArgumentDirection.In, true);
                    metadata.Bind(SearchPattern, argument);
                    metadata.AddArgument(argument);
                }
                else
                {
                    metadata.AddValidationError(string.Format(Resources.ValidationType_StringOrStrings_Error, nameof(SearchPattern)));
                }
            }
        }

        private bool IsStringOrCollectionOfString(InArgument arg)
        {
            return arg.ArgumentType == typeof(string) || typeof(IEnumerable<string>).IsAssignableFrom(arg.ArgumentType);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            var path = Path.Get(context);
            var criteria = SearchPattern?.Get(context) ?? "*.*";

            var directories = ResolveValues(path);
            var patterns = ResolveValues(criteria);

            IEnumerable<string> result = new string[] { };
            foreach (var directory in directories)
            {
                foreach (var pattern in patterns)
                {
                    result = result.Union(Directory.EnumerateFiles(directory, pattern, SearchOption));
                }
            }

            if (Exclusions > 0)
                result = result.Where(filePath => (new FileInfo(filePath).Attributes & Exclusions) == 0);

            // Outputs
            return (ctx) => Result.Set(ctx, result);
        }

        private HashSet<string> ResolveValues(object value)
        {
            var unique = new HashSet<string>();
            if (value is string[] valueArray)
            {
                foreach (var val in valueArray)
                {
                    unique.Add(val);
                }
            }
            else
            {
                unique.Add((string)value);
            }

            return unique;
        }

        #endregion Protected Methods
    }
}