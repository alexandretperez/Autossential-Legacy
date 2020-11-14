using Autossential.Activities.Properties;
using System.Activities;
using System.Activities.Validation;
using System.Collections.Generic;

namespace Autossential.Activities.Contraints
{
    internal class ContainerChildConstraint : NativeActivity<bool>
    {
        [RequiredArgument]
        [System.ComponentModel.DefaultValue(null)]
        public InArgument<IEnumerable<Activity>> ParentChain
        {
            get;
            set;
        }

        protected override void Execute(NativeActivityContext context)
        {
            foreach (var activity in ParentChain.Get(context))
            {
                if (IsInContainer(activity))
                    return;
            }

            Constraint.AddValidationError(context, new ValidationError(string.Format(Resources.ValidationScope_Error, nameof(Container))));
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            var arg = new RuntimeArgument("ParentChain", typeof(IEnumerable<Activity>), ArgumentDirection.In, isRequired: true);
            metadata.Bind(ParentChain, arg);
            metadata.AddArgument(arg);
        }

        private bool IsInContainer(Activity activity)
        {
            if (activity == null) return false;
            return activity is Container;
        }
    }
}