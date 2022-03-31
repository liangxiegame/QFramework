using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
    public class UIGameInfoPanelData : UIPanelData
    {
    }

    public partial class UIGameInfoPanel : UIPanel
    {
        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGameInfoPanelData ?? new UIGameInfoPanelData();

            BtnBack.onClick.AddListener(() =>
            {
                UIKit.OpenPanelAsync<UIStartPanel>().ToAction().Start(this, this.CloseSelf);
            });
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }
    }
}