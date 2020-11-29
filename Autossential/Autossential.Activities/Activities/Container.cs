using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Container_DisplayName))]
    [LocalizedDescription(nameof(Resources.Container_Description))]
    public class Container : NativeActivity
    {
        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        protected override bool CanInduceIdle => true;

        private IObjectContainer _objectContainer;

        [Browsable(false)]
        public ActivityAction<IObjectContainer​> Body { get; set; }

        #region Constructors

        public Container(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer>(ParentContainerPropertyTag),
                Handler = DefaultBody()
            };
        }

        public static Activity DefaultBody()
        {
            return new Sequence { DisplayName = Resources.Do };
        }

        public Container() : this(new ObjectContainer())
        {
        }

        #endregion Constructors

        protected override void Execute(NativeActivityContext context)
        {
            var exitBookmark = context.CreateBookmark(OnExit, BookmarkOptions.NonBlocking);
            context.Properties.Add(Exit.Bookmark, exitBookmark);

            context.ScheduleAction<IObjectContainer>(Body, _objectContainer, OnCompleted, OnFaulted);
        }

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
            foreach (var obj in _objectContainer.Where(o => o is IDisposable))
            {
                if (obj is IDisposable dispObject)
                    dispObject.Dispose();
            }
            _objectContainer.Clear();
        }

        #endregion Helpers

        private void OnExit(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.CancelChildren();
            Cleanup();
        }
    }
}