using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public class UISectionPanelData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISectionPanel : QUIBehaviour
	{
        private int sectionNu = 4;
        private Button[] buttons;

		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UISectionPanelData;
           
			//please add init code here
            SetButtons();
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
        {
            if(buttons != null){
                for (int i = 0; i < buttons.Length; i++)
                {
                    int index = i;
                    buttons[i].onClick.AddListener(() => { ChoiceSection(index); });
                }
            }

            settingBtn.onClick.AddListener(() => {
                UIMgr.OpenPanel<UISettingPanel>(UILevel.PopUI, prefabName: "Resources/UISettingPanel");
            });
            backBtn.onClick.AddListener(() => { 
                UIMgr.OpenPanel<UIMenuPanel>(UILevel.PopUI, prefabName: "Resources/UIMenuPanel");
                CloseSelf(); });
		}

		protected override void OnShow()
		{
			base.OnShow();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
			base.OnClose();
		}

        private void SetButtons(){
            buttons = new Button[sectionNu];
            for (int i = 0; i < buttons.Length; i++)
            {
                GameObject obj = Instantiate(SectionBtn.gameObject) as GameObject;
                obj.transform.parent = SectionBtn.gameObject.transform.parent;
                obj.transform.localScale = Vector3.one;
                obj.transform.position = Vector3.one;
                string btnName = "Section" + (i+1).ToString();
                obj.name = btnName;
                obj.Show();
                obj.GetComponentInChildren<Text>().text = btnName;
                buttons[i] = obj.GetComponent<Button>();
            }
        }

        private void ChoiceSection(int i ){
            Hide();
            UIMgr.OpenPanel<UIGamePanel>(UILevel.Common,new UIGamePanelData(i+1), prefabName: "Resources/UIGamePanel");
        }

		void ShowLog(string content)
		{
			Debug.Log("[ UISectionPanel:]" + content);
		}

		UISectionPanelData mData = null;
	}
}