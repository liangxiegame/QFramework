/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public static class BehaviourExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var component = gameObject.GetComponent<MonoBehaviour>();

            component.Enable(); // component.enabled = true
            component.Disable(); // component.enabled = false
        }

        public static T Enable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = true;
            return selfBehaviour;
        }

        public static T Disable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = false;
            return selfBehaviour;
        }
    }

    public static class CameraExtension
    {
        public static void Example()
        {
            var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
            Debug.Log(screenshotTexture2D);
        }

        public static Texture2D CaptureCamera(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(renderTexture);

            return screenShot;
        }
    }

    public static class ColorExtension
    {
        public static void Example()
        {
            var color = "#C5563CFF".HtmlStringToColor();
            Log.I(color);
        }

        /// <summary>
        /// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static Color HtmlStringToColor(this string htmlString)
        {
            var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out var retColor);
            return parseSucceed ? retColor : Color.black;
        }
    }

    public static class GraphicExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image = gameObject.AddComponent<Image>();
            var rawImage = gameObject.AddComponent<RawImage>();

            // image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
            image.ColorAlpha(1.0f);
            rawImage.ColorAlpha(1.0f);
        }

        public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
        {
            var color = selfGraphic.color;
            color.a = alpha;
            selfGraphic.color = color;
            return selfGraphic;
        }
    }

    public static class ImageExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image1 = gameObject.AddComponent<Image>();

            image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;
        }

        public static Image FillAmount(this Image selfImage, float fillamount)
        {
            selfImage.fillAmount = fillamount;
            return selfImage;
        }
    }

    public static class LightmapExtension
    {
        public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
        {
            RenderSettings.ambientLight = htmlStringColor.HtmlStringToColor();
        }
    }
    

    public static class RectTransformExtension
    {
        public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
        }

        public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.x = anchorPosX;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform AnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.y = anchorPosY;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.x = sizeWidth;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.y = sizeHeight;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
        }
    }

    public static class SelectableExtension
    {
        public static T EnableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = true;
            return selfSelectable;
        }

        public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = false;
            return selfSelectable;
        }

        public static T CancalAllTransitions<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.transition = Selectable.Transition.None;
            return selfSelectable;
        }
    }

    public static class ToggleExtension
    {
        public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
        {
            selfToggle.onValueChanged.AddListener(onValueChangedEvent);
        }
    }
    

    public static class UnityActionExtension
    {
        public static void Example()
        {
            UnityAction action = () => { };
            UnityAction<int> actionWithInt = num => { };
            UnityAction<int, string> actionWithIntString = (num, str) => { };

            action.InvokeGracefully();
            actionWithInt.InvokeGracefully(1);
            actionWithIntString.InvokeGracefully(1, "str");
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully(this UnityAction selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得随机列表中元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }


        /// <summary>
        /// 根据权值来获取索引
        /// </summary>
        /// <param name="powers"></param>
        /// <returns></returns>
        public static int GetRandomWithPower(this List<int> powers)
        {
            var sum = 0;
            foreach (var power in powers)
            {
                sum += power;
            }

            var randomNum = UnityEngine.Random.Range(0, sum);
            var currentSum = 0;
            for (var i = 0; i < powers.Count; i++)
            {
                var nextSum = currentSum + powers[i];
                if (randomNum >= currentSum && randomNum <= nextSum)
                {
                    return i;
                }

                currentSum = nextSum;
            }

            Log.E("权值范围计算错误！");
            return -1;
        }

        /// <summary>
        /// 根据权值获取值，Key为值，Value为权值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="powersDict"></param>
        /// <returns></returns>
        public static T GetRandomWithPower<T>(this Dictionary<T, int> powersDict)
        {
            var keys = new List<T>();
            var values = new List<int>();

            foreach (var key in powersDict.Keys)
            {
                keys.Add(key);
                values.Add(powersDict[key]);
            }

            var finalKeyIndex = values.GetRandomWithPower();
            return keys[finalKeyIndex];
        }
    }

    public static class AnimatorExtension
    {
        public static void AddAnimatorParameterIfExists(this Animator animator, string parameterName,
            AnimatorControllerParameterType type, List<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }

        // <summary>
        /// Updates the animator bool.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBool(this Animator self, string parameterName, bool value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetBool(parameterName, value);
            }
        }

        public static void UpdateAnimatorTrigger(this Animator self, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Triggers an animator trigger.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTrigger(this Animator self, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloat(this Animator self, string parameterName, float value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer.
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorInteger(this Animator self, string parameterName, int value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetInteger(parameterName, value);
            }
        }


        // <summary>
        /// Updates the animator bool without checking the parameter's existence.
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBool(this Animator self, string parameterName, bool value)
        {
            self.SetBool(parameterName, value);
        }

        /// <summary>
        /// Updates the animator trigger without checking the parameter's existence
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        public static void UpdateAnimatorTrigger(this Animator self, string parameterName)
        {
            self.SetTrigger(parameterName);
        }

        /// <summary>
        /// Triggers an animator trigger without checking for the parameter's existence.
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTrigger(this Animator self, string parameterName)
        {
            self.SetTrigger(parameterName);
        }

        /// <summary>
        /// Updates the animator float without checking for the parameter's existence.
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloat(this Animator self, string parameterName, float value)
        {
            self.SetFloat(parameterName, value);
        }

        /// <summary>
        /// Updates the animator integer without checking for the parameter's existence.
        /// </summary>
        /// <param name="self">self.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorInteger(this Animator self, string parameterName, int value)
        {
            self.SetInteger(parameterName, value);
        }


        // <summary>
        /// Updates the animator bool after checking the parameter's existence.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBoolIfExists(this Animator self, string parameterName, bool value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                self.SetBool(parameterName, value);
            }
        }

        public static void UpdateAnimatorTriggerIfExists(this Animator self, string parameterName)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                self.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Triggers an animator trigger after checking for the parameter's existence.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTriggerIfExists(this Animator self, string parameterName)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                self.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float after checking for the parameter's existence.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloatIfExists(this Animator self, string parameterName, float value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                self.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer after checking for the parameter's existence.
        /// </summary>
        /// <param name="self">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorIntegerIfExists(this Animator self, string parameterName, int value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                self.SetInteger(parameterName, value);
            }
        }

        /// <summary>
        /// Determines if an animator contains a certain parameter, based on a type and a name
        /// </summary>
        /// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
        /// <param name="self">Self.</param>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var parameters = self.parameters;
            return parameters.Any(currParam => currParam.type == type && currParam.name == name);
        }
    }
    
    public static class LayerMaskExtension
    {
        public static bool ContainsGameObject(this LayerMask selfLayerMask, GameObject gameObject)
        {
            return LayerMaskUtility.IsInLayerMask(gameObject, selfLayerMask);
        }
    }

    public static class LayerMaskUtility
    {
        public static bool IsInLayerMask(GameObject gameObj, LayerMask layerMask)
        {
            // 根据Layer数值进行移位获得用于运算的Mask值
            var objLayerMask = 1 << gameObj.layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }
    }

    public static class MaterialExtension
    {
        /// <summary>
        /// 参考资料: https://blog.csdn.net/qiminixi/article/details/78402505
        /// </summary>
        /// <param name="self"></param>
        public static void SetStandardMaterialToTransparentMode(this Material self)
        {
            self.SetFloat("_Mode", 3);
            self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            self.SetInt("_ZWrite", 0);
            self.DisableKeyword("_ALPHATEST_ON");
            self.EnableKeyword("_ALPHABLEND_ON");
            self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            self.renderQueue = 3000;
        }
    }

    public static class TextureExtensions
    {
        public static Sprite CreateSprite(this Texture2D self)
        {
            return Sprite.Create(self, new Rect(0, 0, self.width, self.height), Vector2.one * 0.5f);
        }
    }

    internal static class Log
    {
        public enum LogLevel
        {
            None = 0,
            Exception = 1,
            Error = 2,
            Warning = 3,
            Normal = 4,
            Max = 5,
        }


        internal static void LogInfo(this object selfMsg)
        {
            I(selfMsg);
        }

        internal static void LogWarning(this object selfMsg)
        {
            W(selfMsg);
        }

        internal static void LogError(this object selfMsg)
        {
            E(selfMsg);
        }

        internal static void LogException(this Exception selfExp)
        {
            E(selfExp);
        }

        private static LogLevel mLogLevel = LogLevel.Normal;

        public static LogLevel Level
        {
            get { return mLogLevel; }
            set { mLogLevel = value; }
        }

        internal static void I(object msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Normal)
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                Debug.Log(msg);
            }
            else
            {
                Debug.LogFormat(msg.ToString(), args);
            }
        }

        internal static void E(Exception e)
        {
            if (mLogLevel < LogLevel.Exception)
            {
                return;
            }

            Debug.LogException(e);
        }

        internal static void E(object msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Error)
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                Debug.LogError(msg);
            }
            else
            {
                Debug.LogError(string.Format(msg.ToString(), args));
            }
        }

        internal static void W(object msg)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

            Debug.LogWarning(msg);
        }

        internal static void W(string msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

            Debug.LogWarning(string.Format(msg, args));
        }
    }
}