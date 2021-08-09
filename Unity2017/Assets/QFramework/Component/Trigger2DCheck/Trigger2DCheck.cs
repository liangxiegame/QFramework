using UnityEngine;

namespace QFramework
{
    public class Trigger2DCheck : MonoBehaviour
    {
        public int EnterCount;

        public bool Triggered
        {
            get
            {
                return EnterCount > 0;
            }
        }

        public LayerMask TargetLayers;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInLayerMask(other.gameObject, TargetLayers))
            {
                EnterCount++;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsInLayerMask(other.gameObject, TargetLayers))
            {
                EnterCount--;
            }
        }
        
        private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            // 根据Layer数值进行移位获得用于运算的Mask值
            var objLayerMask = 1 << obj.layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }
    }
}