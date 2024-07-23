using System;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop.StandardDescriptors.HardwiredDescriptors
{
	public abstract class HardwiredUserDataDescriptor : DispatchingUserDataDescriptor
	{
		protected HardwiredUserDataDescriptor(Type T) :
			base(T, "::hardwired::" + T.Name)
		{

		}

	}
}
