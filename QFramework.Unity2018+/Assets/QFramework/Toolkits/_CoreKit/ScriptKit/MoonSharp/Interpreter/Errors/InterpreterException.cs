using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Base type of all exceptions thrown in MoonSharp
	/// </summary>
#if !(PCL || ((!UNITY_EDITOR) && (ENABLE_DOTNET)) || NETFX_CORE)
	[Serializable]
#endif
	public class InterpreterException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InterpreterException"/> class.
		/// </summary>
		/// <param name="ex">The ex.</param>
		protected InterpreterException(Exception ex, string message)
			: base(message, ex)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InterpreterException"/> class.
		/// </summary>
		/// <param name="ex">The ex.</param>
		protected InterpreterException(Exception ex)
			: base(ex.Message, ex)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InterpreterException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		protected InterpreterException(string message)
			: base(message)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InterpreterException"/> class.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		protected InterpreterException(string format, params object[] args)
			: base(string.Format(format, args))
		{

		}

		/// <summary>
		/// Gets the instruction pointer of the execution (if it makes sense)
		/// </summary>
		public int InstructionPtr { get; internal set; }

		/// <summary>
		/// Gets the interpreter call stack.
		/// </summary>
		public IList<MoonSharp.Interpreter.Debugging.WatchItem> CallStack { get; internal set; }

		/// <summary>
		/// Gets the decorated message (error message plus error location in script) if possible.
		/// </summary>
		public string DecoratedMessage { get; internal set; }


		/// <summary>
		/// Gets or sets a value indicating whether the message should not be decorated
		/// </summary>
		public bool DoNotDecorateMessage { get; set; }


		internal void DecorateMessage(Script script, SourceRef sref, int ip = -1)
		{
			if (string.IsNullOrEmpty(this.DecoratedMessage))
			{
				if (DoNotDecorateMessage)
				{
					this.DecoratedMessage = this.Message;
					return;
				}
				else if (sref != null)
				{
					this.DecoratedMessage = string.Format("{0}: {1}", sref.FormatLocation(script), this.Message);
				}
				else
				{
					this.DecoratedMessage = string.Format("bytecode:{0}: {1}", ip, this.Message);
				}
			}
		}


		/// <summary>
		/// Rethrows this instance if 
		/// </summary>
		/// <returns></returns>
		public virtual void Rethrow()
		{
		}



	}
}
