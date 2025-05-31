#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

// TODO:
namespace QFramework
{
	public static class CustomDrawerUtility
	{
		/// <summary>
		/// Key is Associated with drawer type (the T in [CustomPropertyDrawer(typeof(T))])
		/// Value is PropertyDrawer Type
		/// </summary>
		private static readonly Dictionary<Type, Type> PropertyDrawersInAssembly = new Dictionary<Type, Type>();
		private static readonly Dictionary<int, PropertyDrawer> PropertyDrawersCache = new Dictionary<int, PropertyDrawer>();
		private static readonly string IgnoreScope = typeof(int).Module.ScopeName;

		/// <summary>
		/// Create PropertyDrawer for specified property if any PropertyDrawerType for such property is found.
		/// FieldInfo and Attribute will be inserted in created drawer.
		/// </summary>
		public static PropertyDrawer GetPropertyDrawerForProperty(SerializedProperty property, FieldInfo fieldInfo, Attribute attribute)
		{
			var propertyId = property.GetUniquePropertyId();
			if (PropertyDrawersCache.TryGetValue(propertyId, out var drawer)) return drawer;

			var targetType = fieldInfo.FieldType;
			var drawerType = GetPropertyDrawerTypeForFieldType(targetType);
			if (drawerType != null)
			{
				drawer = InstantiatePropertyDrawer(drawerType, fieldInfo, attribute);
				
				if (drawer == null) 
					LogKit.W(property, 
						$"Unable to instantiate CustomDrawer of type {drawerType} for {fieldInfo.FieldType}", 
						property.serializedObject.targetObject);
			}

			PropertyDrawersCache[propertyId] = drawer;
			return drawer;
		}
		
		public static PropertyDrawer InstantiatePropertyDrawer(Type drawerType, FieldInfo fieldInfo, Attribute insertAttribute)
		{
			try
			{
				var drawerInstance = (PropertyDrawer)Activator.CreateInstance(drawerType);
					
				// Reassign the attribute and fieldInfo fields in the drawer so it can access the argument values
				var fieldInfoField = drawerType.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
				if (fieldInfoField != null) fieldInfoField.SetValue(drawerInstance, fieldInfo);
				var attributeField = drawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
				if (attributeField != null) attributeField.SetValue(drawerInstance, insertAttribute);

				return drawerInstance;
			}
			catch (Exception)
			{
				return null;
			}
		}
		
		/// <summary>
		/// Try to get PropertyDrawer for a target Type, or any Base Type for a target Type
		/// </summary>
		public static Type GetPropertyDrawerTypeForFieldType(Type drawerTarget)
		{
			// Ignore .net types from mscorlib.dll
			if (drawerTarget.Module.ScopeName.Equals(IgnoreScope)) return null;
			CacheDrawersInAssembly();
			
			// Of all property drawers in the assembly we need to find one that affects target type
			// or one of the base types of target type
			var checkType = drawerTarget;
			while (checkType != null)
			{
				if (PropertyDrawersInAssembly.TryGetValue(drawerTarget, out var drawer)) return drawer;
				checkType = checkType.BaseType;
			}

			return null;
		}

		private static Type[] GetTypesSafe(Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types;
			}
		}
		
		private static void CacheDrawersInAssembly()
		{
			if (PropertyDrawersInAssembly.IsNotNullAndEmpty()) return;

			var propertyDrawerType = typeof(PropertyDrawer);
			var allDrawerTypesInDomain = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(GetTypesSafe)
				.Where(t => t != null && propertyDrawerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

			foreach (var drawerType in allDrawerTypesInDomain)
			{
				var propertyDrawerAttribute = CustomAttributeData.GetCustomAttributes(drawerType).FirstOrDefault();
				if (propertyDrawerAttribute == null) continue;
				var drawerTargetType = propertyDrawerAttribute.ConstructorArguments.FirstOrDefault().Value as Type;
				if (drawerTargetType == null) continue;

				if (PropertyDrawersInAssembly.ContainsKey(drawerTargetType)) continue;
				PropertyDrawersInAssembly.Add(drawerTargetType, drawerType);
			}
		}
	}
}
#endif