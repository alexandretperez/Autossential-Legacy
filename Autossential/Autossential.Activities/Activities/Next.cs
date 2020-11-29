using Autossential.Activities.Contraints;
using Autossential.Activities.Properties;
using System.Activities;
using System.Activities.Validation;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.Next_DisplayName))]
    [LocalizedDescription(nameof(Resources.Next_Description))]
    public class Next : NativeActivity
    {
        internal const string Bookmark = "Next";

        protected override bool CanInduceIdle => true;

        public Next()
        {
            var arg = new DelegateInArgument<Next>("constraintArg");
            var ctx = new DelegateInArgument<ValidationContext>("validationContext");
            Constraints.Add(new Constraint<Next>
            {
                Body = new ActivityAction<Next, ValidationContext>
                {
                    Argument1 = arg,
                    Argument2 = ctx,
                    Handler = new NextConstraint
                    {
                        ParentChain = new GetParentChain { ValidationContext = ctx }
                    }
                }
            });
        }

        protected override void Execute(NativeActivityContext context)
        {
            var bookmark = (Bookmark)context.Properties.Find(Bookmark);
            if (bookmark != null)
            {
                var value = context.CreateBookmark();
                context.ResumeBookmark(bookmark, value);
            }
        }
    }
}