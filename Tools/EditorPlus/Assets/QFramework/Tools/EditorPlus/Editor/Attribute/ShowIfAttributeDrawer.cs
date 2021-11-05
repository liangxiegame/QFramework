// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 11:26
//  *
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
    internal class ShowIfAttributeDrawer : PropertyDrawer
    {
        private bool isShow = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute attribute = (ShowIfAttribute) this.attribute;
            isShow = CheckShowTargets(property, attribute);
            float height = isShow ? EditorGUI.GetPropertyHeight(property, label, property.hasVisibleChildren) : -2;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (isShow) { EditorGUI.PropertyField(position, property, label, property.hasVisibleChildren); }
        }

        private static object TargetObjectGetField(SerializedProperty property, string name)
        {
            Object targetObject = property.serializedObject.targetObject;
            FieldInfo fieldInfo = targetObject.GetType().GetField(name);
            object obj = fieldInfo.GetValue(targetObject);
            return obj;
        }

        private const string strArrayKeyword = ".Array.data[";

        //如果特性作用于数组
        private static object GetTargetObjectForArray(SerializedProperty property, string targetName)
        {
            string strPath = property.propertyPath;

            //数组名格式：xxxx.Array.data[0].yyyy
            //判断数组名
            if (strPath.Contains(strArrayKeyword))
            {
                int start = strPath.IndexOf(strArrayKeyword, StringComparison.Ordinal) + 12;
                int end = strPath.IndexOf("].", StringComparison.Ordinal);
                if (end < 0) { return null; }

                int length = end - start;

                //得到数组索引
                if (!int.TryParse(strPath.Substring(start, length), out int index)) { return null; }

                //得到数组变量名
                string strArrayFieldName = strPath.Substring(0, start - 12);

                //得到数组变量值
                object obj = TargetObjectGetField(property, strArrayFieldName);

                //取元素值
                IList list = (IList) obj;
                if (list != null) { return list[index]; }

                Array array = (Array) obj;
                if (array != null) { return array.GetValue(index); }
            }

            return null;
        }

        //如果特性作用于类或结构体
        private static object GetTargetObjectChild(SerializedProperty property, string targetName)
        {
            int startIndex = property.propertyPath.IndexOf('.');
            if (startIndex >= 0)
            {
                string strPropertyName = property.propertyPath.Remove(startIndex);
                object obj = TargetObjectGetField(property, strPropertyName);
                return obj;
            }

            return null;
        }

        public static bool CheckFieldOrProperty(object targetObject, ShowIfAttribute.Target target)
        {
            Type targetObjectType = targetObject.GetType();
            FieldInfo fieldInfo = targetObjectType.GetField(target.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null) { return CheckIsUnityObject(fieldInfo.GetValue(targetObject)) == target.show; }

            PropertyInfo propertyInfo = targetObjectType.GetProperty(target.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                return CheckIsUnityObject(propertyInfo.GetValue(targetObject, null)) == target.show;
            }

            return true;
        }

        private static bool CheckIsUnityObject(object value)
        {
            if (value == null) { return false; }

            if (value is Object obj) { return obj; }

            if (value is bool b) { return b; }

            if (value is int n) { return n > 0; }

            if (value is float f) { return f > 0; }

            return false;
        }

        internal static bool CheckShowTarget(SerializedProperty property, ShowIfAttribute.Target target)
        {
            if (target == null) { return true; }

            object obj = GetTargetObjectForArray(property, target.name);

            if (obj == null) { obj = GetTargetObjectChild(property, target.name); }

            if (obj == null) { obj = property.serializedObject.targetObject; }

            return CheckFieldOrProperty(obj, target);
        }

        internal static bool CheckShowTargets(SerializedProperty property, ShowIfAttribute orAttribute)
        {
            bool res = true;
            if (orAttribute.mode == ShowIfAttribute.Mode.And)
            {
                for (int i = 0; i < orAttribute.targets.Length; i++)
                {
                    res &= CheckShowTarget(property, orAttribute.targets[i]);
                    if (!res) { return false; }
                }

                return true;
            }

            res = false;
            for (int i = 0; i < orAttribute.targets.Length; i++)
            {
                res |= CheckShowTarget(property, orAttribute.targets[i]);
                if (res) { return true; }
            }

            return false;
        }
    }
}