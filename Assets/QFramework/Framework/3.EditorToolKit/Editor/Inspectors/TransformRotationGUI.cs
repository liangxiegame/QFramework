using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace QFramework {
	public class TransformRotationGUI
	{
		private object transformRotationGUI;
		private MethodInfo onEnable;
		private MethodInfo rotationField;

		public TransformRotationGUI()
		{
			if (transformRotationGUI == null)
			{
				var type = Type.GetType("UnityEditor.TransformRotationGUI,UnityEditor");

				onEnable = type.GetMethod("OnEnable");
				rotationField = type.GetMethod("RotationField", new Type[] { });

				transformRotationGUI = Activator.CreateInstance(type);
			}
		}

		/// <summary>
		/// Initializes the GUI.
		/// </summary>
		/// <param name="property">A serialized quaternion property.</param>
		/// <param name="content">The content to draw the property with.</param>
		public void Initialize(SerializedProperty property, GUIContent content)
		{
			onEnable.Invoke(transformRotationGUI, new object[] { property, content });
		}

		/// <summary>
		/// Draws the rotation GUI.
		/// </summary>
		public void Draw()
		{
			rotationField.Invoke(transformRotationGUI, null);
		}
	}
}