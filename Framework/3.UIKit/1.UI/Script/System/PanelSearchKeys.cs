using System;

namespace QFramework
{
    public class PanelSearchKeys : IPoolType, IPoolable
    {
        public Type PanelType;

        public string AssetBundleName;

        public string GameObjName;

        public UILevel Level = UILevel.Common;

        public IUIData UIData;


        public void OnRecycled()
        {
            PanelType = null;
            AssetBundleName = null;
            GameObjName = null;
            UIData = null;
        }

        public bool IsRecycled { get; set; }


        public static PanelSearchKeys Allocate()
        {
            return SafeObjectPool<PanelSearchKeys>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<PanelSearchKeys>.Instance.Recycle(this);
        }
    }
}