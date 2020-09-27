using System.Activities;
using System.Windows.Forms;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for EnumerateFilesDesigner.xaml
    /// </summary>
    public partial class EnumerateFilesDesigner
    {
        public EnumerateFilesDesigner()
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
                    ModelItem.Properties[nameof(EnumerateFiles.Path)].SetValue(new InArgument<string>(fbd.SelectedPath));
                }
            }
        }
    }
}