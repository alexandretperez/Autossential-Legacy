using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Iterate_DisplayName))]
    [LocalizedDescription(nameof(Resources.Iterate_Description))]
    public class Iterate : NativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<IObjectContainer​> Body { get; set; }

        protected override bool CanInduceIdle => true;

        [LocalizedDisplayName(nameof(Resources.Iterate_Iterations_DisplayName))]
        [LocalizedDescription(nameof(Resources.Iterate_Iterations_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Iterations { get; set; }

        [LocalizedDisplayName(nameof(Resources.Iterate_Index_DisplayName))]
        [LocalizedDescription(nameof(Resources.Iterate_Index_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<int> Index { get; set; }

        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        // Object Container: Add strongly-typed objects here and they will be available in the scope's child activities.
        private readonly IObjectContainer _objectContainer;

        #endregion Properties

        public Iterate(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer>(ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = Resources.Do }
            };
        }

        public Iterate() : this(new ObjectContainer())
        {
        }

        private int _totalIterations = 0;
        private int _currentIteration = 0;
        private bool _break;

        protected override void Execute(NativeActivityContext context)
        {
            var exitBookmark = context.CreateBookmark(OnExit, BookmarkOptions.NonBlocking);
            context.Properties.Add(Exit.Bookmark, exitBookmark);

            var nextBookmark = context.CreateBookmark(OnNext, BookmarkOptions.MultipleResume | BookmarkOptions.NonBlocking);
            context.Properties.Add(Next.Bookmark, nextBookmark);

            _totalIterations = Iterations.Get(context);

            ExecuteNext(context);
        }

        private void ExecuteNext(NativeActivityContext context)
        {
            Index.Set(context, _currentIteration);
            context.ScheduleAction<IObjectContainer​>(Body, _objectContainer, OnIterateCompleted);
        }

        private void OnIterateCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
                _break = true;
            }

            if (++_currentIteration < _totalIterations && !_break)
                ExecuteNext(context);
        }

        private void OnNext(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.CancelChildren();
            if (value is Bookmark b)
                context.ResumeBookmark(b, value);
        }

        private void OnExit(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.CancelChildren();
            _break = true;
            if (value is Bookmark b)
                context.ResumeBookmark(b, value);
        }
    }
}