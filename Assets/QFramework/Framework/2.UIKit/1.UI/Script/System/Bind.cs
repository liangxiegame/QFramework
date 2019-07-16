using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [AddComponentMenu("QF-UIKit/Bind")]
    public class Bind : MonoBehaviour, IBind
    {
        [HideInInspector] public BindType MarkType = BindType.DefaultUnityElement;

        public string Comment
        {
            get { return CustomComment; }
        }

        public Transform Transform
        {
            get { return transform; }
        }

        [HideInInspector] public string CustomComponentName;

        [HideInInspector] public string ComponentGeneratePath;

        [HideInInspector] public string CustomComment;

        public BindType GetBindType()
        {
            return MarkType;
        }

        public virtual string ComponentName
        {
            get
            {
                if (MarkType == BindType.DefaultUnityElement)
                {
                    if (null != GetComponent("SkeletonAnimation"))
                        return "SkeletonAnimation";
                    if (null != GetComponent<ScrollRect>())
                        return "ScrollRect";
                    if (null != GetComponent<InputField>())
                        return "InputField";
                    if (null != GetComponent("TMP.TextMeshProUGUI"))
                        return "TextMeshProUGUI";
                    if (null != GetComponent<Button>())
                        return "Button";
                    if (null != GetComponent<Text>())
                        return "Text";
                    if (null != GetComponent<RawImage>())
                        return "RawImage";
                    if (null != GetComponent<Toggle>())
                        return "Toggle";
                    if (null != GetComponent<Slider>())
                        return "Slider";
                    if (null != GetComponent<Scrollbar>())
                        return "Scrollbar";
                    if (null != GetComponent<Image>())
                        return "Image";
                    if (null != GetComponent<ToggleGroup>())
                        return "ToggleGroup";
                    if (null != GetComponent<Animator>())
                        return "Animator";
                    if (null != GetComponent<Canvas>())
                        return "Canvas";
                    if (null != GetComponent("Empty4Raycast"))
                        return "Empty4Raycast";
                    if (null != GetComponent<RectTransform>())
                        return "RectTransform";
                    if (GetComponent<MeshRenderer>())
                    {
                        return "MeshRenderer";
                    }

                    if (GetComponent<SpriteRenderer>())
                    {
                        return "SpriteRenderer";
                    }

                    // NGUI 支持
                    if (GetComponent("UIButton")) return "UIButton";
                    if (GetComponent("UILabel")) return "UILabel";

                    if (GetComponent<ViewController>())
                    {
                        var script = GetComponent<ViewController>();
                        return script.GetType().FullName;
                    }

                    return "Transform";
                }

                return CustomComponentName;
            }
        }
    }
}