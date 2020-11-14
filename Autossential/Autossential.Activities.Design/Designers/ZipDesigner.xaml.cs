using Autossential.Helpers;
using Microsoft.VisualBasic.Activities;
using Microsoft.Win32;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for ZipDesigner.xaml
    /// </summary>
    public partial class ZipDesigner
    {
        public ZipDesigner()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = true
            };

            if (string.IsNullOrEmpty(ofd.Filter))
                ofd.Filter = "All files (*.*)|*.*";

            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            ofd.Title = "Select file(s)";

            if (ofd.ShowDialog() == true)
            {
                var baseUri = new Uri(ofd.InitialDirectory);

                var files = ofd.FileNames;
                if (files.Length == 1)
                {
                    ModelItem.Properties[nameof(Zip.ToCompress)].SetValue(InArgument<string>.FromValue(IOHelper.GetRelativePath(ofd.InitialDirectory, files[0])));
                    return;
                }

                var paths = files.Select(path => $"\"{IOHelper.GetRelativePath(ofd.InitialDirectory, path)}\"");

                ModelItem.Properties[nameof(Zip.ToCompress)].SetValue(new InArgument<IEnumerable<string>>(new VisualBasicValue<IEnumerable<string>>
                {
                    ExpressionText = $"{{{string.Join(", ", paths)}}}"
                }));
            }
        }
    }
}