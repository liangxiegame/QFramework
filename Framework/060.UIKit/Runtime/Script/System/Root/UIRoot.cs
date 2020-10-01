using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [MonoSingletonPath("UIRoot")]
    public class UIRoot : MonoSingleton<UIRoot>
    {
        public Camera UICamera;
        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;

        private new static UIRoot mInstance;

        public new static UIRoot Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = FindObjectOfType<UIRoot>();
                }

                if (!mInstance)
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

        void MakeSureCanvas(UILevel level,IPanel panel)
        {
            var canvas = panel.Transform.GetComponent<Canvas>();

            if (!canvas)
            {
                canvas = panel.Transform.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int) level;
                
                panel.Transform.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        
        public void SetLevelOfPanel(UILevel level, IPanel panel)
        {
            panel.Transform.SetParent(transform);
            MakeSureCanvas(level, panel);
        }
    }
}