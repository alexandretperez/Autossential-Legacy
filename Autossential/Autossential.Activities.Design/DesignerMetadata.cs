using Autossential.Activities.Design.Designers;
using Autossential.Activities.Design.Properties;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;

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
            var fileCompressionCategory = new CategoryAttribute($"{main}.File.Compression");
            var programmingCategory = new CategoryAttribute($"{main}.Programming");
            var workflowCategory = new CategoryAttribute($"{main}.Workflow");
            var securityCategory = new CategoryAttribute($"{main}.Security");

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

            builder.AddCustomAttributes(typeof(PromoteHeaders), dataTableCategory);
            builder.AddCustomAttributes(typeof(PromoteHeaders), new DesignerAttribute(typeof(PromoteHeadersDesigner)));
            builder.AddCustomAttributes(typeof(PromoteHeaders), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(EnumerateFiles), fileCategory);
            builder.AddCustomAttributes(typeof(EnumerateFiles), new DesignerAttribute(typeof(EnumerateFilesDesigner)));
            builder.AddCustomAttributes(typeof(EnumerateFiles), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Zip), fileCompressionCategory);
            builder.AddCustomAttributes(typeof(Zip), new DesignerAttribute(typeof(ZipDesigner)));
            builder.AddCustomAttributes(typeof(Zip), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Unzip), fileCompressionCategory);
            builder.AddCustomAttributes(typeof(Unzip), new DesignerAttribute(typeof(UnzipDesigner)));
            builder.AddCustomAttributes(typeof(Unzip), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(CheckPoint), workflowCategory);
            builder.AddCustomAttributes(typeof(CheckPoint), new DesignerAttribute(typeof(CheckPointDesigner)));
            builder.AddCustomAttributes(typeof(CheckPoint), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(EncryptText), securityCategory);
            builder.AddCustomAttributes(typeof(EncryptText), new DesignerAttribute(typeof(EncryptTextDesigner)));
            builder.AddCustomAttributes(typeof(EncryptText), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DecryptText), securityCategory);
            builder.AddCustomAttributes(typeof(DecryptText), new DesignerAttribute(typeof(DecryptTextDesigner)));
            builder.AddCustomAttributes(typeof(DecryptText), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(EncryptDataTable), securityCategory);
            builder.AddCustomAttributes(typeof(EncryptDataTable), new DesignerAttribute(typeof(EncryptDataTableDesigner)));
            builder.AddCustomAttributes(typeof(EncryptDataTable), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DecryptDataTable), securityCategory);
            builder.AddCustomAttributes(typeof(DecryptDataTable), new DesignerAttribute(typeof(DecryptDataTableDesigner)));
            builder.AddCustomAttributes(typeof(DecryptDataTable), new HelpKeywordAttribute(""));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}