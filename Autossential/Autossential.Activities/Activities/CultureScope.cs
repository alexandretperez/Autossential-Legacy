using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.CultureScope_DisplayName))]
    [LocalizedDescription(nameof(Resources.CultureScope_Description))]
    public class CultureScope : ContinuableAsyncNativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<IObjectContainer​> Body { get; set; }

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.CultureScope_CultureName_DisplayName))]
        [LocalizedDescription(nameof(Resources.CultureScope_CultureName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> CultureName { get; set; }

        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        // Object Container: Add strongly-typed objects here and they will be available in the scope's child activities.
        private readonly IObjectContainer _objectContainer;

        private CultureInfo _previousCulture;

        #endregion Properties

        #region Constructors

        public CultureScope(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer>(ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = Resources.Do }
            };
        }

        public CultureScope() : this(new ObjectContainer())
        {
        }

        #endregion Constructors

        #region Protected Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            if (CultureName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(CultureName)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<NativeActivityContext>> ExecuteAsync(NativeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var cultureName = CultureName.Get(context);

            return (ctx) =>
            {
                // Schedule child activities
                _previousCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

                if (Body != null)
                    ctx.ScheduleAction<IObjectContainer>(Body, _objectContainer, OnCompleted, OnFaulted);

                // Outputs
            };
        }

        #endregion Protected Methods

        #region Events

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            faultContext.CancelChildren();
            Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Cleanup();
        }

        #endregion Events

        #region Helpers

        private void Cleanup()
        {
            Thread.CurrentThread.CurrentCulture = _previousCulture;

            foreach (var obj in _objectContainer.Where(o => o is IDisposable))
            {
                if (obj is IDisposable dispObject)
                    dispObject.Dispose();
            }
            _objectContainer.Clear();
        }

        #endregion Helpers
    }
}