using QFramework.Example;
using UnityEngine;

namespace QFramework
{
    public class UIPanelWithImagePrefabExample : MonoBehaviour
    {
        private void Start()
        {
            ResKit.Init();

            UIKit.OpenPanel<UIPanelWithImagePrefab>();
        }
    }
}
