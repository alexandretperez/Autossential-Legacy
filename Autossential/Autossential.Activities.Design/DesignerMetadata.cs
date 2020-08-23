using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Autossential.Activities.Design.Designers;
using Autossential.Activities.Design.Properties;

namespace Autossential.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var main = Resources.Category;
            var dataTableCategory = new CategoryAttribute($"{main}.DataTable");
            var fileCategory = new CategoryAttribute($"{main}.File");
            var programmingCategory = new CategoryAttribute($"{main}.Programming");
            var workflowCategory = new CategoryAttribute($"{main}.Workflow");

            builder.AddCustomAttributes(typeof(Aggregate), dataTableCategory);
            builder.AddCustomAttributes(typeof(Aggregate), new DesignerAttribute(typeof(AggregateDesigner)));
            builder.AddCustomAttributes(typeof(Aggregate), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(WaitFile), fileCategory);
            builder.AddCustomAttributes(typeof(WaitFile), new DesignerAttribute(typeof(WaitFileDesigner)));
            builder.AddCustomAttributes(typeof(WaitFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(RemoveEmptyRows), dataTableCategory);
            builder.AddCustomAttributes(typeof(RemoveEmptyRows), new DesignerAttribute(typeof(RemoveEmptyRowsDesigner)));
            builder.AddCustomAttributes(typeof(RemoveEmptyRows), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Container), workflowCategory);
            builder.AddCustomAttributes(typeof(Container), new DesignerAttribute(typeof(ContainerDesigner)));
            builder.AddCustomAttributes(typeof(Container), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Exit), workflowCategory);
            builder.AddCustomAttributes(typeof(Exit), new DesignerAttribute(typeof(ExitDesigner)));
            builder.AddCustomAttributes(typeof(Exit), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(CultureScope), programmingCategory);
            builder.AddCustomAttributes(typeof(CultureScope), new DesignerAttribute(typeof(CultureScopeDesigner)));
            builder.AddCustomAttributes(typeof(CultureScope), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DataRowToDictionary), dataTableCategory);
            builder.AddCustomAttributes(typeof(DataRowToDictionary), new DesignerAttribute(typeof(DataRowToDictionaryDesigner)));
            builder.AddCustomAttributes(typeof(DataRowToDictionary), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DictionaryToDataTable), dataTableCategory);
            builder.AddCustomAttributes(typeof(DictionaryToDataTable), new DesignerAttribute(typeof(DictionaryToDataTableDesigner)));
            builder.AddCustomAttributes(typeof(DictionaryToDataTable), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Increment), programmingCategory);
            builder.AddCustomAttributes(typeof(Increment), new DesignerAttribute(typeof(IncrementDesigner)));
            builder.AddCustomAttributes(typeof(Increment), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Decrement), programmingCategory);
            builder.AddCustomAttributes(typeof(Decrement), new DesignerAttribute(typeof(DecrementDesigner)));
            builder.AddCustomAttributes(typeof(Decrement), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
