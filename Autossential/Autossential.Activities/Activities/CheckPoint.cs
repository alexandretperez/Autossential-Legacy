using Autossential.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using UiPath.Shared.Activities.Localization;

namespace Autossential.Activities
{
    [LocalizedDisplayName(nameof(Resources.CheckPoint_DisplayName))]
    [LocalizedDescription(nameof(Resources.CheckPoint_Description))]
    public class CheckPoint : Activity
    {
        [LocalizedDisplayName(nameof(Resources.CheckPoint_Expression_DisplayName))]
        [LocalizedDescription(nameof(Resources.CheckPoint_Expression_Description))]
        public InArgument<bool> Expression { get; set; }

        [LocalizedDisplayName(nameof(Resources.CheckPoint_Exception_DisplayName))]
        [LocalizedDescription(nameof(Resources.CheckPoint_Exception_Description))]
        public InArgument<Exception> Exception { get; set; }

        public CheckPoint()
        {
            Implementation = (() => new If
            {
                Condition = new ArgumentValue<bool>(nameof(Expression)),
                Else = new Throw
                {
                    DisplayName = DisplayName,
                    Exception = new LambdaValue<Exception>(ctx => Exception.Get(ctx))
                }
            });
        }
    }
}