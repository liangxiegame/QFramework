using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using UnityEditor;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{
    public class PackageControlPanel : EditorWindow
    {

        public UFramePackageDescriptor Package { get; set; }

        void OnGUI()
        {
            //if(Package!=null)
            //InvertApplication.SignalEvent<IDrawPackageControlPanel>(_ => _.DrawControlPanel(new Rect(0, 0, Screen.width, Screen.height),Package));
        }

        void Update()
        {
            Repaint();
        }

    }
}