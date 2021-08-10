using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QFramework
{
    [ExecuteInEditMode]
    public class EditorReload
    {
        [DidReloadScripts]
        static void OnReload()
        {
            FromUnityToDll.Setting = new SettingFromUnityToDll();
            // ReSharper disable once RedundantAssignment
            var architectureForInit = EasyIMGUI.Interface;
            architectureForInit = XMLKit.Interface;
            var convertSystem = architectureForInit.GetSystem<IXMLToObjectConvertSystem>();
            var convertModule = convertSystem.GetConvertModule("EasyIMGUI");
            convertModule.RegisterConverter("SupportItem",
                new CustomXMLToObjectConverter<AdvertisementItemView>(node =>
                    new AdvertisementItemView(node.Attributes["Title"].Value, node.Attributes["Link"].Value)));
        }

        static EditorReload()
        {
#if UNITY_EDITOR
            OnReload();
#endif
        }

        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        static void OnLoad()
        {
        }
    }
}