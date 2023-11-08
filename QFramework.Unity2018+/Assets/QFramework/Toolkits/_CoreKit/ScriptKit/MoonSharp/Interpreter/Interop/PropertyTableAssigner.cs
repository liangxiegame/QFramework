using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Utility class which may be used to set properties on an object of type T, from values contained in a Lua table.
	/// Properties must be decorated with the <see cref="MoonSharpPropertyAttribute"/>.
	/// This is a generic version of <see cref="PropertyTableAssigner"/>.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	public class PropertyTableAssigner<T> : IPropertyTableAssigner
	{
		PropertyTableAssigner m_InternalAssigner;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyTableAssigner{T}"/> class.
		/// </summary>
		/// <param name="expectedMissingProperties">The expected missing properties, that is expected fields in the table with no corresponding property in the object.</param>
		public PropertyTableAssigner(params string[] expectedMissingProperties)
		{
			m_InternalAssigner = new PropertyTableAssigner(typeof(T), expectedMissingProperties);
		}

		/// <summary>
		/// Adds an expected missing property, that is an expected field in the table with no corresponding property in the object.
		/// </summary>
		/// <param name="name">The name.</param>
		public void AddExpectedMissingProperty(string name)
		{
			m_InternalAssigner.AddExpectedMissingProperty(name);
		}

		/// <summary>
		/// Assigns properties from tables to an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="data">The table.</param>
		/// <exception cref="System.ArgumentNullException">Object is null</exception>
		/// <exception cref="ScriptRuntimeException">A field does not correspond to any property and that property is not one of the expected missing ones.</exception>
		public void AssignObject(T obj, Table data)
		{
			m_InternalAssigner.AssignObject(obj, data);
		}

		/// <summary>
		/// Gets the type-unsafe assigner corresponding to this object.
		/// </summary>
		/// <returns></returns>
		public PropertyTableAssigner GetTypeUnsafeAssigner()
		{
			return m_InternalAssigner;
		}

		/// <summary>
		/// Sets the subassigner for the given type. Pass null to remove usage of subassigner for the given type.
		/// </summary>
		/// <param name="propertyType">Type of the property for which the subassigner will be used.</param>
		/// <param name="assigner">The property assigner.</param>
		public void SetSubassignerForType(Type propertyType, IPropertyTableAssigner assigner)
		{
			m_InternalAssigner.SetSubassignerForType(propertyType, assigner);
		}

		/// <summary>
		/// Sets the subassigner for the given type
		/// </summary>
		/// <typeparam name="SubassignerType">Type of the property for which the subassigner will be used.</typeparam>
		/// <param name="assigner">The property assigner.</param>
		public void SetSubassigner<SubassignerType>(PropertyTableAssigner<SubassignerType> assigner)
		{
			m_InternalAssigner.SetSubassignerForType(typeof(SubassignerType), assigner);
		}


		/// <summary>
		/// Assigns the properties of the specified object without checking the type.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="data">The data.</param>
		void IPropertyTableAssigner.AssignObjectUnchecked(object o, Table data)
		{
			AssignObject((T)o, data);
		}
	}


	/// <summary>
	/// Utility class which may be used to set properties on an object from values contained in a Lua table.
	/// Properties must be decorated with the <see cref="MoonSharpPropertyAttribute"/>.
	/// See <see cref="PropertyTableAssigner{T}"/> for a generic compile time type-safe version.
	/// </summary>
	public class PropertyTableAssigner : IPropertyTableAssigner
	{
		Type m_Type;
		Dictionary<string, PropertyInfo> m_PropertyMap = new Dictionary<string, PropertyInfo>();
		Dictionary<Type, IPropertyTableAssigner> m_SubAssigners = new Dictionary<Type, IPropertyTableAssigner>();

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyTableAssigner"/> class.
		/// </summary>
		/// <param name="type">The type of the object.</param>
		/// <param name="expectedMissingProperties">The expected missing properties, that is expected fields in the table with no corresponding property in the object.</param>
		/// <exception cref="System.ArgumentException">
		/// Type cannot be a value type.
		/// </exception>
		public PropertyTableAssigner(Type type, params string[] expectedMissingProperties)
		{
			m_Type = type;

			if (Framework.Do.IsValueType(m_Type))
				throw new ArgumentException("Type cannot be a value type.");

			foreach(string property in expectedMissingProperties)
			{
				m_PropertyMap.Add(property, null);
			}

			foreach (PropertyInfo pi in Framework.Do.GetProperties(m_Type))
			{
				foreach (MoonSharpPropertyAttribute attr in pi.GetCustomAttributes(true).OfType<MoonSharpPropertyAttribute>())
				{
					string name = attr.Name ?? pi.Name;

					if (m_PropertyMap.ContainsKey(name))
					{
						throw new ArgumentException(string.Format("Type {0} has two definitions for MoonSharp property {1}", m_Type.FullName, name));
					}
					else
					{
						m_PropertyMap.Add(name, pi);
					}
				}
			}

		}

		/// <summary>
		/// Adds an expected missing property, that is an expected field in the table with no corresponding property in the object.
		/// </summary>
		/// <param name="name">The name.</param>
		public void AddExpectedMissingProperty(string name)
		{
			m_PropertyMap.Add(name, null);
		}


		private bool TryAssignProperty(object obj, string name, DynValue value)
		{
			if (m_PropertyMap.ContainsKey(name))
			{
				PropertyInfo pi = m_PropertyMap[name];

				if (pi != null)
				{
					object o;

					if (value.Type == DataType.Table && m_SubAssigners.ContainsKey(pi.PropertyType))
					{
						var subassigner = m_SubAssigners[pi.PropertyType];
						o = Activator.CreateInstance(pi.PropertyType);
						subassigner.AssignObjectUnchecked(o, value.Table);
					}
					else
					{
						o = Interop.Converters.ScriptToClrConversions.DynValueToObjectOfType(value,
							pi.PropertyType, null, false);
					}

					Framework.Do.GetSetMethod(pi).Invoke(obj, new object[] { o });
				}

				return true;
			}

			return false;
		}

		private void AssignProperty(object obj, string name, DynValue value)
		{
			if (TryAssignProperty(obj, name, value)) return;
			if (TryAssignProperty(obj, DescriptorHelpers.UpperFirstLetter(name), value)) return;
			if (TryAssignProperty(obj, DescriptorHelpers.Camelify(name), value)) return;
			if (TryAssignProperty(obj, DescriptorHelpers.UpperFirstLetter(DescriptorHelpers.Camelify(name)), value)) return;

			throw new ScriptRuntimeException("Invalid property {0}", name);
		}

		/// <summary>
		/// Assigns properties from tables to an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="data">The table.</param>
		/// <exception cref="System.ArgumentNullException">Object is null</exception>
		/// <exception cref="System.ArgumentException">The object is of an incompatible type.</exception>
		/// <exception cref="ScriptRuntimeException">A field does not correspond to any property and that property is not one of the expected missing ones.</exception>
		public void AssignObject(object obj, Table data)
		{
			if (obj == null)
				throw new ArgumentNullException("Object is null");

			if (!Framework.Do.IsInstanceOfType(m_Type, obj))
				throw new ArgumentException(string.Format("Invalid type of object : got '{0}', expected {1}", obj.GetType().FullName, m_Type.FullName));

			foreach (var pair in data.Pairs)
			{
				if (pair.Key.Type != DataType.String)
				{
					throw new ScriptRuntimeException("Invalid property of type {0}", pair.Key.Type.ToErrorTypeString());
				}

				AssignProperty(obj, pair.Key.String, pair.Value);
			}
		}

		/// <summary>
		/// Sets the subassigner for the given type. Pass null to remove usage of subassigner for the given type.
		/// </summary>
		/// <param name="propertyType">Type of the property for which the subassigner will be used.</param>
		/// <param name="assigner">The property assigner.</param>
		public void SetSubassignerForType(Type propertyType, IPropertyTableAssigner assigner)
		{
			if (   Framework.Do.IsAbstract(propertyType) 
				|| Framework.Do.IsGenericType(propertyType) 
				|| Framework.Do.IsInterface(propertyType) 
				|| Framework.Do.IsValueType(propertyType))
			{
				throw new ArgumentException("propertyType must be a concrete, reference type");
			}

			m_SubAssigners[propertyType] = assigner;
		}

		/// <summary>
		/// Assigns the properties of the specified object without checking the type.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="data">The data.</param>
		void IPropertyTableAssigner.AssignObjectUnchecked(object obj, Table data)
		{
			this.AssignObject(obj, data);
		}
	}

	/// <summary>
	/// Common interface for property assigners - basically used for sub-assigners
	/// </summary>
	public interface IPropertyTableAssigner
	{
		/// <summary>
		/// Assigns the properties of the specified object without checking the type.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="data">The data.</param>
		void AssignObjectUnchecked(object o, Table data);
	}
}
