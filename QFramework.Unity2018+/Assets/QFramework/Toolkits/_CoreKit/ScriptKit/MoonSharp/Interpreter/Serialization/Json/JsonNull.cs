using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Serialization.Json
{
	/// <summary>
	/// UserData representing a null value in a table converted from Json
	/// </summary>
	public sealed class JsonNull
	{
		public static bool isNull() { return true; }

		[MoonSharpHidden]
		public static bool IsJsonNull(DynValue v)
		{
			return v.Type == DataType.UserData &&
				v.UserData.Descriptor != null &&
				v.UserData.Descriptor.Type == typeof(JsonNull);
		}

		[MoonSharpHidden]
		public static DynValue Create()
		{
			return UserData.CreateStatic<JsonNull>();
		}
	}
}
