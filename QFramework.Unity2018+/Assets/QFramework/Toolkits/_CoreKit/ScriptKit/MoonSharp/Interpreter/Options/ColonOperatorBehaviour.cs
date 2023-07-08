using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Defines behaviour of the colon ':' operator in CLR callbacks.
	/// Default behaviour is for ':' being treated the same as a '.' if the functions is implemented on the CLR side (e.g. in C#).
	/// </summary>
	public enum ColonOperatorBehaviour
	{
		/// <summary>
		/// The colon is treated the same as the dot ('.') operator.
		/// </summary>
		TreatAsDot,
		/// <summary>
		/// The colon is treated the same as the dot ('.') operator if the first argument is userdata, as a Lua colon operator otherwise.
		/// </summary>
		TreatAsDotOnUserData,
		/// <summary>
		/// The colon is treated in the same as the Lua colon operator works.
		/// </summary>
		TreatAsColon
	}
}
