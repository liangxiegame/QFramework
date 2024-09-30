using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [MonoSingletonPath("QFramework/ActionKit/ScreenTransitionCanvas")]
    internal class ScreenTransitionCanvas :MonoBehaviour, ISingleton
    {
        internal static ScreenTransitionCanvas Instance =>
            PrefabSingletonProperty<ScreenTransitionCanvas>.InstanceWithLoader(
                Resources.Load<GameObject>);
        
        public Image ColorImage;
        
        public void OnSingletonInit()
        {
            
        }
    }
}
