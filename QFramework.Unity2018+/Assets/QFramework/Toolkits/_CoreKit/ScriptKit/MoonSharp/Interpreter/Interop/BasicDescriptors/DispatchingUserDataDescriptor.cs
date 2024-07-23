using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	/// <summary>
	/// An abstract user data descriptor which accepts members described by <see cref="IMemberDescriptor"/> objects and
	/// correctly dispatches to them.
	/// Metamethods are also by default dispatched to operator overloads and other similar methods - see
	/// <see cref="MetaIndex"/> .
	/// </summary>
	public abstract class DispatchingUserDataDescriptor : IUserDataDescriptor, IOptimizableDescriptor
	{
		private int m_ExtMethodsVersion = 0;
		private Dictionary<string, IMemberDescriptor> m_MetaMembers = new Dictionary<string, IMemberDescriptor>();
		private Dictionary<string, IMemberDescriptor> m_Members = new Dictionary<string, IMemberDescriptor>();

		/// <summary>
		/// The special name used by CLR for indexer getters
		/// </summary>
		protected const string SPECIALNAME_INDEXER_GET = "get_Item";
		/// <summary>
		/// The special name used by CLR for indexer setters
		/// </summary>
		protected const string SPECIALNAME_INDEXER_SET = "set_Item";

		/// <summary>
		/// The special name used by CLR for explicit cast conversions
		/// </summary>
		protected const string SPECIALNAME_CAST_EXPLICIT = "op_Explicit";
		/// <summary>
		/// The special name used by CLR for implicit cast conversions
		/// </summary>
		protected const string SPECIALNAME_CAST_IMPLICIT = "op_Implicit";


		/// <summary>
		/// Gets the name of the descriptor (usually, the name of the type described).
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the type this descriptor refers to
		/// </summary>
		public Type Type { get; private set; }
		/// <summary>
		/// Gets a human readable friendly name of the descriptor
		/// </summary>
		public string FriendlyName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardUserDataDescriptor" /> class.
		/// </summary>
		/// <param name="type">The type this descriptor refers to.</param>
		/// <param name="friendlyName">A friendly name for the type, or null.</param>
		protected DispatchingUserDataDescriptor(Type type, string friendlyName = null)
		{
			Type = type;
			Name = type.FullName;
			FriendlyName = friendlyName ?? type.Name;
		}

		/// <summary>
		/// Adds a member to the meta-members list.
		/// </summary>
		/// <param name="name">The name of the metamethod.</param>
		/// <param name="desc">The desc.</param>
		/// <exception cref="System.ArgumentException">
		/// Thrown if a name conflict is detected and one of the conflicting members does not support overloads.
		/// </exception>
		public void AddMetaMember(string name, IMemberDescriptor desc)
		{
			if (desc != null)
				AddMemberTo(m_MetaMembers, name, desc);
		}


		/// <summary>
		/// Adds a DynValue as a member
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void AddDynValue(string name, DynValue value)
		{
			var desc = new DynValueMemberDescriptor(name, value); 
			AddMemberTo(m_Members, name, desc);
		}

		/// <summary>
		/// Adds a property to the member list
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="desc">The descriptor.</param>
		/// <exception cref="System.ArgumentException">
		/// Thrown if a name conflict is detected and one of the conflicting members does not support overloads.
		/// </exception>
		public void AddMember(string name, IMemberDescriptor desc)
		{
			if (desc != null)
				AddMemberTo(m_Members, name, desc);
		}

		/// <summary>
		/// Gets the member names.
		/// </summary>
		public IEnumerable<string> MemberNames
		{
			get { return m_Members.Keys; }
		}

		/// <summary>
		/// Gets the members.
		/// </summary>
		public IEnumerable<KeyValuePair<string, IMemberDescriptor>> Members
		{
			get { return m_Members; }
		}

		/// <summary>
		/// Finds the member with a given name. If not found, null is returned.
		/// </summary>
		/// <param name="memberName">Name of the member.</param>
		/// <returns></returns>
		public IMemberDescriptor FindMember(string memberName)
		{
			return m_Members.GetOrDefault(memberName);
		}

		/// <summary>
		/// Removes the member with a given name. In case of overloaded functions, all overloads are removed.
		/// </summary>
		/// <param name="memberName">Name of the member.</param>
		public void RemoveMember(string memberName)
		{
			m_Members.Remove(memberName);
		}

		/// <summary>
		/// Gets the meta member names.
		/// </summary>
		public IEnumerable<string> MetaMemberNames
		{
			get { return m_MetaMembers.Keys; }
		}

		/// <summary>
		/// Gets the meta members.
		/// </summary>
		public IEnumerable<KeyValuePair<string, IMemberDescriptor>> MetaMembers
		{
			get { return m_MetaMembers; }
		}

		/// <summary>
		/// Finds the meta member with a given name. If not found, null is returned.
		/// </summary>
		/// <param name="memberName">Name of the member.</param>
		public IMemberDescriptor FindMetaMember(string memberName)
		{
			return m_MetaMembers.GetOrDefault(memberName);
		}

		/// <summary>
		/// Removes the meta member with a given name. In case of overloaded functions, all overloads are removed.
		/// </summary>
		/// <param name="memberName">Name of the member.</param>
		public void RemoveMetaMember(string memberName)
		{
			m_MetaMembers.Remove(memberName);
		}




		private void AddMemberTo(Dictionary<string, IMemberDescriptor> members, string name, IMemberDescriptor desc)
		{
			IOverloadableMemberDescriptor odesc = desc as IOverloadableMemberDescriptor;

			if (odesc != null)
			{
				if (members.ContainsKey(name))
				{
					OverloadedMethodMemberDescriptor overloads = members[name] as OverloadedMethodMemberDescriptor;

					if (overloads != null)
						overloads.AddOverload(odesc);
					else
						throw new ArgumentException(string.Format("Multiple members named {0} are being added to type {1} and one or more of these members do not support overloads.", name, this.Type.FullName));
				}
				else
				{
					members.Add(name, new OverloadedMethodMemberDescriptor(name, this.Type, odesc));
				}
			}
			else
			{
				if (members.ContainsKey(name))
				{
					throw new ArgumentException(string.Format("Multiple members named {0} are being added to type {1} and one or more of these members do not support overloads.", name, this.Type.FullName));
				}
				else
				{
					members.Add(name, desc);
				}
			}
		}

		/// <summary>
		/// Performs an "index" "get" operation. This tries to resolve minor variations of member names.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="index">The index.</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		public virtual DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing)
		{
			if (!isDirectIndexing)
			{
				IMemberDescriptor mdesc = m_Members
					.GetOrDefault(SPECIALNAME_INDEXER_GET)
					.WithAccessOrNull(MemberDescriptorAccess.CanExecute);

				if (mdesc != null)
					return ExecuteIndexer(mdesc, script, obj, index, null);
			}

			index = index.ToScalar();

			if (index.Type != DataType.String)
				return null;

			DynValue v = TryIndex(script, obj, index.String);
			if (v == null) v = TryIndex(script, obj, UpperFirstLetter(index.String));
			if (v == null) v = TryIndex(script, obj, Camelify(index.String));
			if (v == null) v = TryIndex(script, obj, UpperFirstLetter(Camelify(index.String)));

			if (v == null && m_ExtMethodsVersion < UserData.GetExtensionMethodsChangeVersion())
			{
				m_ExtMethodsVersion = UserData.GetExtensionMethodsChangeVersion();

				v = TryIndexOnExtMethod(script, obj, index.String);
				if (v == null) v = TryIndexOnExtMethod(script, obj, UpperFirstLetter(index.String));
				if (v == null) v = TryIndexOnExtMethod(script, obj, Camelify(index.String));
				if (v == null) v = TryIndexOnExtMethod(script, obj, UpperFirstLetter(Camelify(index.String)));
			}

			return v;
		}


		/// <summary>
		/// Tries to perform an indexing operation by checking newly added extension methods for the given indexName.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="indexName">Member name to be indexed.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		private DynValue TryIndexOnExtMethod(Script script, object obj, string indexName)
		{
			List<IOverloadableMemberDescriptor> methods = UserData.GetExtensionMethodsByNameAndType(indexName, this.Type);

			if (methods != null && methods.Count > 0)
			{
				var ext = new OverloadedMethodMemberDescriptor(indexName, this.Type);
				ext.SetExtensionMethodsSnapshot(UserData.GetExtensionMethodsChangeVersion(), methods);
				m_Members.Add(indexName, ext);
				return DynValue.NewCallback(ext.GetCallback(script, obj));
			}

			return null;
		}

		/// <summary>
		/// Determines whether the descriptor contains the specified member (by exact name)
		/// </summary>
		/// <param name="exactName">Name of the member.</param>
		/// <returns></returns>
		public bool HasMember(string exactName)
		{
			return m_Members.ContainsKey(exactName);
		}

		/// <summary>
		/// Determines whether the descriptor contains the specified member in the meta list (by exact name)
		/// </summary>
		/// <param name="exactName">Name of the meta-member.</param>
		/// <returns></returns>
		public bool HasMetaMember(string exactName)
		{
			return m_MetaMembers.ContainsKey(exactName);
		}


		/// <summary>
		/// Tries to perform an indexing operation by checking methods and properties for the given indexName
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="indexName">Member name to be indexed.</param>
		/// <returns></returns>
		protected virtual DynValue TryIndex(Script script, object obj, string indexName)
		{
			IMemberDescriptor desc;

			if (m_Members.TryGetValue(indexName, out desc))
			{
				return desc.GetValue(script, obj);
			}

			return null;
		}

		/// <summary>
		/// Performs an "index" "set" operation. This tries to resolve minor variations of member names.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="index">The index.</param>
		/// <param name="value">The value to be set</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		public virtual bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing)
		{
			if (!isDirectIndexing)
			{
				IMemberDescriptor mdesc = m_Members
					.GetOrDefault(SPECIALNAME_INDEXER_SET)
					.WithAccessOrNull(MemberDescriptorAccess.CanExecute);

				if (mdesc != null)
				{
					ExecuteIndexer(mdesc, script, obj, index, value);
					return true;
				}
			}

			index = index.ToScalar();

			if (index.Type != DataType.String)
				return false;

			bool v = TrySetIndex(script, obj, index.String, value);
			if (!v) v = TrySetIndex(script, obj, UpperFirstLetter(index.String), value);
			if (!v) v = TrySetIndex(script, obj, Camelify(index.String), value);
			if (!v) v = TrySetIndex(script, obj, UpperFirstLetter(Camelify(index.String)), value);

			return v;
		}

		/// <summary>
		/// Tries to perform an indexing "set" operation by checking methods and properties for the given indexName
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="indexName">Member name to be indexed.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		protected virtual bool TrySetIndex(Script script, object obj, string indexName, DynValue value)
		{
			IMemberDescriptor descr = m_Members.GetOrDefault(indexName);

			if (descr != null)
			{
				descr.SetValue(script, obj, value);
				return true;
			}
			else
			{
				return false;
			}
		}

		void IOptimizableDescriptor.Optimize()
		{
			foreach (var m in this.m_MetaMembers.Values.OfType<IOptimizableDescriptor>())
				m.Optimize();

			foreach (var m in this.m_Members.Values.OfType<IOptimizableDescriptor>())
				m.Optimize();
		}

		/// <summary>
		/// Converts the specified name from underscore_case to camelCase.
		/// Just a wrapper over the <see cref="DescriptorHelpers"/> method with the same name,
 		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		protected static string Camelify(string name)
		{
			return DescriptorHelpers.Camelify(name);
		}

		/// <summary>
		/// Converts the specified name to one with an uppercase first letter (something to Something).
		/// Just a wrapper over the <see cref="DescriptorHelpers"/> method with the same name,
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		protected static string UpperFirstLetter(string name)
		{
			return DescriptorHelpers.UpperFirstLetter(name);
		}

		/// <summary>
		/// Converts this userdata to string
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		public virtual string AsString(object obj)
		{
			return (obj != null) ? obj.ToString() : null;
		}



		/// <summary>
		/// Executes the specified indexer method.
		/// </summary>
		/// <param name="mdesc">The method descriptor</param>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="index">The indexer parameter</param>
		/// <param name="value">The dynvalue to set on a setter, or null.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected virtual DynValue ExecuteIndexer(IMemberDescriptor mdesc, Script script, object obj, DynValue index, DynValue value)
		{
			IList<DynValue> values;

			if (index.Type == DataType.Tuple)
			{
				if (value == null)
				{
					values = index.Tuple;
				}
				else
				{
					values = new List<DynValue>(index.Tuple);
					values.Add(value);
				}
			}
			else
			{
				if (value == null)
				{
					values = new DynValue[] { index };
				}
				else
				{
					values = new DynValue[] { index, value };
				}
			}

			CallbackArguments args = new CallbackArguments(values, false);
			ScriptExecutionContext execCtx = script.CreateDynamicExecutionContext();

			DynValue v = mdesc.GetValue(script, obj);

			if (v.Type != DataType.ClrFunction)
				throw new ScriptRuntimeException("a clr callback was expected in member {0}, while a {1} was found", mdesc.Name, v.Type);

			return v.Callback.ClrCallback(execCtx, args);
		}


		/// <summary>
		/// Gets a "meta" operation on this userdata. If a descriptor does not support this functionality,
		/// it should return "null" (not a nil). 
		/// See <see cref="IUserDataDescriptor.MetaIndex" /> for further details.
		/// 
		/// If a method exists marked with <see cref="MoonSharpUserDataMetamethodAttribute" /> for the specific
		/// metamethod requested, that method is returned.
		/// 
		/// If the above fails, the following dispatching occur:
		/// 
		/// __add, __sub, __mul, __div, __mod and __unm are dispatched to C# operator overloads (if they exist)
		/// __eq is dispatched to System.Object.Equals.
		/// __lt and __le are dispatched IComparable.Compare, if the type implements IComparable or IComparable{object}
		/// __len is dispatched to Length and Count properties, if those exist.
		/// __iterator is handled if the object implements IEnumerable or IEnumerator.
		/// __tonumber is dispatched to implicit or explicit conversion operators to standard numeric types.
		/// __tobool is dispatched to an implicit or explicit conversion operator to bool. If that fails, operator true is used.
		/// 
		/// <param name="script">The script originating the request</param>
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="metaname">The name of the metamember.</param>
		/// </summary>
		/// <returns></returns>
		public virtual DynValue MetaIndex(Script script, object obj, string metaname)
		{
			IMemberDescriptor desc = m_MetaMembers.GetOrDefault(metaname);

			if (desc != null)
			{
				return desc.GetValue(script, obj);
			}

			switch (metaname)
			{
				case "__add":
					return DispatchMetaOnMethod(script, obj, "op_Addition");
				case "__sub":
					return DispatchMetaOnMethod(script, obj, "op_Subtraction");
				case "__mul":
					return DispatchMetaOnMethod(script, obj, "op_Multiply");
				case "__div":
					return DispatchMetaOnMethod(script, obj, "op_Division");
				case "__mod":
					return DispatchMetaOnMethod(script, obj, "op_Modulus");
				case "__unm":
					return DispatchMetaOnMethod(script, obj, "op_UnaryNegation");
				case "__eq":
					return MultiDispatchEqual(script, obj);
				case "__lt":
					return MultiDispatchLessThan(script, obj);
				case "__le":
					return MultiDispatchLessThanOrEqual(script, obj);
				case "__len":
					return TryDispatchLength(script, obj);
				case "__tonumber":
					return TryDispatchToNumber(script, obj);
				case "__tobool":
					return TryDispatchToBool(script, obj);
				case "__iterator":
					return ClrToScriptConversions.EnumerationToDynValue(script, obj);
				default:
					return null;
			}
		}

		#region MetaMethodsDispatching


		private int PerformComparison(object obj, object p1, object p2)
		{
			IComparable comp = (IComparable)obj;

			if (comp != null)
			{
				if (object.ReferenceEquals(obj, p1))
					return comp.CompareTo(p2);
				else if (object.ReferenceEquals(obj, p2))
					return -comp.CompareTo(p1);
			}

			throw new InternalErrorException("unexpected case");
		}


		private DynValue MultiDispatchLessThanOrEqual(Script script, object obj)
		{
			IComparable comp = obj as IComparable;
			if (comp != null)
			{
				return DynValue.NewCallback(
					(context, args) =>
						DynValue.NewBoolean(PerformComparison(obj, args[0].ToObject(), args[1].ToObject()) <= 0));
			}

			return null;
		}

		private DynValue MultiDispatchLessThan(Script script, object obj)
		{
			IComparable comp = obj as IComparable;
			if (comp != null)
			{
				return DynValue.NewCallback(
					(context, args) =>
						DynValue.NewBoolean(PerformComparison(obj, args[0].ToObject(), args[1].ToObject()) < 0));
			}

			return null;
		}

		private DynValue TryDispatchLength(Script script, object obj)
		{
			if (obj == null) return null;

			var lenprop = m_Members.GetOrDefault("Length");
			if (lenprop != null && lenprop.CanRead() && !lenprop.CanExecute()) return lenprop.GetGetterCallbackAsDynValue(script, obj);

			var countprop = m_Members.GetOrDefault("Count");
			if (countprop != null && countprop.CanRead() && !countprop.CanExecute()) return countprop.GetGetterCallbackAsDynValue(script, obj);

			return null;
		}


		private DynValue MultiDispatchEqual(Script script, object obj)
		{
			return DynValue.NewCallback(
				(context, args) => DynValue.NewBoolean(CheckEquality(obj, args[0].ToObject(), args[1].ToObject())));
		}


		private bool CheckEquality(object obj, object p1, object p2)
		{
			if (obj != null)
			{
				if (object.ReferenceEquals(obj, p1))
					return obj.Equals(p2);
				else if (object.ReferenceEquals(obj, p2))
					return obj.Equals(p1);
			}

			if (p1 != null) return p1.Equals(p2);
			else if (p2 != null) return p2.Equals(p1);
			else return true;
		}

		private DynValue DispatchMetaOnMethod(Script script, object obj, string methodName)
		{
			IMemberDescriptor desc = m_Members.GetOrDefault(methodName);

			if (desc != null)
			{
				return desc.GetValue(script, obj);
			}
			else
				return null;
		}


		private DynValue TryDispatchToNumber(Script script, object obj)
		{
			foreach (Type t in NumericConversions.NumericTypesOrdered)
			{
				var name = t.GetConversionMethodName();
				var v = DispatchMetaOnMethod(script, obj, name);
				if (v != null) return v;
			}
			return null;
		}


		private DynValue TryDispatchToBool(Script script, object obj)
		{
			var name = typeof(bool).GetConversionMethodName();
			var v = DispatchMetaOnMethod(script, obj, name);
			if (v != null) return v;
			return DispatchMetaOnMethod(script, obj, "op_True");
		}

		#endregion


		/// <summary>
		/// Determines whether the specified object is compatible with the specified type.
		/// Unless a very specific behaviour is needed, the correct implementation is a 
		/// simple " return type.IsInstanceOfType(obj); "
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		public virtual bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}
	}
}
