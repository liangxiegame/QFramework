using UnityEngine;
using QFramework;

public class UIHomePage : UIPanel
{
	protected override void OnClose()
	{
		
	}

	protected override void OnInit(IUIData uiData = null)
	{
		mUIComponents.BtnQuitGame_Button.onClick.AddListener(delegate { Debug.Log("BtnQuitGameClick"); });

		mUIComponents.BtnAbout_Button.onClick.AddListener(delegate { Debug.Log("BtnAboutClick"); });

		mUIComponents.BtnStart_Button.onClick.AddListener(delegate { Debug.Log("BtnStartClick"); });	
	}


	UIHomePageComponents mUIComponents = null;
}