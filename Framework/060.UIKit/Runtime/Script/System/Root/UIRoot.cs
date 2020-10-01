using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [MonoSingletonPath("UIRoot")]
    public class UIRoot : MonoSingleton<UIRoot>
    {
        public Transform BgTrans;
        public Transform AnimationUnderTrans;
        public Transform CommonTrans;
        public Transform AnimationOnTrans;
        public Transform PopUITrans;
        public Transform ConstTrans;
        public Transform ToastTrans;
        public Transform ForwardTrans;

        public Camera UICamera;
        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;

        private new static UIRoot mInstance;

        public new static UIRoot Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = FindObjectOfType<UIRoot>();
                }

                if (null == mInstance)
                {
                    Instantiate(Resources.Load<GameObject>("UIRoot"));
                    mInstance = MonoSingletonProperty<UIRoot>.Instance;
                    mInstance.name = "UIRoot";
                    DontDestroyOnLoad(mInstance);
                }

                return mInstance;
            }
        }


        public Camera Camera
        {
            get { return UICamera; }
        }

        public void SetResolution(int width, int height, float matchOnWidthOrHeight)
        {
            CanvasScaler.referenceResolution = new Vector2(width, height);
            CanvasScaler.matchWidthOrHeight = matchOnWidthOrHeight;
        }

        public Vector2 GetResolution()
        {
            return CanvasScaler.referenceResolution;
        }

        public float GetMatchOrWidthOrHeight()
        {
            return CanvasScaler.matchWidthOrHeight;
        }

        public void SetLevelOfPanel(UILevel level, IPanel panel)
        {
            switch (level)
            {
                case UILevel.Bg:
                    panel.Transform.SetParent(UIKit.Root.BgTrans);
                    break;
                case UILevel.AnimationUnderPage:
                    panel.Transform.SetParent(UIKit.Root.AnimationUnderTrans);
                    break;
                case UILevel.Common:
                    panel.Transform.SetParent(UIKit.Root.CommonTrans);
                    break;
                case UILevel.AnimationOnPage:
                    panel.Transform.SetParent(UIKit.Root.AnimationOnTrans);
                    break;
                case UILevel.PopUI:
                    panel.Transform.SetParent(UIKit.Root.PopUITrans);
                    break;
                case UILevel.Const:
                    panel.Transform.SetParent(UIKit.Root.ConstTrans);
                    break;
                case UILevel.Toast:
                    panel.Transform.SetParent(UIKit.Root.ToastTrans);
                    break;
                case UILevel.Forward:
                    panel.Transform.SetParent(UIKit.Root.ForwardTrans);
                    break;
            }
        }
    }
}