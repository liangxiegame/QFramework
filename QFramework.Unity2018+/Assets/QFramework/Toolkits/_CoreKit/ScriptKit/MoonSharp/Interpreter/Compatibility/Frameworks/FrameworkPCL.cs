#if !(DOTNET_CORE || NETFX_CORE) && PCL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	class FrameworkCurrent : FrameworkClrBase
	{
		public override Type GetTypeInfoFromType(Type t)
		{
			return t;
		}

		public override bool IsDbNull(object o)
		{
			return o != null && o.GetType().FullName.StartsWith("System.DBNull");
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr.ToString());
		}

		public override Type GetInterface(Type type, string name)
		{
			return type.GetInterfaces().
				FirstOrDefault(t => t.Name == name);
		}
	}
}

#endif