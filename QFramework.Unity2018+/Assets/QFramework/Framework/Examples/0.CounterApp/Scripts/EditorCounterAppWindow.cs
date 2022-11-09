#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework.Example
{
    public class EditorCounterAppWindow : EditorWindow,IController
    {

        [MenuItem("QFramework/Example/EditorCounterAppWindow")]
        static void Open()
        {
            GetWindow<EditorCounterAppWindow>().Show();
        }
        
        private ICounterAppModel mCounterAppModel;

        private void OnEnable()
        {
            mCounterAppModel = this.GetModel<ICounterAppModel>();
        }

        private void OnDisable()
        {
            mCounterAppModel = null;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("+"))
            {
                this.SendCommand<IncreaseCountCommand>();
            }
            
            GUILayout.Label(mCounterAppModel.Count.Value.ToString());


            if (GUILayout.Button("-"))
            {
                this.SendCommand<DecreaseCountCommand>();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }
    }
}
#endif