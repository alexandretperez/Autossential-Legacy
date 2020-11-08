using Autossential.Enums;
using Autossential.Helpers;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for AggregateDesigner.xaml
    /// </summary>
    public partial class AggregateDesigner
    {
        public AggregateDesigner()
        {
            InitializeComponent();

            cbFunctions.ItemsSource = EnumHelper.EnumAsDictionary<AggregationFunction>();
            cbFunctions.DisplayMemberPath = "Key";
            cbFunctions.SelectedValuePath = "Value";
        }
    }
}