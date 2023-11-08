using System;

namespace MoonSharp.Interpreter.REPL
{
	/// <summary>
	/// This class provides a simple REPL intepreter ready to be reused in a simple way.
	/// </summary>
	public class ReplInterpreter
	{
		Script m_Script;
		string m_CurrentCommand = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplInterpreter"/> class.
		/// </summary>
		/// <param name="script">The script.</param>
		public ReplInterpreter(Script script)
		{
			this.m_Script = script;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instances handle inputs starting with a "?" as a 
		/// dynamic expression to evaluate instead of script code (likely invalid)
		/// </summary>
		public bool HandleDynamicExprs { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instances handle inputs starting with a "=" as a 
		/// non-dynamic expression to evaluate (just like the Lua interpreter does by default).
		/// </summary>
		public bool HandleClassicExprsSyntax { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance has a pending command 
		/// </summary>
		public virtual bool HasPendingCommand { get { return m_CurrentCommand.Length > 0; } }

		/// <summary>
		/// Gets the current pending command.
		/// </summary>
		public virtual string CurrentPendingCommand { get { return m_CurrentCommand; } }

		/// <summary>
		/// Gets the classic prompt (">" or ">>") given the current state of the interpreter
		/// </summary>
		public virtual string ClassicPrompt { get { return HasPendingCommand ? ">>" : ">"; } }

		/// <summary>
		/// Evaluate a REPL command.
		/// This method returns the result of the computation, or null if more input is needed for having valid code.
		/// In case of errors, exceptions are propagated to the caller.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>This method returns the result of the computation, or null if more input is needed for a computation.</returns>
		public virtual DynValue Evaluate(string input)
		{
			bool isFirstLine = !HasPendingCommand;

			bool forced = (input == "");

			m_CurrentCommand += input;

			if (m_CurrentCommand.Length == 0)
				return DynValue.Void;

			m_CurrentCommand += "\n";

			try
			{
				DynValue result = null;

				if (isFirstLine && HandleClassicExprsSyntax && m_CurrentCommand.StartsWith("="))
				{
					m_CurrentCommand = "return " + m_CurrentCommand.Substring(1); 
				}
				
				if (isFirstLine && HandleDynamicExprs && m_CurrentCommand.StartsWith("?"))
				{
					var code = m_CurrentCommand.Substring(1);
					var exp = m_Script.CreateDynamicExpression(code);
					result = exp.Evaluate();
				}
				else
				{
					var v = m_Script.LoadString(m_CurrentCommand, null, "stdin");
					result = m_Script.Call(v);
				}

				m_CurrentCommand = "";
				return result;
			}
			catch (SyntaxErrorException ex)
			{
				if (forced || !ex.IsPrematureStreamTermination)
				{
					m_CurrentCommand = "";
					ex.Rethrow();
					throw;
				}
				else
				{
					return null;
				}
			}
			catch (ScriptRuntimeException sre)
			{
				m_CurrentCommand = "";
				sre.Rethrow();
				throw;
			}
			catch (Exception)
			{
				m_CurrentCommand = "";
				throw;
			}
		}
	}
}
