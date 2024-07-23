#define EMIT_DEBUG_OPS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter.Execution.VM
{
	internal class ByteCode : RefIdObject
	{
		public List<Instruction> Code = new List<Instruction>();
		public Script Script { get; private set; }
		private List<SourceRef> m_SourceRefStack = new List<SourceRef>();
		private SourceRef m_CurrentSourceRef = null;

		internal LoopTracker LoopTracker = new LoopTracker();

		public ByteCode(Script script)
		{
			Script = script;
		}


		public IDisposable EnterSource(SourceRef sref)
		{
			return new SourceCodeStackGuard(sref, this);
		}


		private class SourceCodeStackGuard : IDisposable
		{
			ByteCode m_Bc;

			public SourceCodeStackGuard(SourceRef sref, ByteCode bc)
			{
				m_Bc = bc;
				m_Bc.PushSourceRef(sref);
			}

			public void Dispose()
			{
				m_Bc.PopSourceRef();
			}
		}


		public void PushSourceRef(SourceRef sref)
		{
			m_SourceRefStack.Add(sref);
			m_CurrentSourceRef = sref;
		}

		public void PopSourceRef()
		{
			m_SourceRefStack.RemoveAt(m_SourceRefStack.Count - 1);
			m_CurrentSourceRef = (m_SourceRefStack.Count > 0) ? m_SourceRefStack[m_SourceRefStack.Count - 1] : null;
		}

	#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE) && (!(NETFX_CORE))
		public void Dump(string file)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Code.Count; i++)
			{
				if (Code[i].OpCode == OpCode.Debug)
					sb.AppendFormat("    {0}\n", Code[i]);
				else
					sb.AppendFormat("{0:X8}  {1}\n", i, Code[i]);
			}

			File.WriteAllText(file, sb.ToString());
		}
	#endif

		public int GetJumpPointForNextInstruction()
		{
			return Code.Count;
		}
		public int GetJumpPointForLastInstruction()
		{
			return Code.Count - 1;
		}

		public Instruction GetLastInstruction()
		{
			return Code[Code.Count - 1];
		}

		private Instruction AppendInstruction(Instruction c)
		{
			Code.Add(c);
			return c;
		}

		public Instruction Emit_Nop(string comment)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Nop, Name = comment });
		}

		public Instruction Emit_Invalid(string type)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Invalid, Name = type });
		}

		public Instruction Emit_Pop(int num = 1)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Pop, NumVal = num });
		}

		public void Emit_Call(int argCount, string debugName)
		{
			AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Call, NumVal = argCount, Name = debugName });
		}

		public void Emit_ThisCall(int argCount, string debugName)
		{
			AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.ThisCall, NumVal = argCount, Name = debugName });
		}

		public Instruction Emit_Literal(DynValue value)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Literal, Value = value });
		}

		public Instruction Emit_Jump(OpCode jumpOpCode, int idx, int optPar = 0)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = jumpOpCode, NumVal = idx, NumVal2 = optPar });
		}

		public Instruction Emit_MkTuple(int cnt)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.MkTuple, NumVal = cnt });
		}

		public Instruction Emit_Operator(OpCode opcode)
		{
			var i = AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = opcode });

			if (opcode == OpCode.LessEq)
				AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.CNot });

			if (opcode == OpCode.Eq || opcode == OpCode.Less)
				AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.ToBool });

			return i;
		}


		[Conditional("EMIT_DEBUG_OPS")]
		public void Emit_Debug(string str)
		{
			AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Debug, Name = str.Substring(0, Math.Min(32, str.Length)) });
		}

		public Instruction Emit_Enter(RuntimeScopeBlock runtimeScopeBlock)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Clean, NumVal = runtimeScopeBlock.From, NumVal2 = runtimeScopeBlock.ToInclusive });
		}

		public Instruction Emit_Leave(RuntimeScopeBlock runtimeScopeBlock)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Clean, NumVal = runtimeScopeBlock.From, NumVal2 = runtimeScopeBlock.To });
		}

		public Instruction Emit_Exit(RuntimeScopeBlock runtimeScopeBlock)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Clean, NumVal = runtimeScopeBlock.From, NumVal2 = runtimeScopeBlock.ToInclusive });
		}

		public Instruction Emit_Clean(RuntimeScopeBlock runtimeScopeBlock)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Clean, NumVal = runtimeScopeBlock.To + 1, NumVal2 = runtimeScopeBlock.ToInclusive });
		}

		public Instruction Emit_Closure(SymbolRef[] symbols, int jmpnum)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Closure, SymbolList = symbols, NumVal = jmpnum });
		}

		public Instruction Emit_Args(params SymbolRef[] symbols)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Args, SymbolList = symbols });
		}

		public Instruction Emit_Ret(int retvals)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Ret, NumVal = retvals });
		}

		public Instruction Emit_ToNum(int stage = 0)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.ToNum, NumVal = stage });
		}

		public Instruction Emit_Incr(int i)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Incr, NumVal = i });
		}

		public Instruction Emit_NewTable(bool shared)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.NewTable, NumVal = shared ? 1 : 0 });
		}

		public Instruction Emit_IterPrep()
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.IterPrep });
		}

		public Instruction Emit_ExpTuple(int stackOffset)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.ExpTuple, NumVal = stackOffset });
		}

		public Instruction Emit_IterUpd()
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.IterUpd });
		}

		public Instruction Emit_Meta(string funcName, OpCodeMetadataType metaType, DynValue value = null)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef)
			{
				OpCode = OpCode.Meta,
				Name = funcName,
				NumVal2 = (int)metaType,
				Value = value
			});
		}


		public Instruction Emit_BeginFn(RuntimeScopeFrame stackFrame)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef)
			{
				OpCode = OpCode.BeginFn,
				SymbolList = stackFrame.DebugSymbols.ToArray(),
				NumVal = stackFrame.Count,
				NumVal2 = stackFrame.ToFirstBlock,
			});
		}

		public Instruction Emit_Scalar()
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Scalar });
		}

		public int Emit_Load(SymbolRef sym)
		{
			switch (sym.Type)
			{
				case SymbolRefType.Global:
					Emit_Load(sym.i_Env);
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Index, Value = DynValue.NewString(sym.i_Name) });
					return 2;
				case SymbolRefType.Local:
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Local, Symbol = sym });
					return 1;
				case SymbolRefType.Upvalue:
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Upvalue, Symbol = sym });
					return 1;
				default:
					throw new InternalErrorException("Unexpected symbol type : {0}", sym);
			}
		}

		public int Emit_Store(SymbolRef sym, int stackofs, int tupleidx)
		{
			switch (sym.Type)
			{
				case SymbolRefType.Global:
					Emit_Load(sym.i_Env);
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.IndexSet, Symbol = sym, NumVal = stackofs, NumVal2 = tupleidx, Value = DynValue.NewString(sym.i_Name) });
					return 2;
				case SymbolRefType.Local:
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.StoreLcl, Symbol = sym, NumVal = stackofs, NumVal2 = tupleidx });
					return 1;
				case SymbolRefType.Upvalue:
					AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.StoreUpv, Symbol = sym, NumVal = stackofs, NumVal2 = tupleidx });
					return 1;
				default:
					throw new InternalErrorException("Unexpected symbol type : {0}", sym);
			}
		}

		public Instruction Emit_TblInitN()
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.TblInitN });
		}

		public Instruction Emit_TblInitI(bool lastpos)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.TblInitI, NumVal = lastpos ? 1 : 0 });
		}

		public Instruction Emit_Index(DynValue index = null, bool isNameIndex = false, bool isExpList = false)
		{
			OpCode o;
			if (isNameIndex) o = OpCode.IndexN;
			else if (isExpList) o = OpCode.IndexL;
			else o = OpCode.Index;

			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = o, Value = index });
		}

		public Instruction Emit_IndexSet(int stackofs, int tupleidx, DynValue index = null, bool isNameIndex = false, bool isExpList = false)
		{
			OpCode o;
			if (isNameIndex) o = OpCode.IndexSetN;
			else if (isExpList) o = OpCode.IndexSetL;
			else o = OpCode.IndexSet;

			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = o, NumVal = stackofs, NumVal2 = tupleidx, Value = index });
		}

		public Instruction Emit_Copy(int numval)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Copy, NumVal = numval });
		}

		public Instruction Emit_Swap(int p1, int p2)
		{
			return AppendInstruction(new Instruction(m_CurrentSourceRef) { OpCode = OpCode.Swap, NumVal = p1, NumVal2 = p2 });
		}

	}
}
