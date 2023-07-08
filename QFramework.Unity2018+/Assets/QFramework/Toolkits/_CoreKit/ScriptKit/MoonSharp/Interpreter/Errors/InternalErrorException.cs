using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Exception thrown when an inconsistent state is reached in the interpreter
	/// </summary>
#if !(PCL || ((!UNITY_EDITOR) && (ENABLE_DOTNET)) || NETFX_CORE)
	[Serializable]
#endif
	public class InternalErrorException : InterpreterException
	{
		internal InternalErrorException(string message)
			: base(message)
		{

		}

		internal InternalErrorException(string format, params object[] args)
			: base(format, args)
		{

		}
	}

}
