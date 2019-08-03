using System;
using System.Collections.Generic;
using System.Text;
using Unidux.Util;
using UnityEditor;
using UnityEngine;

namespace Unidux.Experimental.Editor
{
    public partial class UniduxPanelStateTab
    {
        protected Func6<IList<string>, string, object, Type, Action<object>, bool> SelectValueRender(Type type, object element)
        {
            if (type == typeof(int?))
            {
                return this.RenderNullableInt;
            }
            else if (element is int)
            {
                return this.RenderInt;
            }
            else if (type == typeof(uint?))
            {
                return this.RenderNullabelUInt;
            }
            else if (element is uint)
            {
                return this.RenderUInt;
            }
            else if (type == typeof(float?))
            {
                return this.RenderNullableFloat;
            }
            else if (element is float)
            {
                return this.RenderFloat;
            }
            else if (type == typeof(double?))
            {
                return this.RenderNullableDouble;
            }
            else if (element is double)
            {
                return this.RenderDouble;
            }
            else if (type == typeof(long?))
            {
                return this.RenderNullableLong;
            }
            else if (element is long)
            {
                return this.RenderLong;
            }
            else if (type == typeof(ulong?))
            {
                return this.RenderNullableULong;
            }
            else if (element is ulong)
            {
                return this.RenderULong;
            }
            else if (type == typeof(bool?))
            {
                return this.RenderNullableBool;
            }
            else if (element is bool)
            {
                return this.RenderBool;
            }
            else if (type.IsEnum)
            {
                return this.RenderEnum;
            }
            else if (element is Color)
            {
                return this.RenderColor;
            }
            else if (element is Vector2)
            {
                return this.RenderVector2;
            }
            else if (element is Vector3)
            {
                return this.RenderVector3;
            }
            else if (element is Vector4)
            {
                return this.RenderVector4;
            }

            return this.RenderUnknown;
        }

        protected bool RenderUnknown(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            EditorGUILayout.HelpBox(new StringBuilder(name).Append(" is empty").ToString(), MessageType.Info);
            return false;
        }

        protected bool RenderNullableInt(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (int?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseInt();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);

            return dirty;
        }

        protected bool RenderInt(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (int) element;
            var newValue = EditorGUILayout.DelayedIntField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderNullabelUInt(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (uint?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseUInt();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderUInt(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (int) ((uint) element);
            var newValue = EditorGUILayout.DelayedIntField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (newValue < 0) Debug.LogWarning("Illeagal uint value setted: " + newValue);
            else if (dirty) setter((uint) newValue);

            return dirty;
        }

        protected bool RenderNullableFloat(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (float?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseFloat();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderFloat(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (float) element;
            var newValue = EditorGUILayout.DelayedFloatField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderNullableDouble(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (double?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseDouble();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderDouble(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (double) element;
            var newValue = EditorGUILayout.DelayedDoubleField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderNullableLong(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (long?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseLong();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderLong(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (long) element;
            var newValue = EditorGUILayout.LongField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderNullableULong(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (ulong?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            var newValue = newValueString.ParseULong();
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderULong(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (long) ((ulong) element);
            var newValue = EditorGUILayout.LongField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (newValue < 0) Debug.LogWarning("Illeagal ulong value setted: " + newValue);
            else if (dirty) setter((ulong) newValue);
            return dirty;
        }

        protected bool RenderNullableBool(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (bool?) element;
            var oldValueString = oldValue.HasValue ? oldValue.Value.ToString() : "null";
            var newValueString = EditorGUILayout.DelayedTextField(name, oldValueString);
            bool? newValue = newValueString.ParseBool();

            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderBool(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (bool) element;
            var newValue = EditorGUILayout.Toggle(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderEnum(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            string[] _choices = Enum.GetNames(type);
            var oldValue = (int) element;
            var newValue = EditorGUILayout.Popup(name, oldValue, _choices);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderColor(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (Color) element;
            var newValue = EditorGUILayout.ColorField(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderVector2(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (Vector2) element;
            var newValue = EditorGUILayout.Vector2Field(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderVector3(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (Vector3) element;
            var newValue = EditorGUILayout.Vector3Field(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }

        protected bool RenderVector4(IList<string> _, string name, object element, Type type, Action<object> setter)
        {
            var oldValue = (Vector4) element;
            var newValue = EditorGUILayout.Vector4Field(name, oldValue);
            var dirty = (oldValue != newValue);

            if (dirty) setter(newValue);
            return dirty;
        }
    }
}