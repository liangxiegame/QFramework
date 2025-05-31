using UnityEngine;

namespace QFramework
{
    public class DisplayLabelAttribute : PropertyAttribute
    {
        public readonly string DisplayName;

        public DisplayLabelAttribute(string displayName) => DisplayName = displayName;
    }
}

#if UNITY_EDITOR
namespace QFramework
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(DisplayLabelAttribute))]
    public class DisplayLabelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
            if (customDrawer != null) return customDrawer.GetPropertyHeight(property, label);

            return EditorGUI.GetPropertyHeight(property, label);
        }

#if UNITY_EDITOR
        public static void LogWarning(Object owner, string message, Object target = null)
            => Debug.LogWarning($"<color=brown>{owner.name}</color> caused: " + message, target);

        public static void LogWarning(UnityEditor.SerializedProperty property, string message, Object target = null)
            => Debug.LogWarning($"Property <color=brown>{property.name}</color> " +
                                $"in Object <color=brown>{property.serializedObject.targetObject.name}</color> caused: " +
                                message, target);

        public static void LogCollectionsNotSupportedWarning(UnityEditor.SerializedProperty property, string nameOfType)
            => LogWarning(property, $"Array fields are not supported by <color=brown>[{nameOfType}]</color>. " +
                                    "Consider to use <color=blue>CollectionWrapper</color>",
                property.serializedObject.targetObject);
#endif

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
                LogCollectionsNotSupportedWarning(property, nameof(DisplayLabelAttribute));

            label.text = ((DisplayLabelAttribute)attribute).DisplayName;

            var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
            if (customDrawer != null) customDrawer.OnGUI(position, property, label);
            else EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif