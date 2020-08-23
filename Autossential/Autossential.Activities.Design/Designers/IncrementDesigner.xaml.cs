using System.Activities.Presentation.View;
using UiPath.Shared.Activities.Design.Services;

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
            ModelItem.Properties[nameof(Increment.Value)].SetValue(model.Value + 1);
        }
    }
}