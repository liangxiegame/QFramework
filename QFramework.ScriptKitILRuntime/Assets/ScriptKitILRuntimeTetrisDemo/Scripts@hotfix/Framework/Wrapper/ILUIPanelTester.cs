using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

namespace QFramework
{
	public partial class ILUIPanelTester
	{
		void OnStart()
		{
			var panelName = SceneManager.GetActiveScene().name.Replace("Test", string.Empty);
			ResKit.Init();
			ILUIKit.OpenPanel(panelName);
		}

		void OnDestroy()
		{
			
		}
		

	}
}
