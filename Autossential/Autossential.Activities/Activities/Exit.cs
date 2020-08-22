using Autossential.Activities.Contraints;
using Autossential.Activities.Properties;
using System.Activities;
using System.Activities.Validation;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Exit_DisplayName))]
    [LocalizedDescription(nameof(Resources.Exit_Description))]
    public class Exit : NativeActivity
    {
        private class ExitConstraint : ContainerChildConstraint { }

        internal const string ExitBookmark = "Exit";

        protected override bool CanInduceIdle => true;

        public Exit()
        {
            var arg = new DelegateInArgument<Exit>("constraintArg");
            var ctx = new DelegateInArgument<ValidationContext>("validationContext");
            Constraints.Add(new Constraint<Exit>
            {
                Body = new ActivityAction<Exit, ValidationContext>
                {
                    Argument1 = arg,
                    Argument2 = ctx,
                    Handler = new ExitConstraint
                    {
                        ParentChain = new GetParentChain { ValidationContext = ctx }
                    }
                }
            });
        }

        protected override void Execute(NativeActivityContext context)
        {
            var bookmark = (Bookmark)context.Properties.Find(ExitBookmark);
            if (bookmark != null)
            {
                var value = context.CreateBookmark();
                context.ResumeBookmark(bookmark, value);
            }
        }
    }
}