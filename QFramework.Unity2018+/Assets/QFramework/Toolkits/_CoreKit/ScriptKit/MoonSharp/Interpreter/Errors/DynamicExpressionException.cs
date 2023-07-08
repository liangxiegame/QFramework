
using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Exception thrown when a dynamic expression is invalid
	/// </summary>
#if !(PCL || ((!UNITY_EDITOR) && (ENABLE_DOTNET)) || NETFX_CORE)
	[Serializable]
#endif
	public class DynamicExpressionException : ScriptRuntimeException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicExpressionException"/> class.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		public DynamicExpressionException(string format, params object[] args)
			: base("<dynamic>: " + format, args)
		{

		}
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicExpressionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DynamicExpressionException(string message)
			: base("<dynamic>: " + message)
		{

		}
	}
}
