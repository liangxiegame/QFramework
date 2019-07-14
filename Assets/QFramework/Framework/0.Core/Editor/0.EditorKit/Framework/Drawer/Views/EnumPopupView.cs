using System;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class EnumPopupView<T> : View //where T : struct
    {
        public Property<T> ValueProperty { get; }

        public EnumPopupView(T initValue)
        {
            ValueProperty = new Property<T>(initValue);
            ValueProperty.Value = initValue;

            Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            if(typeof(T).IsEnum){
                Enum enumType = ValueProperty.Value as Enum;
                enumType = EditorGUILayout.EnumPopup(enumType, Style, LayoutStyles);
            }
        }
    }
}