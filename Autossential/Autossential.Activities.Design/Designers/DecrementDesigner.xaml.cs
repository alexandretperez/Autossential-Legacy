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
            ModelItem.Properties[nameof(Decrement.Value)].SetValue(model.Value + 1);
        }
    }
}
