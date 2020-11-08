using Autossential.Helpers;
using Autossential.Security;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for EncryptDataTableDesigner.xaml
    /// </summary>
    public partial class EncryptDataTableDesigner
    {
        public EncryptDataTableDesigner()
        {
            InitializeComponent();

            cbAlgorithms.ItemsSource = EnumHelper.EnumAsDictionary<SymmetricAlgorithms>();
            cbAlgorithms.DisplayMemberPath = "Key";
            cbAlgorithms.SelectedValuePath = "Value";
        }
    }
}