using Autossential.Helpers;
using Autossential.Security;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for EncryptTextDesigner.xaml
    /// </summary>
    public partial class EncryptTextDesigner
    {
        public EncryptTextDesigner()
        {
            InitializeComponent();

            cbAlgorithms.ItemsSource = EnumHelper.EnumAsDictionary<SymmetricAlgorithms>();
            cbAlgorithms.DisplayMemberPath = "Key";
            cbAlgorithms.SelectedValuePath = "Value";
        }
    }
}