using System.Activities;
using System.Windows.Forms;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for UnzipDesigner.xaml
    /// </summary>
    public partial class UnzipDesigner
    {
        public UnzipDesigner()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var dialog = fbd.ShowDialog();
                if (dialog == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    ModelItem.Properties[nameof(Unzip.ExtractTo)].SetValue(InArgument<string>.FromValue(fbd.SelectedPath));
                }
            }
        }
    }
}