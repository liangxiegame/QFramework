using QFramework.GraphDesigner;

namespace QFramework
{
    [TemplateClass(TemplateLocation.DesignerFile)]
    public class UIPanelDataTemplate: IClassTemplate<PanelCodeData>,ITemplateCustomFilename
    {
        public string OutputPath { get; private set; }
        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.SetBaseType(typeof(UIPanelData));
            Ctx.CurrentDeclaration.Name = Ctx.Data.PanelName + "Data";
        }

        public TemplateContext<PanelCodeData> Ctx { get; set; }
        public string Filename { get; private set; }
    }
}