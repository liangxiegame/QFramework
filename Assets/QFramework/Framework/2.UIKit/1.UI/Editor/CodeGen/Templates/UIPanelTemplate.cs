using System.CodeDom;
using QFramework.GraphDesigner;

namespace QFramework
{
    [TemplateClass(TemplateLocation.DesignerFile)]
    [RequiresNamespace("UnityEngine")]
    [RequiresNamespace("UnityEngine.UI")]
    [AsPartial]
    public class UIPanelTemplate : IClassTemplate<PanelCodeData>, ITemplateCustomFilename
    {
        public string OutputPath { get; private set; }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.SetBaseType(typeof(UIPanel));
            Ctx.CurrentDeclaration.Name = Ctx.Data.PanelName;
        }

        [GenerateMethod, AsOverride]
        protected void ProcessMsg(int eventId, QMsg msg)
        {
            Ctx._("throw new System.NotImplementedException ()");
        }

        [GenerateMethod, AsOverride]
        protected void OnInit(IUIData uiData = null)
        {
            Ctx._("mData = uiData as {0} ?? new {0}()",Ctx.Data.PanelName + "Data");
            Ctx._comment("please add init code here");
        }

        [GenerateMethod, AsOverride]
        protected void OnOpen(IUIData uiData = null)
        {
        }

        [GenerateMethod, AsOverride]
        protected void OnShow()
        {
        }

        [GenerateMethod, AsOverride]
        protected void OnHide()
        {
        }

        [GenerateMethod, AsOverride]
        protected void OnClose()
        {
        }


        public TemplateContext<PanelCodeData> Ctx { get; set; }

        public string Filename { get; private set; }
    }
}