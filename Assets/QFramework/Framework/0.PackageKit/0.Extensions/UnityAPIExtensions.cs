/****************************************************************************
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace QF.Extensions
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    public static class AnimatorExtension
    {
        public static void OnCurrentStateComplete(this Animator self,Action onCompleted )
        {
            DoCeckCurrentAnimationComplete(self, onCompleted)
                .ToObservable()
                .Subscribe()
                .AddTo(self);
        }
        
        static IEnumerator DoCeckCurrentAnimationComplete(this Animator self,Action onCompleted )
        {
            while (self.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                yield return null;
            }

            if (onCompleted != null && !self.IsInTransition(0))
            {
                onCompleted();
            }
        }
    }
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
            Debug.Log(screenshotTexture2D.width);
        }

        public static Texture2D CaptureCamera(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(renderTexture);

            return screenShot;
        }
    }

    public static class ColorExtension
    {
        public static void Example()
        {
            var color = "#C5563CFF".HtmlStringToColor();
            Debug.Log(color);
        }

        /// <summary>
        /// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static Color HtmlStringToColor(this string htmlString)
        {
            Color retColor;
            var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);
            return parseSucceed ? retColor : Color.black;
        }

        /// <summary>
        /// unity's color always new a color
        /// </summary>
        public static Color White = Color.white;
    }

    /// <summary>
    /// GameObject's Util/Static This Extension
    /// </summary>
    public static class GameObjectExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var transform = gameObject.transform;
            var selfScript = gameObject.AddComponent<MonoBehaviour>();
            var boxCollider = gameObject.AddComponent<BoxCollider>();

            gameObject.Show(); // gameObject.SetActive(true)
            selfScript.Show(); // this.gameObject.SetActive(true)
            boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
            gameObject.transform.Show(); // transform.gameObject.SetActive(true)

            gameObject.Hide(); // gameObject.SetActive(false)
            selfScript.Hide(); // this.gameObject.SetActive(false)
            boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
            transform.Hide(); // transform.gameObject.SetActive(false)

            selfScript.DestroyGameObj();
            boxCollider.DestroyGameObj();
            transform.DestroyGameObj();

            selfScript.DestroyGameObjGracefully();
            boxCollider.DestroyGameObjGracefully();
            transform.DestroyGameObjGracefully();

            selfScript.DestroyGameObjAfterDelay(1.0f);
            boxCollider.DestroyGameObjAfterDelay(1.0f);
            transform.DestroyGameObjAfterDelay(1.0f);

            selfScript.DestroyGameObjAfterDelayGracefully(1.0f);
            boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
            transform.DestroyGameObjAfterDelayGracefully(1.0f);

            gameObject.Layer(0);
            selfScript.Layer(0);
            boxCollider.Layer(0);
            transform.Layer(0);

            gameObject.Layer("Default");
            selfScript.Layer("Default");
            boxCollider.Layer("Default");
            transform.Layer("Default");
        }

        #region CEGO001 Show

        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        public static T Show<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Show();
            return selfComponent;
        }

        #endregion

        #region CEGO002 Hide

        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        public static T Hide<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Hide();
            return selfComponent;
        }

        #endregion

        #region CEGO003 DestroyGameObj

        public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
        {
            selfBehaviour.gameObject.DestroySelf();
        }

        #endregion

        #region CEGO004 DestroyGameObjGracefully

        public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfGracefully();
            }
        }

        #endregion

        #region CEGO005 DestroyGameObjGracefully

        public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            return selfBehaviour;
        }

        public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            }

            return selfBehaviour;
        }

        #endregion

        #region CEGO006 Layer

        public static GameObject Layer(this GameObject selfObj, int layer)
        {
            selfObj.layer = layer;
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, int layer) where T : Component
        {
            selfComponent.gameObject.layer = layer;
            return selfComponent;
        }

        public static GameObject Layer(this GameObject selfObj, string layerName)
        {
            selfObj.layer = LayerMask.NameToLayer(layerName);
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, string layerName) where T : Component
        {
            selfComponent.gameObject.layer = LayerMask.NameToLayer(layerName);
            return selfComponent;
        }

        #endregion

        #region CEGO007 Component

        public static T GetOrAddComponent<T>(this GameObject selfComponent) where T : Component
        {
            var comp = selfComponent.gameObject.GetComponent<T>();
            return comp ? comp : selfComponent.gameObject.AddComponent<T>();
        }

        public static Component GetOrAddComponent(this GameObject selfComponent, Type type)
        {
            var comp = selfComponent.gameObject.GetComponent(type);
            return comp ? comp : selfComponent.gameObject.AddComponent(type);
        }

        #endregion
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

    public static class ObjectExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();

            gameObject.Instantiate()
                .Name("ExtensionExample")
                .DestroySelf();

            gameObject.Instantiate()
                .DestroySelfGracefully();

            gameObject.Instantiate()
                .DestroySelfAfterDelay(1.0f);

            gameObject.Instantiate()
                .DestroySelfAfterDelayGracefully(1.0f);

            gameObject
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("TestObj")
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("ExtensionExample")
                .DontDestroyOnLoad();
        }


        #region CEUO001 Instantiate

        public static T Instantiate<T>(this T selfObj) where T : Object
        {
            return Object.Instantiate(selfObj);
        }

        #endregion

        #region CEUO002 Instantiate

        public static T Name<T>(this T selfObj, string name) where T : Object
        {
            selfObj.name = name;
            return selfObj;
        }

        #endregion

        #region CEUO003 Destroy Self

        public static void DestroySelf<T>(this T selfObj) where T : Object
        {
            Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj);
            }

            return selfObj;
        }

        #endregion

        #region CEUO004 Destroy Self AfterDelay 

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : Object
        {
            Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

        #endregion

        #region CEUO005 Apply Self To 

        public static T ApplySelfTo<T>(this T selfObj, System.Action<T> toFunction) where T : Object
        {
            toFunction.InvokeGracefully(selfObj);
            return selfObj;
        }

        #endregion

        #region CEUO006 DontDestroyOnLoad

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
        {
            Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }

        #endregion

        public static T As<T>(this Object selfObj) where T : Object
        {
            return selfObj as T;
        }

        public static T LogInfo<T>(this T selfObj, string msgContent, params object[] args) where T : Object
        {
            Log.I(msgContent, args);
            return selfObj;
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

#if SLUA_SUPPORT
    using SLua;
    [CustomLuaClass]
#endif

    /// <summary>
    /// Transform's Extension
    /// </summary>
    public static class TransformExtension
    {
        public static void Example()
        {
            var selfScript = new GameObject().AddComponent<MonoBehaviour>();
            var transform = selfScript.transform;

            transform
                .Parent(null)
                .LocalIdentity()
                .LocalPositionIdentity()
                .LocalRotationIdentity()
                .LocalScaleIdentity()
                .LocalPosition(Vector3.zero)
                .LocalPosition(0, 0, 0)
                .LocalPosition(0, 0)
                .LocalPositionX(0)
                .LocalPositionY(0)
                .LocalPositionZ(0)
                .LocalRotation(Quaternion.identity)
                .LocalScale(Vector3.one)
                .LocalScaleX(1.0f)
                .LocalScaleY(1.0f)
                .Identity()
                .PositionIdentity()
                .RotationIdentity()
                .Position(Vector3.zero)
                .PositionX(0)
                .PositionY(0)
                .PositionZ(0)
                .Rotation(Quaternion.identity)
                .DestroyAllChild()
                .AsLastSibling()
                .AsFirstSibling()
                .SiblingIndex(0);

            selfScript
                .Parent(null)
                .LocalIdentity()
                .LocalPositionIdentity()
                .LocalRotationIdentity()
                .LocalScaleIdentity()
                .LocalPosition(Vector3.zero)
                .LocalPosition(0, 0, 0)
                .LocalPosition(0, 0)
                .LocalPositionX(0)
                .LocalPositionY(0)
                .LocalPositionZ(0)
                .LocalRotation(Quaternion.identity)
                .LocalScale(Vector3.one)
                .LocalScaleX(1.0f)
                .LocalScaleY(1.0f)
                .Identity()
                .PositionIdentity()
                .RotationIdentity()
                .Position(Vector3.zero)
                .PositionX(0)
                .PositionY(0)
                .PositionZ(0)
                .Rotation(Quaternion.identity)
                .DestroyAllChild()
                .AsLastSibling()
                .AsFirstSibling()
                .SiblingIndex(0);
        }


        /// <summary>
        /// 缓存的一些变量,免得每次声明
        /// </summary>
        private static Vector3 mLocalPos;

        private static Vector3 mScale;
        private static Vector3 mPos;

        #region CETR001 Parent

        public static T Parent<T>(this T selfComponent, Component parentComponent) where T : Component
        {
            selfComponent.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
            return selfComponent;
        }

        /// <summary>
        /// 设置成为顶端 Transform
        /// </summary>
        /// <returns>The root transform.</returns>
        /// <param name="selfComponent">Self component.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T AsRootTransform<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetParent(null);
            return selfComponent;
        }

        #endregion

        #region CETR002 LocalIdentity

        public static T LocalIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            selfComponent.transform.localRotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR003 LocalPosition

        public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
        {
            selfComponent.transform.localPosition = localPos;
            return selfComponent;
        }

        public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localPosition;
        }



        public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.localPosition = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.z = z;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }


        public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            return selfComponent;
        }

        #endregion

        #region CETR004 LocalRotation

        public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localRotation;
        }

        public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
        {
            selfComponent.transform.localRotation = localRotation;
            return selfComponent;
        }

        public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localRotation = Quaternion.identity;
            return selfComponent;
        }

        #endregion

        #region CETR005 LocalScale

        public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
        {
            selfComponent.transform.localScale = scale;
            return selfComponent;
        }

        public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localScale;
        }

        public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one * xyz;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR006 Identity

        public static T Identity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            selfComponent.transform.rotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR007 Position

        public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
        {
            selfComponent.transform.position = position;
            return selfComponent;
        }

        public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.position;
        }

        public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.position = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T Position<T>(this T selfComponent, float x, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, float x) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, Func<float, float> xSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = xSetter(mPos.x);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, Func<float, float> ySetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = ySetter(mPos.y);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = z;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, Func<float, float> zSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = zSetter(mPos.z);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        #endregion

        #region CETR008 Rotation

        public static T RotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.rotation = Quaternion.identity;
            return selfComponent;
        }

        public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
        {
            selfComponent.transform.rotation = rotation;
            return selfComponent;
        }

        public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.rotation;
        }

        #endregion

        #region CETR009 WorldScale/LossyScale/GlobalScale/Scale

        public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        #endregion

        #region CETR010 Destroy All Child

        public static T DestroyAllChild<T>(this T selfComponent) where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfComponent;
        }

        public static GameObject DestroyAllChild(this GameObject selfGameObj)
        {
            var childCount = selfGameObj.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfGameObj;
        }

        #endregion

        #region CETR011 Sibling Index

        public static T AsLastSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsLastSibling();
            return selfComponent;
        }

        public static T AsFirstSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsFirstSibling();
            return selfComponent;
        }

        public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
        {
            selfComponent.transform.SetSiblingIndex(index);
            return selfComponent;
        }

        #endregion


        public static Transform FindByPath(this Transform selfTrans, string path)
        {
            return selfTrans.Find(path.Replace(".", "/"));
        }

        public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
        {
            var childTrans = selfTransform.Find(uniqueName);

            if (null != childTrans)
                return childTrans;

            foreach (Transform trans in selfTransform)
            {
                childTrans = trans.SeekTrans(uniqueName);

                if (null != childTrans)
                    return childTrans;
            }

            return null;
        }

        public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).gameObject.Show();
            return selfComponent;
        }

        public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).Hide();
            return selfComponent;
        }

        public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
        {
            selfTrans.SetParent(fromTrans.parent);
            selfTrans.localPosition = fromTrans.localPosition;
            selfTrans.localRotation = fromTrans.localRotation;
            selfTrans.localScale = fromTrans.localScale;
        }

        /// <summary>
        /// 递归遍历子物体，并调用函数
        /// </summary>
        /// <param name="tfParent"></param>
        /// <param name="action"></param>
        public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
        {
            action(tfParent);
            foreach (Transform tfChild in tfParent)
            {
                tfChild.ActionRecursion(action);
            }
        }

        /// <summary>
        /// 递归遍历查找指定的名字的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="name">目标名</param>
        /// <param name="stringComparison">字符串比较规则</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, string name,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (tfParent.name.Equals(name, stringComparison))
            {
                //Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(name, stringComparison);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        /// <summary>
        /// 递归遍历查找相应条件的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(predicate);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
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
            return list[UnityEngine.Random.Range(0, list.Count - 1)];
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
}