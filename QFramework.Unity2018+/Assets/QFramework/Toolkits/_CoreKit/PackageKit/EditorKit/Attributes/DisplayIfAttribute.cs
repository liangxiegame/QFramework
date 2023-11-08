using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using QFramework;

namespace QFramework
{
    /// <summary>
    /// Conditionally Show/Hide field in inspector, based on some other field or property value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DisplayIfAttribute : PropertyAttribute
    {
        public bool IsSet => Data != null && Data.IsSet;
        public readonly DisplayIfData Data;

        /// <param name="fieldToCheck">String name of field to check value</param>
        /// <param name="inverse">Inverse check result</param>
        /// <param name="compareValues">On which values field will be shown in inspector</param>
        public DisplayIfAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
            => Data = new DisplayIfData(fieldToCheck, inverse, compareValues);


        public DisplayIfAttribute(string[] fieldToCheck, bool[] inverse = null, params object[] compare)
            => Data = new DisplayIfData(fieldToCheck, inverse, compare);

        public DisplayIfAttribute(params string[] fieldToCheck) => Data = new DisplayIfData(fieldToCheck);

        public DisplayIfAttribute(bool useMethod, string method, bool inverse = false)
            => Data = new DisplayIfData(useMethod, method, inverse);
    }

    public class DisplayIfData
    {
        public bool IsSet => _fieldToCheck.IsNotNullAndEmpty() || _fieldsToCheckMultiple.IsNotNullAndEmpty() ||
                             _predicateMethod.IsNotNullAndEmpty();

        private readonly string _fieldToCheck;
        private readonly bool _inverse;
        private readonly string[] _compareValues;

        private readonly string[] _fieldsToCheckMultiple;
        private readonly bool[] _inverseMultiple;
        private readonly string[] _compareValuesMultiple;

        private readonly string _predicateMethod;

        public DisplayIfData(string fieldToCheck, bool inverse = false, params object[] compareValues)
            => (_fieldToCheck, _inverse, _compareValues) =
                (fieldToCheck, inverse, compareValues.Select(c => c.ToString().ToUpper()).ToArray());

        public DisplayIfData(string[] fieldToCheck, bool[] inverse = null, params object[] compare) =>
            (_fieldsToCheckMultiple, _inverseMultiple, _compareValuesMultiple) =
            (fieldToCheck, inverse, compare.Select(c => c.ToString().ToUpper()).ToArray());

        public DisplayIfData(params string[] fieldToCheck) => _fieldsToCheckMultiple = fieldToCheck;

        // ReSharper disable once UnusedParameter.Local
        public DisplayIfData(bool useMethod, string methodName, bool inverse = false)
            => (_predicateMethod, _inverse) = (methodName, inverse);


#if UNITY_EDITOR
        /// <summary>
        /// Iterate over Field Conditions
        /// </summary>
        public IEnumerator<(string Field, bool Inverse, string[] CompareAgainst)> GetEnumerator()
        {
            if (_fieldToCheck.IsNotNullAndEmpty()) yield return (_fieldToCheck, _inverse, _compareValues);
            if (_fieldsToCheckMultiple.IsNotNullAndEmpty())
            {
                for (var i = 0; i < _fieldsToCheckMultiple.Length; i++)
                {
                    var field = _fieldsToCheckMultiple[i];
                    bool withInverseValue = _inverseMultiple != null && _inverseMultiple.Length - 1 >= i;
                    bool withCompareValue = _compareValuesMultiple != null && _compareValuesMultiple.Length - 1 >= i;
                    var inverse = withInverseValue && _inverseMultiple[i];
                    var compare = withCompareValue ? new[] { _compareValuesMultiple[i] } : null;

                    yield return (field, inverse, compare);
                }
            }
        }

        /// <summary>
        /// Call and check Method Condition, if any
        /// </summary>
        public bool IsMethodConditionMatch(object owner)
        {
            if (_predicateMethod.IsNullOrEmpty()) return true;

            var predicateMethod = GetMethodCondition(owner);
            if (predicateMethod == null) return true;

            bool match = (bool)predicateMethod.Invoke(owner, null);
            if (_inverse) match = !match;
            return match;
        }


        private MethodInfo GetMethodCondition(object owner)
        {
            if (_predicateMethod.IsNullOrEmpty()) return null;
            if (_initializedMethodInfo) return _cachedMethodInfo;
            _initializedMethodInfo = true;

            var ownerType = owner.GetType();
            var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var method = ownerType.GetMethods(bindings).SingleOrDefault(m => m.Name == _predicateMethod);

            if (method == null || method.ReturnType != typeof(bool))
            {
                Debug.Log(owner + _predicateMethod + "Not Found");
                _cachedMethodInfo = null;
            }
            else _cachedMethodInfo = method;

            return _cachedMethodInfo;
        }

        private MethodInfo _cachedMethodInfo;
        private bool _initializedMethodInfo;
#endif
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(DisplayIfAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
        private bool _toShow = true;
        private bool _initialized;
        private PropertyDrawer _customPropertyDrawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!(attribute is DisplayIfAttribute conditional)) return EditorGUI.GetPropertyHeight(property);

            CachePropertyDrawer(property);
            _toShow = IsPropertyConditionMatch(property, conditional.Data);
            if (!_toShow) return -2;

            if (_customPropertyDrawer != null) return _customPropertyDrawer.GetPropertyHeight(property, label);
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_toShow) return;

            if (!CustomDrawerUsed()) EditorGUI.PropertyField(position, property, label, true);


            bool CustomDrawerUsed()
            {
                if (_customPropertyDrawer == null) return false;

                try
                {
                    _customPropertyDrawer.OnGUI(position, property, label);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Unable to use CustomDrawer of type " + _customPropertyDrawer.GetType() + ": " + e);

                    return false;
                }
            }
        }

        /// <summary>
        /// Try to find and cache any PropertyDrawer or PropertyAttribute on the field
        /// </summary>
        private void CachePropertyDrawer(SerializedProperty property)
        {
            if (_initialized) return;
            _initialized = true;
            if (fieldInfo == null) return;

            var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
            if (customDrawer == null) customDrawer = TryCreateAttributeDrawer();

            _customPropertyDrawer = customDrawer;


            // Try to get drawer for any other Attribute on the field
            PropertyDrawer TryCreateAttributeDrawer()
            {
                var secondAttribute = TryGetSecondAttribute();
                if (secondAttribute == null) return null;

                var attributeType = secondAttribute.GetType();
                var customDrawerType = CustomDrawerUtility.GetPropertyDrawerTypeForFieldType(attributeType);
                if (customDrawerType == null) return null;

                return CustomDrawerUtility.InstantiatePropertyDrawer(customDrawerType, fieldInfo, secondAttribute);


                //Get second attribute if any
                Attribute TryGetSecondAttribute()
                {
                    return (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
                        .FirstOrDefault(a => !(a is DisplayIfAttribute));
                }
            }
        }

        public static bool IsConditionMatch(UnityEngine.Object owner, DisplayIfData condition)
        {
            if (!condition.IsSet) return true;

            var so = new SerializedObject(owner);
            foreach (var fieldCondition in condition)
            {
                if (fieldCondition.Field.IsNullOrEmpty()) continue;

                var property = so.FindProperty(fieldCondition.Field);
                if (property == null) LogFieldNotFound(so.targetObject, fieldCondition.Field);

                bool passed = IsConditionMatch(property, fieldCondition.Inverse, fieldCondition.CompareAgainst);
                if (!passed) return false;
            }

            return condition.IsMethodConditionMatch(owner);
        }

        public static bool IsPropertyConditionMatch(SerializedProperty property, DisplayIfData condition)
        {
            if (!condition.IsSet) return true;

            foreach (var fieldCondition in condition)
            {
                var relativeProperty = FindRelativeProperty(property, fieldCondition.Field);
                if (relativeProperty == null) LogFieldNotFound(property, fieldCondition.Field);

                bool passed = IsConditionMatch(relativeProperty, fieldCondition.Inverse, fieldCondition.CompareAgainst);
                if (!passed) return false;
            }

            return condition.IsMethodConditionMatch(GetParent(property));
        }

        private static void LogFieldNotFound(SerializedProperty property, string field) => Debug.LogWarning(
            $"Conditional Attribute is trying to check field {field} which is not present");

        private static void LogFieldNotFound(UnityEngine.Object owner, string field) => Debug.LogWarning($"Conditional Attribute is trying to check field {field} which is not present");

        public static void LogMethodNotFound(UnityEngine.Object owner, string method) => Debug.LogWarning(
            $"Conditional Attribute is trying to invoke method {method} " +
            "which is missing or not with a bool return type");

        private static bool IsConditionMatch(SerializedProperty property, bool inverse, string[] compareAgainst)
        {
            if (property == null) return true;

            string asString = AsStringValue(property).ToUpper();

            if (compareAgainst != null && compareAgainst.Length > 0)
            {
                var matchAny = CompareAgainstValues(asString, compareAgainst, IsFlagsEnum());
                if (inverse) matchAny = !matchAny;
                return matchAny;
            }

            bool someValueAssigned = asString != "FALSE" && asString != "0" && asString != "NULL";
            if (someValueAssigned) return !inverse;

            return inverse;


            bool IsFlagsEnum()
            {
                if (property.propertyType != SerializedPropertyType.Enum) return false;
                var value = GetValue(property);
                if (value == null) return false;
                return value.GetType().GetCustomAttribute<FlagsAttribute>() != null;
            }
        }


        /// <summary>
        /// True if the property value matches any of the values in '_compareValues'
        /// </summary>
        private static bool CompareAgainstValues(string propertyValueAsString, string[] compareAgainst,
            bool handleFlags)
        {
            if (!handleFlags) return ValueMatches(propertyValueAsString);

            if (propertyValueAsString == "-1") //Handle Everything
                return true;
            if (propertyValueAsString == "0") //Handle Nothing
                return false;

            var separateFlags = propertyValueAsString.Split(',');
            foreach (var flag in separateFlags)
            {
                if (ValueMatches(flag.Trim())) return true;
            }

            return false;


            bool ValueMatches(string value)
            {
                foreach (var compare in compareAgainst)
                    if (value == compare)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Get the other Property which is stored alongside with specified Property, by name
        /// </summary>
        private static SerializedProperty FindRelativeProperty(SerializedProperty property, string propertyName)
        {
            if (property.depth == 0) return property.serializedObject.FindProperty(propertyName);

            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            var nestedProperty = NestedPropertyOrigin(property, elements);

            // if nested property is null = we hit an array property
            if (nestedProperty == null)
            {
                var cleanPath = path.Substring(0, path.IndexOf('['));
                var arrayProp = property.serializedObject.FindProperty(cleanPath);
                Debug.LogWarning(arrayProp.name + "Not Supported Array");

                return null;
            }

            return nestedProperty.FindPropertyRelative(propertyName);
        }

        // For [Serialized] types with [Conditional] fields
        private static SerializedProperty NestedPropertyOrigin(SerializedProperty property, string[] elements)
        {
            SerializedProperty parent = null;

            for (int i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];
                int index = -1;
                if (element.Contains("["))
                {
                    index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                }

                parent = i == 0
                    ? property.serializedObject.FindProperty(element)
                    : parent != null
                        ? parent.FindPropertyRelative(element)
                        : null;

                if (index >= 0 && parent != null) parent = parent.GetArrayElementAtIndex(index);
            }

            return parent;
        }

        public static string AsStringValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;

                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (property.type == "char") return Convert.ToChar(property.intValue).ToString();
                    return property.intValue.ToString();

                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();

                case SerializedPropertyType.Enum:
                    return GetValue(property).ToString();

                default:
                    return string.Empty;
            }
        }

        #region SerializedProperty Get Parent

        // Found here http://answers.unity.com/answers/425602/view.html
        // Update here https://gist.github.com/AdrienVR/1548a145c039d2fddf030ebc22f915de to support inherited private members.
        /// <summary>
        /// Get parent object of SerializedProperty
        /// </summary>
        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    obj = GetValueAt(obj, elementName, index);
                }
                else
                {
                    obj = GetFieldValue(obj, element);
                }
            }

            return obj;


            object GetValueAt(object source, string name, int index)
            {
                var enumerable = GetFieldValue(source, name) as IEnumerable;
                if (enumerable == null) return null;

                var enm = enumerable.GetEnumerator();
                while (index-- >= 0)
                    enm.MoveNext();
                return enm.Current;
            }

            object GetFieldValue(object source, string name)
            {
                if (source == null)
                    return null;

                foreach (var type in GetHierarchyTypes(source.GetType()))
                {
                    var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (f != null)
                        return f.GetValue(source);

                    var p = type.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p != null)
                        return p.GetValue(source, null);
                }

                return null;


                IEnumerable<Type> GetHierarchyTypes(Type sourceType)
                {
                    yield return sourceType;
                    while (sourceType.BaseType != null)
                    {
                        yield return sourceType.BaseType;
                        sourceType = sourceType.BaseType;
                    }
                }
            }
        }

        #endregion

        public static string GetFixedPropertyPath(SerializedProperty property) =>
            property.propertyPath.Replace(".Array.data[", "[");

        /// <summary>
        /// Get raw object value out of the SerializedProperty
        /// </summary>
        public static object GetValue(SerializedProperty property)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            var elements = GetFixedPropertyPath(property).Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    obj = GetValueByArrayFieldName(obj, elementName, index);
                }
                else obj = GetValueByFieldName(obj, element);
            }

            return obj;


            object GetValueByArrayFieldName(object source, string name, int index)
            {
                if (!(GetValueByFieldName(source, name) is IEnumerable enumerable)) return null;
                var enumerator = enumerable.GetEnumerator();

                for (var i = 0; i <= index; i++)
                    if (!enumerator.MoveNext())
                        return null;
                return enumerator.Current;
            }

            // Search "source" object for a field with "name" and get it's value
            object GetValueByFieldName(object source, string name)
            {
                if (source == null) return null;
                var type = source.GetType();

                while (type != null)
                {
                    var fieldInfo = type.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (fieldInfo != null) return fieldInfo.GetValue(source);

                    var propertyInfo = type.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (propertyInfo != null) return propertyInfo.GetValue(source, null);

                    type = type.BaseType;
                }

                return null;
            }
        }
    }
}
#endif