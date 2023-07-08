#if DOTNET_CORE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	class FrameworkCurrent : FrameworkClrBase
	{
		public override Type GetInterface(Type type, string name)
		{
			return type.GetTypeInfo().GetInterface(name);
		}

		public override TypeInfo GetTypeInfoFromType(Type t)
		{
			return t.GetTypeInfo();
		}

		public override bool IsDbNull(object o)
		{
			return o != null && o.GetType().FullName.StartsWith("System.DBNull");
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr);
		}
	}
}
#endif
