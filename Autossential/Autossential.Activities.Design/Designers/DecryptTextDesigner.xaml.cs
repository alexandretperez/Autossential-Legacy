using Autossential.Helpers;
using Autossential.Security;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for DecryptTextDesigner.xaml
    /// </summary>
    public partial class DecryptTextDesigner
    {
        public DecryptTextDesigner()
        {
            InitializeComponent();

            cbAlgorithms.ItemsSource = EnumHelper.EnumAsDictionary<SymmetricAlgorithms>();
            cbAlgorithms.DisplayMemberPath = "Key";
            cbAlgorithms.SelectedValuePath = "Value";
        }
    }
}