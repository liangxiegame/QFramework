using QFramework.Example;
using UnityEngine;

namespace QFramework
{
    public class VersionCheck : MonoBehaviour
    {
        private void Start()
        {
            
            
            // IVersionCheckStrategy strategy =  new SimulateVersionCheckStrategy();
            IVersionCheckStrategy strategy =  new RealVersionCheckStrategy();
            
            strategy.VersionCheck((hasNewVersion,localVersion,serverVersion) =>
            {
                if (hasNewVersion)
                {
                    UIKit.OpenPanel<UIHotFixCheckPanel>(new UIHotFixCheckPanelData()
                    {
                        ServerVersion = serverVersion.Version.ToString(),

                        LocalVersion = localVersion.Version.ToString(),

                        OnCancel = () =>
                        {
                            UIKit.ClosePanel("resources://UIHotFixCheckPanel");
                            GetComponent<ILKitBehaviour>().Enable();
                        },

                        OnOk = () =>
                        {
                            strategy.UpdateRes(() =>
                            {
                                ILRuntimeScriptSetting.LoadDLLFromStreamingAssetsPath = false;
                                AssetBundleSettings.LoadAssetResFromStreamingAssetsPath = false;

                                UIKit.ClosePanel("resources://UIHotFixCheckPanel");
                                GetComponent<ILKitBehaviour>().Enable();
                            
                            });

                        }
                    }, prefabName: "resources://UIHotFixCheckPanel");
                }
                else
                {
                    GetComponent<ILKitBehaviour>().Enable();
                }
            });
        }
    }
}