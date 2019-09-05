using System.CodeDom;
using QF.GraphDesigner;

namespace QFramework
{
    [TemplateClass(TemplateLocation.DesignerFile)]
    [RequiresNamespace("UnityEngine")]
    [RequiresNamespace("UnityEngine.UI")]
    [AsPartial]
    public class UIPanelTemplate : IClassTemplate<PanelCodeInfo>, ITemplateCustomFilename
    {
        public string OutputPath { get; private set; }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.SetBaseType(typeof(UIPanel));
            Ctx.CurrentDeclaration.Name = Ctx.Data.GameObjectName;
        }

        [GenerateMethod, AsOverride]
        protected void ProcessMsg(int eventId, QMsg msg)
        {
            Ctx._("throw new System.NotImplementedException ()");
        }

        [GenerateMethod, AsOverride]
        protected void OnInit(IUIData uiData = null)
        {
            Ctx._("mData = uiData as {0} ?? new {0}()",Ctx.Data.GameObjectName + "Data");
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


        public TemplateContext<PanelCodeInfo> Ctx { get; set; }

        public string Filename { get; private set; }
    }
}