using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// A class representing a script coroutine
	/// </summary>
	public class Coroutine : RefIdObject, IScriptPrivateResource
	{
		/// <summary>
		/// Possible types of coroutine
		/// </summary>
		public enum CoroutineType
		{
			/// <summary>
			/// A valid coroutine
			/// </summary>
			Coroutine,
			/// <summary>
			/// A CLR callback assigned to a coroutine. 
			/// </summary>
			ClrCallback,
			/// <summary>
			/// A CLR callback assigned to a coroutine and already executed.
			/// </summary>
			ClrCallbackDead
		}

		/// <summary>
		/// Gets the type of coroutine
		/// </summary>
		public  CoroutineType Type { get; private set; }

		private CallbackFunction m_ClrCallback;
		private Processor m_Processor;


		internal Coroutine(CallbackFunction function)
		{
			Type = CoroutineType.ClrCallback;
			m_ClrCallback = function;
			OwnerScript = null;
		}

		internal Coroutine(Processor proc)
		{
			Type = CoroutineType.Coroutine;
			m_Processor = proc;
			m_Processor.AssociatedCoroutine = this;
			OwnerScript = proc.GetScript();
		}

		internal void MarkClrCallbackAsDead()
		{
			if (Type != CoroutineType.ClrCallback)
				throw new InvalidOperationException("State must be CoroutineType.ClrCallback");

			Type = CoroutineType.ClrCallbackDead;
		}


		/// <summary>
		/// Gets this coroutine as a typed enumerable which can be looped over for resuming.
		/// Returns its result as DynValue(s)
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public IEnumerable<DynValue> AsTypedEnumerable()
		{
			if (Type != CoroutineType.Coroutine)
				throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");

			while (this.State == CoroutineState.NotStarted || this.State == CoroutineState.Suspended || this.State == CoroutineState.ForceSuspended)
				yield return Resume();
		}


		/// <summary>
		/// Gets this coroutine as a typed enumerable which can be looped over for resuming.
		/// Returns its result as System.Object. Only the first element of tuples is returned.
		/// Only non-CLR coroutines can be resumed with this method. Use an overload of the Resume method accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public IEnumerable<object> AsEnumerable()
		{
			foreach(DynValue v in AsTypedEnumerable())
			{
				yield return v.ToScalar().ToObject();
			}
		}

		/// <summary>
		/// Gets this coroutine as a typed enumerable which can be looped over for resuming.
		/// Returns its result as the specified type. Only the first element of tuples is returned.
		/// Only non-CLR coroutines can be resumed with this method. Use an overload of the Resume method accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public IEnumerable<T> AsEnumerable<T>()
		{
			foreach(DynValue v in AsTypedEnumerable())
			{
				yield return v.ToScalar().ToObject<T>();
			}
		}

		/// <summary>
		/// The purpose of this method is to convert a MoonSharp/Lua coroutine to a Unity3D coroutine.
		/// This loops over the coroutine, discarding returned values, and returning null for each invocation.
		/// This means however that the coroutine will be invoked each frame.
		/// Only non-CLR coroutines can be resumed with this method. Use an overload of the Resume method accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public System.Collections.IEnumerator AsUnityCoroutine()
		{
#pragma warning disable 0219
			foreach (DynValue v in AsTypedEnumerable())
			{
				yield return null;
			}
#pragma warning restore 0219
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public DynValue Resume(params DynValue[] args)
		{
			this.CheckScriptOwnership(args);

			if (Type == CoroutineType.Coroutine)
				return m_Processor.Coroutine_Resume(args);
			else 
				throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");
		}


		/// <summary>
		/// Resumes the coroutine.
		/// </summary>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public DynValue Resume(ScriptExecutionContext context, params DynValue[] args)
		{
			this.CheckScriptOwnership(context);
			this.CheckScriptOwnership(args);

			if (Type == CoroutineType.Coroutine)
				return m_Processor.Coroutine_Resume(args);
			else if (Type == CoroutineType.ClrCallback)
			{
				DynValue ret = m_ClrCallback.Invoke(context, args);
				MarkClrCallbackAsDead();
				return ret;
			}
			else
				throw ScriptRuntimeException.CannotResumeNotSuspended(CoroutineState.Dead);
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public DynValue Resume()
		{
			return Resume(new DynValue[0]);
		}


		/// <summary>
		/// Resumes the coroutine.
		/// </summary>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <returns></returns>
		public DynValue Resume(ScriptExecutionContext context)
		{
			return Resume(context, new DynValue[0]);
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.</exception>
		public DynValue Resume(params object[] args)
		{
			if (Type != CoroutineType.Coroutine)
				throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");

			DynValue[] dargs = new DynValue[args.Length];

			for (int i = 0; i < dargs.Length; i++)
				dargs[i] = DynValue.FromObject(this.OwnerScript, args[i]);

			return Resume(dargs);
		}


		/// <summary>
		/// Resumes the coroutine
		/// </summary>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public DynValue Resume(ScriptExecutionContext context, params object[] args)
		{
			DynValue[] dargs = new DynValue[args.Length];

			for (int i = 0; i < dargs.Length; i++)
				dargs[i] = DynValue.FromObject(context.GetScript(), args[i]);

			return Resume(context, dargs);
		}




		/// <summary>
		/// Gets the coroutine state.
		/// </summary>
		public CoroutineState State
		{
			get
			{
				if (Type == CoroutineType.ClrCallback)
					return CoroutineState.NotStarted;
				else if (Type == CoroutineType.ClrCallbackDead)
					return CoroutineState.Dead;
				else 
					return m_Processor.State;
			}
		}

		/// <summary>
		/// Gets the coroutine stack trace for debug purposes
		/// </summary>
		/// <param name="skip">The skip.</param>
		/// <param name="entrySourceRef">The entry source reference.</param>
		/// <returns></returns>
		public WatchItem[] GetStackTrace(int skip, SourceRef entrySourceRef = null)
		{
			if (this.State != CoroutineState.Running)
			{
				entrySourceRef = m_Processor.GetCoroutineSuspendedLocation();
			}

			List<WatchItem> stack = m_Processor.Debugger_GetCallStack(entrySourceRef);
			return stack.Skip(skip).ToArray();
		}

		/// <summary>
		/// Gets the script owning this resource.
		/// </summary>
		/// <value>
		/// The script owning this resource.
		/// </value>
		/// <exception cref="System.NotImplementedException"></exception>
		public Script OwnerScript
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the automatic yield counter.
		/// </summary>
		/// <value>
		/// The automatic yield counter.
		/// </value>
		public long AutoYieldCounter
		{
			get { return m_Processor.AutoYieldCounter; }
			set { m_Processor.AutoYieldCounter = value; }
		}
	}
}
