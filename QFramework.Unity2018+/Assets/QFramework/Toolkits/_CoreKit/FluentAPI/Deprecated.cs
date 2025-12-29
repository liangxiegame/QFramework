/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace QFramework
{
    /// 程序集工具
    /// </summary>
    public class AssemblyUtil
    {
        /// <summary>
        /// 获取 Assembly-CSharp 程序集
        /// </summary>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Assembly DefaultCSharpAssembly
        {
            get
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(a => a.GetName().Name == "Assembly-CSharp");
            }
        }

        /// <summary>
        /// 获取默认的程序集中的类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Type GetDefaultAssemblyType(string typeName)
        {
            return DefaultCSharpAssembly.GetType(typeName);
        }
    }


    /// <summary>
    /// 简单的概率计算
    /// </summary>
    public static class ProbilityHelper
    {
        [Obsolete("请使用 action?.Invoke(),please use action?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully(this UnityAction selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        [Obsolete("请使用 action?.Invoke(),please use action?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        [Obsolete("请使用 action?.Invoke(),please use action?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T RandomValueFrom<T>(params T[] values)
        {
            return values[Random.Range(0, values.Length)];
        }

        /// <summary>
        /// percent probability
        /// </summary>
        /// <param name="percent"> 0 ~ 100 </param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool PercentProbability(int percent)
        {
            return Random.Range(0, 1000) * 0.001f < 50 * 0.01f;
        }
    }

    public static class ReflectionExtension
    {
        public static Assembly GetAssemblyCSharp()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
            return null;
        }

        public static Assembly GetAssemblyCSharpEditor()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp-Editor,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp-Editor.dll");
            return null;
        }
    }

    /// <summary>
    /// Write in unity 2017 .Net 3.5
    /// after unity 2018 .Net 4.x and new C# version are more powerful
    /// </summary>
    public static class DeprecatedExtension
    {
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static int GetRandomWithPower(this List<int> powers)
        {
            var sum = 0;
            foreach (var power in powers)
            {
                sum += power;
            }

            var randomNum = Random.Range(0, sum);
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

            LogKit.E("权值范围计算错误！");
            return -1;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
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
        
        
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void AddAnimatorParameterIfExists(this Animator animator, string parameterName,
            AnimatorControllerParameterType type, List<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorBool(this Animator self, string parameterName, bool value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetBool(parameterName, value);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorTrigger(this Animator self, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetTrigger(parameterName);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void SetAnimatorTrigger(this Animator self, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetTrigger(parameterName);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorFloat(this Animator self, string parameterName, float value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetFloat(parameterName, value);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorInteger(this Animator self, string parameterName, int value,
            List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                self.SetInteger(parameterName, value);
            }
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorBool(this Animator self, string parameterName, bool value)
        {
            self.SetBool(parameterName, value);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorTrigger(this Animator self, string parameterName)
        {
            self.SetTrigger(parameterName);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void SetAnimatorTrigger(this Animator self, string parameterName)
        {
            self.SetTrigger(parameterName);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorFloat(this Animator self, string parameterName, float value)
        {
            self.SetFloat(parameterName, value);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorInteger(this Animator self, string parameterName, int value)
        {
            self.SetInteger(parameterName, value);
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorBoolIfExists(this Animator self, string parameterName, bool value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                self.SetBool(parameterName, value);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorTriggerIfExists(this Animator self, string parameterName)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                self.SetTrigger(parameterName);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void SetAnimatorTriggerIfExists(this Animator self, string parameterName)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                self.SetTrigger(parameterName);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorFloatIfExists(this Animator self, string parameterName, float value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                self.SetFloat(parameterName, value);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void UpdateAnimatorIntegerIfExists(this Animator self, string parameterName, int value)
        {
            if (self.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                self.SetInteger(parameterName, value);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var parameters = self.parameters;
            return parameters.Any(currParam => currParam.type == type && currParam.name == name);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T EnableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = true;
            return selfSelectable;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = false;
            return selfSelectable;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T CancelAllTransitions<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.transition = Selectable.Transition.None;
            return selfSelectable;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
        {
            selfToggle.onValueChanged.AddListener(onValueChangedEvent);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.x = anchorPosX;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }
        

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.x = sizeWidth;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.y = sizeHeight;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
        {
            RenderSettings.ambientLight = htmlStringColor.HtmlStringToColor();
        }

        /// <summary>
        /// 参考资料: https://blog.csdn.net/qiminixi/article/details/78402505
        /// </summary>
        /// <param name="self"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void SetStandardMaterialToTransparentMode(this Material self)
        {
            self.SetFloat("_Mode", 3);
            self.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            self.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            self.SetInt("_ZWrite", 0);
            self.DisableKeyword("_ALPHATEST_ON");
            self.EnableKeyword("_ALPHABLEND_ON");
            self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            self.renderQueue = 3000;
        }

        [Obsolete("请使用 gameObj.IsInLayerMask(layerMask)，use gameObj.IsInLayerMask(layerMask) instead", true)]
        public static bool ContainsGameObject(this LayerMask selfLayerMask, GameObject gameObject)
        {
            return gameObject.IsInLayerMask(selfLayerMask);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Sprite CreateSprite(this Texture2D self)
        {
            return Sprite.Create(self, new Rect(0, 0, self.width, self.height), Vector2.one * 0.5f);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
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

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).gameObject.Show();
            return selfComponent;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).Hide();
            return selfComponent;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
        {
            selfTrans.SetParent(fromTrans.parent);
            selfTrans.localPosition = fromTrans.localPosition;
            selfTrans.localRotation = fromTrans.localRotation;
            selfTrans.localScale = fromTrans.localScale;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
        {
            action(tfParent);
            foreach (Transform tfChild in tfParent)
            {
                tfChild.ActionRecursion(action);
            }
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
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

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                LogKit.I("Hit " + tfParent.name);
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

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetPath(this Transform transform)
        {
            var sb = new StringBuilder();
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

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static Transform FindByPath(this Transform selfTrans, string path)
        {
            return selfTrans.Find(path.Replace(".", "/"));
        }

        [Obsolete("弃用，请使用 Scale(), use Scale() instead", APIVersion.Force)]
        public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        [Obsolete("弃用，请使用 Scale(), use Scale() instead", APIVersion.Force)]
        public static Vector3 GetScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        [Obsolete("弃用，请使用 Scale(), use Scale() instead", APIVersion.Force)]
        public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        [Obsolete("弃用，请使用 Scale(), use Scale() instead", APIVersion.Force)]
        public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        [Obsolete("弃用，请使用 Rotation(), use Rotation() instead", APIVersion.Force)]
        public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.rotation;
        }


        [Obsolete("弃用啦 请使用 DestroyChildren,use DestroyChildren() instead")]
        public static T DestroyAllChild<T>(this T selfComponent) where T : Component
        {
            return selfComponent.DestroyChildren();
        }

        [Obsolete("弃用啦 请使用 DestroyChildren,use DestroyChildren() instead")]
        public static GameObject DestroyAllChild(this GameObject selfGameObj)
        {
            return selfGameObj.DestroyChildren();
        }

        [Obsolete("弃用，请使用 Position(), use Position() instead", APIVersion.Force)]
        public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.position;
        }

        [Obsolete("弃用，请使用 LocalScale(), use LocalScale() instead", APIVersion.Force)]
        public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localScale;
        }

        [Obsolete("弃用，请使用 LocalRotation(), use LocalRotation() instead", APIVersion.Force)]
        public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localRotation;
        }

        [Obsolete("弃用，请使用 LocalPosition(), use LocalPosition() instead", APIVersion.Force)]
        public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localPosition;
        }

        [Obsolete("弃用，请使用 Self, use Self instead", APIVersion.Force)]
        public static T ApplySelfTo<T>(this T selfObj, Action<T> toFunction) where T : Object
        {
            toFunction.InvokeGracefully(selfObj);
            return selfObj;
        }


        [Obsolete(
            "请使用 GetAttribute<T>(),please use GetAttribute<T>() instead",
            APIVersion.Force)]
        public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T : Attribute
        {
            return method.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }

        [Obsolete(
            "请使用 GetAttribute<T>(),please use GetAttribute<T>() instead",
            APIVersion.Force)]
        public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
        {
            return field.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }

        [Obsolete(
            "请使用 GetAttribute<T>(),please use GetAttribute<T>() instead",
            APIVersion.Force)]
        public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
        {
            return prop.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }

        [Obsolete(
            "请使用 GetAttribute<T>(),please use GetAttribute<T>() instead",
            APIVersion.Force)]
        public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }

        /// <summary>
        /// 通过反射方式获取域值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">域名</param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static object GetFieldByReflect(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            return fieldInfo == null ? null : fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 通过反射方式获取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, index);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static object InvokeByReflect(this object obj, string methodName, params object[] args)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            return methodInfo == null ? null : methodInfo.Invoke(obj, args);
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static object DefaultForType(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string LastWord(this string selfUrl)
        {
            return selfUrl.Split('/').Last();
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toAppend"></param>
        /// <param name="args"></param>
        [Obsolete(
            "请使用 str.Builder().AppendFormat(template,args).ToString(),please use str.Builder().AppendFormat(template,args).ToString() instead",
            true)]
        public static StringBuilder AppendFormat(this string selfStr, string toAppend, params object[] args)
        {
            return new StringBuilder(selfStr).AppendFormat(toAppend, args);
        }

        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toPrefix"></param>
        /// <returns></returns>
        [Obsolete(
            "请使用 str.Builder().AddPrefix(***).ToString(),please use str.Builder().AddPrefix(***).ToString() instead",
            true)]
        public static string AddPrefix(this string selfStr, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(selfStr).ToString();
        }


        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        [Obsolete(
            "请使用 str.Builder().Append(***).ToString(),please use str.Builder().Append(***).ToString() instead",
            true)]
        public static StringBuilder Append(this string selfStr, string toAppend)
        {
            return new StringBuilder(selfStr).Append(toAppend);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string UppercaseFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string LowercaseFirst(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string ToUnixLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        /// <summary>
        /// 转换成 CSV
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string ToCSV(this string[] values)
        {
            return string.Join(", ", values
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => value.Trim())
                .ToArray()
            );
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string[] ArrayFromCSV(this string values)
        {
            return values
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string ToSpacedCamelCase(this string text)
        {
            var sb = new StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    sb.Append(' ');
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool ImplementsInterface<T>(this Type type)
        {
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool ImplementsInterface<T>(this object obj)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }


        /// <summary>
        /// 使目录存在,Path可以是目录名必须是文件名
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }


        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetFolderName(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }

        // "使路径标准化，去除空格并将所有'\'转换为'/'"
        // "Normalize paths by removing whitespace and converting all '\' to '/'"
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }


        /// <summary>
        /// 获取不带后缀的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetFilePathWithoutExtension(string fileName)
        {
            if (fileName.Contains("."))
                return fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        /// <summary>
        /// 获取不带后缀的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetFileNameWithoutExtension(string fileName, char separator = '/')
        {
            return GetFilePathWithoutExtension(GetFileName(fileName, separator));
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetFileName(string path, char separator = '/')
        {
            path = MakePathStandard(path);
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        /// <summary>
        /// 获取文件夹名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetDirectoryName(string fileName)
        {
            fileName = MakePathStandard(fileName);
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetDirPath(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetLastDirName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true,
            string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static List<string> GetDirSubDirNameList(this string dirABSPath)
        {
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();

            return dirs.Select(d => d.Name).ToList();
        }

        /// <summary>
        /// 使目录存在
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void OpenFolder(string path)
        {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
            Process.Start("explorer.exe", path);
#endif
        }

        /// <summary>
        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetPathParentFolder(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 检测路径是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string CreateDirIfNotExists4FilePath(this string path)
        {
            var direct = path.GetPathParentFolder();

            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            return path;
        }


        /// <summary>
        /// 获取泛型名字
        /// <code>
        /// var typeName = GenericExtention.GetTypeName<string>();
        /// typeName.LogInfo(); // string
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }


        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static void DoIfNotNull<T>(this T selfObj, Action<T> action) where T : class
        {
            if (selfObj != null)
            {
                action(selfObj);
            }
        }


        /// <summary>
        /// 是否相等
        /// 
        /// 示例：
        /// <code>
        /// if (this.Is(player))
        /// {
        ///     ...
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead", true)]
        public static bool Is<T>(this T selfObj, T value)
        {
            return Equals(selfObj, value);
        }

        [Obsolete("请使用 Object.Equals(A,B)，please use Object.Equals() isntead", true)]
        public static bool Is<T>(this T selfObj, Func<T, bool> condition)
        {
            return condition(selfObj);
        }

        /// <summary>
        /// 表达式成立 则执行 Action
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do(()=>Debug.Log("1 == 1");
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool Do(this bool selfCondition, Action action)
        {
            if (selfCondition)
            {
                action();
            }

            return selfCondition;
        }

        /// <summary>
        /// 不管表达成不成立 都执行 Action，并把结果返回
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do((result)=>Debug.Log("1 == 1:" + result);
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Obsolete("不要使用，Do not used", APIVersion.Force)]
        public static bool Do(this bool selfCondition, Action<bool> action)
        {
            action(selfCondition);

            return selfCondition;
        }


        /// <summary>
        /// 功能：不为空则调用 Func
        /// 
        /// 示例:
        /// <code>
        /// Func<int> func = ()=> 1;
        /// var number = func.InvokeGracefully(); // 等价于 if (func != null) number = func();
        /// </code>
        /// </summary>
        /// <param name="selfFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", APIVersion.Force)]
        public static T InvokeGracefully<T>(this Func<T> selfFunc)
        {
            return null != selfFunc ? selfFunc() : default(T);
        }


        /// <summary>
        /// 功能：不为空则调用 Action
        /// 
        /// 示例:
        /// <code>
        /// System.Action action = () => Log.I("action called");
        /// action.InvokeGracefully(); // if (action != null) action();
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象 </param>
        /// <returns> 是否调用成功 </returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully(this Action selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T>
        /// 
        /// 示例:
        /// <code>
        /// System.Action<int> action = (number) => Log.I("action called" + number);
        /// action.InvokeGracefully(10); // if (action != null) action(10);
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象</param>
        /// <typeparam name="T">参数</typeparam>
        /// <returns> 是否调用成功</returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully<T>(this Action<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T,K>
        ///
        /// 示例
        /// <code>
        /// System.Action<int,string> action = (number,name) => Log.I("action called" + number + name);
        /// action.InvokeGracefully(10,"qframework"); // if (action != null) action(10,"qframework");
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully<T, K>(this Action<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用委托
        ///
        /// 示例：
        /// <code>
        /// // delegate
        /// TestDelegate testDelegate = () => { };
        /// testDelegate.InvokeGracefully();
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call suceed </returns>
        [Obsolete("请使用 someFunc?.Invoke() 方式调用, please use someFunc?.Invoke() instead", APIVersion.Force)]
        public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
        {
            if (null != selfAction)
            {
                selfAction.DynamicInvoke(args);
                return true;
            }

            return false;
        }
    }
}