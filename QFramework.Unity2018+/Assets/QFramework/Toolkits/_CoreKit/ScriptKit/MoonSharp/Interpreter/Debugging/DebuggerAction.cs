using System;

namespace MoonSharp.Interpreter.Debugging
{
	/// <summary>
	/// Wrapper for a debugger initiated action 
	/// </summary>
	public class DebuggerAction
	{
		/// <summary>
		/// Type of the action
		/// </summary>
		public enum ActionType
		{
			/// <summary>
			/// Step-in at the bytecode level
			/// </summary>
			ByteCodeStepIn,
			/// <summary>
			/// Step-over at the bytecode level
			/// </summary>
			ByteCodeStepOver,
			/// <summary>
			/// Step-out at the bytecode level
			/// </summary>
			ByteCodeStepOut,
			/// <summary>
			/// Step-in at the source level
			/// </summary>
			StepIn,
			/// <summary>
			/// Step-over at the source level
			/// </summary>
			StepOver,
			/// <summary>
			/// Step-out at the source level
			/// </summary>
			StepOut,
			/// <summary>
			/// Continue execution "freely"
			/// </summary>
			Run,
			/// <summary>
			/// Toggles breakpoint 
			/// </summary>
			ToggleBreakpoint,
			/// <summary>
			/// Sets a breakpoint
			/// </summary>
			SetBreakpoint,
			/// <summary>
			/// Clears a breakpoint
			/// </summary>
			ClearBreakpoint,
			/// <summary>
			/// Reset all breakpoints
			/// </summary>
			ResetBreakpoints,
			/// <summary>
			/// Refresh the data
			/// </summary>
			Refresh,
			/// <summary>
			/// Hard refresh of data
			/// </summary>
			HardRefresh,
			/// <summary>
			/// No action
			/// </summary>
			None,
		}

		/// <summary>
		/// The type of action
		/// </summary>
		public ActionType Action { get; set; }
		/// <summary>
		/// Gets the time stamp UTC of this action
		/// </summary>
		public DateTime TimeStampUTC { get; private set; }

		/// <summary>
		/// Gets or sets the source identifier this action refers to. <see cref="Script.GetSourceCode"/>
		/// </summary>
		public int SourceID { get; set; }
		/// <summary>
		/// Gets or sets the source line this action refers to.
		/// </summary>
		public int SourceLine { get; set; }
		/// <summary>
		/// Gets or sets the source column this action refers to.
		/// </summary>
		public int SourceCol { get; set; }
		/// <summary>
		/// Gets or sets the lines. This is used for the ResetBreakpoints and sets line-based bps only.
		/// </summary>
		public int[] Lines { get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="DebuggerAction"/> class.
		/// </summary>
		public DebuggerAction()
		{
			TimeStampUTC = DateTime.UtcNow;
		}

		/// <summary>
		/// Gets the age of this debugger action
		/// </summary>
		public TimeSpan Age { get { return DateTime.UtcNow - TimeStampUTC; } }


		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (Action == ActionType.ToggleBreakpoint || Action == ActionType.SetBreakpoint || Action == ActionType.ClearBreakpoint)
			{
				return string.Format("{0} {1}:({2},{3})", Action, SourceID, SourceLine, SourceCol);
			}
			else
			{
				return Action.ToString();
			}
		}
	}
}
