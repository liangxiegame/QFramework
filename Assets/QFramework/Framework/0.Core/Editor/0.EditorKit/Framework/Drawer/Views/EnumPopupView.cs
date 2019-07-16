using System;
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class EnumPopupView : View //where T : struct
    {
        public Property<Enum> ValueProperty { get; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new Property<Enum>(initValue);
            ValueProperty.Value = initValue;

            Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
                Enum enumType = ValueProperty.Value;
                ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style, LayoutStyles);
        }
    }
}