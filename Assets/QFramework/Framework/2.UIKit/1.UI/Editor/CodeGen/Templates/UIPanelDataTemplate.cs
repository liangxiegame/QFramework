using QF.GraphDesigner;

namespace QFramework
{
    [TemplateClass(TemplateLocation.DesignerFile)]
    public class UIPanelDataTemplate: IClassTemplate<PanelCodeInfo>,ITemplateCustomFilename
    {
        public string OutputPath { get; private set; }
        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.SetBaseType(typeof(UIPanelData));
            Ctx.CurrentDeclaration.Name = Ctx.Data.GameObjectName + "Data";
        }

        public TemplateContext<PanelCodeInfo> Ctx { get; set; }
        public string Filename { get; private set; }
    }
}