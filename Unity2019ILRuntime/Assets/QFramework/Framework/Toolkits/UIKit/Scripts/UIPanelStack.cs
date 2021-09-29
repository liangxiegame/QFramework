using System.Collections.Generic;

namespace QFramework
{
    /// <summary>
    /// UI 界面的堆栈
    /// </summary>
    public class UIPanelStack
    {
        private Stack<PanelInfo> mUIStack = new Stack<PanelInfo>();

        public void Push<T>() where T : UIPanel
        {
            Push(UIKit.GetPanel<T>());
        }

        public void Push(IPanel view)
        {
            if (view != null)
            {
                mUIStack.Push(view.Info);
                view.Close();

                var panelSearchKeys = PanelSearchKeys.Allocate();

                panelSearchKeys.GameObjName = view.Transform.name;

                UIManager.Instance.RemoveUI(panelSearchKeys);

                panelSearchKeys.Recycle2Cache();
            }
        }

        public void Pop()
        {
            var previousPanelInfo = mUIStack.Pop();

            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.GameObjName = previousPanelInfo.GameObjName;
            panelSearchKeys.Level = previousPanelInfo.Level;
            panelSearchKeys.UIData = previousPanelInfo.UIData;
            panelSearchKeys.AssetBundleName = previousPanelInfo.AssetBundleName;
            panelSearchKeys.PanelType = previousPanelInfo.PanelType;
    
            UIManager.Instance.OpenUI(panelSearchKeys);
            
            panelSearchKeys.Recycle2Cache();
        }
    }
}