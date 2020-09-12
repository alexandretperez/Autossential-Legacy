using System.Activities.Presentation.Model;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for ContainerDesigner.xaml
    /// </summary>
    public partial class ContainerDesigner
    {
        public ContainerDesigner()
        {
            InitializeComponent();
        }

        protected override void OnModelItemChanged(object newItem)
        {
            Body().Value.PropertyChanged += Body_PropertyChanged;
        }

        private void Body_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            const string propName = nameof(Container.Body.Handler);

            if (e.PropertyName != propName)
                return;

            var body = Body().Value.Properties[nameof(Container.Body.Handler)];
            if (body.Value == null)
            {
                body.SetValue(Container.DefaultBody());
            }
        }

        public ModelProperty Body() => ModelItem.Properties[nameof(Container.Body)];
    }
}