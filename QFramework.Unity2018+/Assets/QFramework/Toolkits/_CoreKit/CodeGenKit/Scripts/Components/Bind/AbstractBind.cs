/****************************************************************************
 * Copyright (c) 2015 ~ 2023 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public abstract class AbstractBind : MonoBehaviour, IBindOld
    {
        [HideInInspector] public BindType MarkType = BindType.DefaultUnityElement;

        public string Comment => CustomComment;

        public Transform Transform => transform;

        [HideInInspector] public string CustomComponentName;


        [HideInInspector] public string CustomComment;

        public BindType GetBindType()
        {
            return MarkType;
        }

        [HideInInspector] [SerializeField] private string mComponentName;

        public virtual string TypeName
        {
            get
            {
                if (MarkType == BindType.DefaultUnityElement)
                {
                    if (string.IsNullOrEmpty(mComponentName))
                    {
                        mComponentName = GetDefaultComponentName();
                    }

                    return mComponentName;
                }

                if (MarkType == BindType.Element || MarkType == BindType.Component)
                {
                    return CustomComponentName;
                }

                return mComponentName;
            }
        }

        /// <summary>
        /// 组件获得优先级，可以节省一部分操作时间
        /// </summary>
        /// <returns></returns>
        string GetDefaultComponentName()
        {
            if (GetComponent<ViewController>()) return GetComponent<ViewController>().GetType().FullName;


            if (GetComponent("SkeletonAnimation")) return "SkeletonAnimation";
            if (GetComponent<ScrollRect>()) return "UnityEngine.UI.ScrollRect";
            if (GetComponent<InputField>()) return "UnityEngine.UI.InputField";

            // text mesh pro supported
            if (GetComponent("TMP.TextMeshProUGUI")) return "TMP.TextMeshProUGUI";
            if (GetComponent("TMPro.TextMeshProUGUI")) return "TMPro.TextMeshProUGUI";
            if (GetComponent("TMPro.TextMeshPro")) return "TMPro.TextMeshPro";
            if (GetComponent("TMPro.TMP_InputField")) return "TMPro.TMP_InputField";

            // ugui bind
            if (GetComponent<Dropdown>()) return "UnityEngine.UI.Dropdown";
            if (GetComponent<Button>()) return "UnityEngine.UI.Button";
            if (GetComponent<Text>()) return "UnityEngine.UI.Text";
            if (GetComponent<RawImage>()) return "UnityEngine.UI.RawImage";
            if (GetComponent<Toggle>()) return "UnityEngine.UI.Toggle";
            if (GetComponent<Slider>()) return "UnityEngine.UI.Slider";
            if (GetComponent<Scrollbar>()) return "UnityEngine.UI.Scrollbar";
            if (GetComponent<Image>()) return "UnityEngine.UI.Image";
            if (GetComponent<ToggleGroup>()) return "UnityEngine.UI.ToggleGroup";

            // other
            if (GetComponent<Rigidbody>()) return "Rigidbody";
            if (GetComponent<Rigidbody2D>()) return "Rigidbody2D";

            if (GetComponent<BoxCollider2D>()) return "BoxCollider2D";
            if (GetComponent<BoxCollider>()) return "BoxCollider";
            if (GetComponent<CircleCollider2D>()) return "CircleCollider2D";
            if (GetComponent<SphereCollider>()) return "SphereCollider";
            if (GetComponent<MeshCollider>()) return "MeshCollider";

            if (GetComponent<Collider>()) return "Collider";
            if (GetComponent<Collider2D>()) return "Collider2D";

            if (GetComponent<Animator>()) return "Animator";
            if (GetComponent<Canvas>()) return "Canvas";
            if (GetComponent<Camera>()) return "Camera";
            // if (GetComponent("Empty4Raycast")) return "QFramework.Empty4Raycast";
            if (GetComponent<RectTransform>()) return "RectTransform";
            if (GetComponent<MeshRenderer>()) return "MeshRenderer";

            if (GetComponent<SpriteRenderer>()) return "SpriteRenderer";

            // NGUI 支持
#if NGUI
            if (GetComponent("UIButton")) return "UIButton";
            if (GetComponent("UILabel")) return "UILabel";
            if (GetComponent("UISprite")) return "UISprite";
            if (GetComponent("UISlider")) return "UISlider";
            if (GetComponent("UITexture")) return "UITexture";
#endif

            return "Transform";
        }
    }
}