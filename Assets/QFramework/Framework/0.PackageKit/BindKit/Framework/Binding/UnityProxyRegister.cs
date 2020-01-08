/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Reflection;
using BindKit.Binding.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BindKit.Binding
{
    public class UnityProxyRegister
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Register<Transform, Vector3>("localPosition", t => t.localPosition, (t, v) => t.localPosition = v);
            Register<Transform, Vector3>("eulerAngles", t => t.eulerAngles, (t, v) => t.eulerAngles = v);
            Register<Transform, Vector3>("localEulerAngles", t => t.localEulerAngles, (t, v) => t.localEulerAngles = v);
            Register<Transform, Vector3>("right", t => t.right, (t, v) => t.right = v);
            Register<Transform, Vector3>("up", t => t.up, (t, v) => t.up = v);
            Register<Transform, Vector3>("forward", t => t.forward, (t, v) => t.forward = v);
            Register<Transform, Vector3>("position", t => t.position, (t, v) => t.position = v);
            Register<Transform, Vector3>("localScale", t => t.localScale, (t, v) => t.localScale = v);
            Register<Transform, Vector3>("lossyScale", t => t.lossyScale, null);
            Register<Transform, Quaternion>("rotation", t => t.rotation, (t, v) => t.rotation = v);
            Register<Transform, Quaternion>("localRotation", t => t.localRotation, (t, v) => t.localRotation = v);
            Register<Transform, Matrix4x4>("worldToLocalMatrix", t => t.worldToLocalMatrix, null);
            Register<Transform, Matrix4x4>("localToWorldMatrix", t => t.localToWorldMatrix, null);
            Register<Transform, int>("childCount", t => t.childCount, null);

            Register<RectTransform, Vector2>("offsetMax", t => t.offsetMax, (t, v) => t.offsetMax = v);
            Register<RectTransform, Vector2>("offsetMin", t => t.offsetMin, (t, v) => t.offsetMin = v);
            Register<RectTransform, Vector2>("pivot", t => t.pivot, (t, v) => t.pivot = v);
            Register<RectTransform, Vector2>("sizeDelta", t => t.sizeDelta, (t, v) => t.sizeDelta = v);
            Register<RectTransform, Vector2>("anchoredPosition", t => t.anchoredPosition, (t, v) => t.anchoredPosition = v);
            Register<RectTransform, Vector2>("anchorMax", t => t.anchorMax, (t, v) => t.anchorMax = v);
            Register<RectTransform, Vector3>("anchoredPosition3D", t => t.anchoredPosition3D, (t, v) => t.anchoredPosition3D = v);
            Register<RectTransform, Vector2>("anchorMin", t => t.anchorMin, (t, v) => t.anchorMin = v);
            Register<RectTransform, Rect>("rect", t => t.rect, null);

            Register<GameObject, bool>("activeSelf", t => t.activeSelf, (t, v) => t.SetActive(v));
            Register<GameObject, int>("layer", t => t.layer, (t, v) => t.layer = v);
            Register<GameObject, string>("tag", t => t.tag, (t, v) => t.tag = v);

            Register<Behaviour, bool>("enabled", t => t.enabled, (t, v) => t.enabled = v);
            Register<Behaviour, bool>("isActiveAndEnabled", t => t.isActiveAndEnabled, null);

            Register<Component, string>("tag", t => t.tag, (t, v) => t.tag = v);

            Register<Canvas, float>("planeDistance", t => t.planeDistance, (t, v) => t.planeDistance = v);
            Register<Canvas, string>("sortingLayerName", t => t.sortingLayerName, (t, v) => t.sortingLayerName = v);
            Register<Canvas, int>("sortingLayerID", t => t.sortingLayerID, (t, v) => t.sortingLayerID = v);
            Register<Canvas, int>("renderOrder", t => t.renderOrder, null);

            Register<CanvasGroup, float>("alpha", t => t.alpha, (t, v) => t.alpha = v);
            Register<CanvasGroup, bool>("interactable", t => t.interactable, (t, v) => t.interactable = v);
            Register<CanvasGroup, bool>("blocksRaycasts", t => t.blocksRaycasts, (t, v) => t.blocksRaycasts = v);
            Register<CanvasGroup, bool>("ignoreParentGroups", t => t.ignoreParentGroups, (t, v) => t.ignoreParentGroups = v);

            Register<GraphicRaycaster, bool>("ignoreReversedGraphics", t => t.ignoreReversedGraphics, (t, v) => t.ignoreReversedGraphics = v);

            Register<Mask, bool>("showMaskGraphic", t => t.showMaskGraphic, (t, v) => t.showMaskGraphic = v);

            Register<Selectable, SpriteState>("spriteState", t => t.spriteState, (t, v) => t.spriteState = v);
            Register<Selectable, ColorBlock>("colors", t => t.colors, (t, v) => t.colors = v);
            Register<Selectable, bool>("interactable", t => t.interactable, (t, v) => t.interactable = v);

            Register<Button, Button.ButtonClickedEvent>("onClick", t => t.onClick, null);

            Register<InputField, InputField.OnChangeEvent>("onValueChanged", t => t.onValueChanged, null);
            Register<InputField, InputField.SubmitEvent>("onEndEdit", t => t.onEndEdit, null);
            Register<InputField, string>("text", t => t.text, (t, v) => t.text = v);

            Register<Scrollbar, Scrollbar.ScrollEvent>("onValueChanged", t => t.onValueChanged, null);
            Register<Scrollbar, float>("size", t => t.size, (t, v) => t.size = v);
            Register<Scrollbar, float>("value", t => t.value, (t, v) => t.value = v);

            Register<Slider, Slider.SliderEvent>("onValueChanged", t => t.onValueChanged, null);
            Register<Slider, float>("value", t => t.value, (t, v) => t.value = v);
            Register<Slider, float>("maxValue", t => t.maxValue, (t, v) => t.maxValue = v);
            Register<Slider, float>("minValue", t => t.minValue, (t, v) => t.minValue = v);

            Register<Dropdown, int>("value", t => t.value, (t, v) => t.value = v);
            Register<Dropdown, Dropdown.DropdownEvent>("onValueChanged", t => t.onValueChanged, null);

            Register<Text, string>("text", t => t.text, (t, v) => t.text = v);
            Register<Text, int>("fontSize", t => t.fontSize, (t, v) => t.fontSize = v);

            Register<Toggle, bool>("isOn", t => t.isOn, (t, v) => t.isOn = v);
            Register<Toggle, Toggle.ToggleEvent>("onValueChanged", t => t.onValueChanged, (t, v) => t.onValueChanged = v);

            Register<ToggleGroup, bool>("allowSwitchOff", t => t.allowSwitchOff, (t, v) => t.allowSwitchOff = v);
        }

        static void Register<T, TValue>(string name, Func<T, TValue> getter, Action<T, TValue> setter)
        {
            var propertyInfo = typeof(T).GetProperty(name);
            if (propertyInfo is PropertyInfo)
            {
                ProxyFactory.Default.Register(new ProxyPropertyInfo<T, TValue>(name, getter, setter));
                return;
            }

            var fieldInfo = typeof(T).GetField(name);
            if (fieldInfo is FieldInfo)
            {
                ProxyFactory.Default.Register(new ProxyFieldInfo<T, TValue>(name, getter, setter));
                return;
            }

            throw new Exception(string.Format("Not found the property or field named '{0}' in {1} type", name, typeof(T).Name));
        }
    }
}
