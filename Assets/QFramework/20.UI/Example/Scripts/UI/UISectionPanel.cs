using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.UIExample
{
	public class UISelectionPanelData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISelectionPanel : QUIBehaviour
	{
		private const int SectionNumber = 4;

		private readonly Button[] mButtons = new Button[SectionNumber];

		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UISelectionPanelData;

			for (var i = 0; i < mButtons.Length; i++)
			{
				mButtons[i] = SectionBtn
					.Instantiate()
					.Parent(SectionBtn.transform.parent)
					.LocalScaleIdentity()
					.LocalPositionIdentity()
					.Name("Section" + (i + 1))
					.Show()
					.ApplySelfTo(selfBtn => selfBtn.GetComponentInChildren<Text>().text = selfBtn.name)
					.ApplySelfTo(selfBtn =>
					{
						var index = i;
						selfBtn.onClick.AddListener(() => { ChoiceSection(index); });
					});
			}
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException();
		}

		protected override void RegisterUIEvent()
		{

			settingBtn.onClick.AddListener(() =>
			{
				UIMgr.OpenPanel<UISettingPanel>(UILevel.PopUI, prefabName: "Resources/UISettingPanel");
			});
			backBtn.onClick.AddListener(() =>
			{
				UIMgr.OpenPanel<UIMenuPanel>(UILevel.PopUI, prefabName: "Resources/UIMenuPanel");
				CloseSelf();
			});
		}

		private void ChoiceSection(int i)
		{
			CloseSelf();
			UIMgr.OpenPanel<UIGamePanel>(UILevel.Common, new UIGamePanelData(i + 1), prefabName: "Resources/UIGamePanel");
		}

		UISelectionPanelData mData = null;
	}
}