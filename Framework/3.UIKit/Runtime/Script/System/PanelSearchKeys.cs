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


        public override string ToString()
        {
            return "PanelSearchKeys PanelType:{0} AssetBundleName:{1} GameObjName:{2} Level:{3} UIData:{4}".FillFormat(PanelType, AssetBundleName, GameObjName, Level,
                UIData);
        }

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