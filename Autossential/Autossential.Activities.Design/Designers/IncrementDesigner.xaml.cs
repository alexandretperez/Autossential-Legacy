using System;
using System.Windows.Input;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for IncrementDesigner.xaml
    /// </summary>
    public partial class IncrementDesigner
    {
        public IncrementDesigner()
        {
            InitializeComponent();
        }

        private void IncrementButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = ModelItem.GetCurrentValue() as Increment;
            var value = model.Value;

            if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                value = Math.Max(1, value - 1);
            }
            else
            {
                value++;
            }

            ModelItem.Properties[nameof(Increment.Value)].SetValue(value);
        }
    }
}