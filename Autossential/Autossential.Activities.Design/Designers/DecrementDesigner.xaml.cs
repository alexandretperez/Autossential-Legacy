using System;
using System.Windows.Input;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for DecrementDesigner.xaml
    /// </summary>
    public partial class DecrementDesigner
    {
        public DecrementDesigner()
        {
            InitializeComponent();
        }

        private void DecrementButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = ModelItem.GetCurrentValue() as Decrement;
            var value = model.Value;

            if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                value = Math.Max(1, value - 1);
            }
            else
            {
                value++;
            }

            ModelItem.Properties[nameof(Decrement.Value)].SetValue(value);
        }
    }
}