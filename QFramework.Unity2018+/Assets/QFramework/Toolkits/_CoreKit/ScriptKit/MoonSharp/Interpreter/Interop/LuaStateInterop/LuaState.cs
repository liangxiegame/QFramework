// Disable warnings about XML documentation
#pragma warning disable 1591

using System.Collections.Generic;

namespace MoonSharp.Interpreter.Interop.LuaStateInterop
{
	/// <summary>
	/// 
	/// </summary>
	public class LuaState
	{
		private List<DynValue> m_Stack;

		public ScriptExecutionContext ExecutionContext { get; private set; }
		public string FunctionName { get; private set; }

		internal LuaState(ScriptExecutionContext executionContext, CallbackArguments args, string functionName)
		{
			ExecutionContext = executionContext;
			m_Stack = new List<DynValue>(16);

			for (int i = 0; i < args.Count; i++)
				m_Stack.Add(args[i]);

			FunctionName = functionName;
		}

		public DynValue Top(int pos = 0)
		{
			return m_Stack[m_Stack.Count - 1 - pos];
		}

		public DynValue At(int pos)
		{
			if (pos < 0)
				pos = m_Stack.Count + pos + 1;

			if (pos > m_Stack.Count)
				return DynValue.Void;

			return m_Stack[pos - 1];
		}

		public int Count
		{
			get { return m_Stack.Count; }
		}

		public void Push(DynValue v)
		{
			m_Stack.Add(v);
		}

		public DynValue Pop()
		{
			var v = Top();
			m_Stack.RemoveAt(m_Stack.Count - 1);
			return v;
		}

		public DynValue[] GetTopArray(int num)
		{
			DynValue[] rets = new DynValue[num];

			for (int i = 0; i < num; i++)
				rets[num - i - 1] = Top(i);

			return rets;
		}


		public DynValue GetReturnValue(int retvals)
		{
			if (retvals == 0)
				return DynValue.Nil;
			else if (retvals == 1)
				return Top();
			else
			{
				DynValue[] rets = GetTopArray(retvals);
				return DynValue.NewTupleNested(rets);
			}
		}



		public void Discard(int nargs)
		{
			for(int i = 0; i < nargs; i++)
				m_Stack.RemoveAt(m_Stack.Count - 1);
		}
	}
}
