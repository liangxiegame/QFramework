using System;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Standard user data descriptor used to instantiate generics.
	/// </summary>
	public class StandardGenericsUserDataDescriptor : IUserDataDescriptor, IGeneratorUserDataDescriptor
	{
		/// <summary>
		/// Gets the interop access mode this descriptor uses for members access
		/// </summary>
		public InteropAccessMode AccessMode { get; private set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="StandardUserDataDescriptor"/> class.
		/// </summary>
		/// <param name="type">The type this descriptor refers to.</param>
		/// <param name="accessMode">The interop access mode this descriptor uses for members access</param>
		public StandardGenericsUserDataDescriptor(Type type, InteropAccessMode accessMode)
		{
			if (accessMode == InteropAccessMode.NoReflectionAllowed)
				throw new ArgumentException("Can't create a StandardGenericsUserDataDescriptor under a NoReflectionAllowed access mode");

			AccessMode = accessMode;
			this.Type = type;
			this.Name = "@@" + type.FullName;
		}


		/// <inheritdoc/>
		public string Name { get; private set; }

		/// <inheritdoc/>
		public Type Type { get; private set; }

		/// <inheritdoc/>
		public DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing)
		{
			return null;
		}

		/// <inheritdoc/>
		public bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing)
		{
			return false;
		}

		/// <inheritdoc/>
		public string AsString(object obj)
		{
			return obj.ToString();
		}

		/// <inheritdoc/>
		public DynValue MetaIndex(Script script, object obj, string metaname)
		{
			return null;
		}

		/// <inheritdoc/>
		public bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}

		/// <inheritdoc/>
		public IUserDataDescriptor Generate(Type type)
		{
			if (UserData.IsTypeRegistered(type))
				return null;

			if (Framework.Do.IsGenericTypeDefinition(type))
				return null;

			return UserData.RegisterType(type, AccessMode);
		}
	}
}
