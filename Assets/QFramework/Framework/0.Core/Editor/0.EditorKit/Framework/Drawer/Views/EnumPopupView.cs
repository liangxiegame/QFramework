using System;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class EnumPopupView<T> : View
    {
        public Property<Enum> ValueProperty { get; private set; }

        public EnumPopupView(T initValue)
        {
            ValueProperty = new Property<Enum>(initValue as Enum);
            ValueProperty.Value = initValue as Enum;

            Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            if(typeof(T).IsEnum){
                var enumType = ValueProperty.Value;
                ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style, LayoutStyles);
            }
        }
    }
}