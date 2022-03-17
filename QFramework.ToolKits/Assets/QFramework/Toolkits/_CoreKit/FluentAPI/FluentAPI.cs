/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    
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

        public static T CancelAllTransitions<T>(this T selfSelectable) where T : Selectable
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

            LogKit.E("权值范围计算错误！");
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
    

}