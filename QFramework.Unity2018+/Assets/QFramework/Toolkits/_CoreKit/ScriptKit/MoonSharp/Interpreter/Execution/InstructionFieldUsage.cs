using System;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Execution
{
	[Flags]
	internal enum InstructionFieldUsage
	{
		None = 0,
		Symbol = 0x1,
		SymbolList = 0x2,
		Name = 0x4,
		Value = 0x8,
		NumVal = 0x10,
		NumVal2 = 0x20,
		NumValAsCodeAddress = 0x8010
	}

	internal static class InstructionFieldUsage_Extensions
	{
		internal static InstructionFieldUsage GetFieldUsage(this OpCode op)
		{
			switch (op)
			{
				case OpCode.TblInitN:
				case OpCode.Scalar:
				case OpCode.IterUpd:
				case OpCode.IterPrep:
				case OpCode.NewTable:
				case OpCode.Concat:
				case OpCode.LessEq:
				case OpCode.Less:
				case OpCode.Eq:
				case OpCode.Add:
				case OpCode.Sub:
				case OpCode.Mul:
				case OpCode.Div:
				case OpCode.Mod:
				case OpCode.Not:
				case OpCode.Len:
				case OpCode.Neg:
				case OpCode.Power:
				case OpCode.CNot:
				case OpCode.ToBool:
					return InstructionFieldUsage.None;
				case OpCode.Pop:
				case OpCode.Copy:
				case OpCode.TblInitI:
				case OpCode.ExpTuple:
				case OpCode.Incr:
				case OpCode.ToNum:
				case OpCode.Ret:
				case OpCode.MkTuple:
					return InstructionFieldUsage.NumVal;
				case OpCode.Jump:
				case OpCode.Jf:
				case OpCode.JNil:
				case OpCode.JFor:
				case OpCode.JtOrPop:
				case OpCode.JfOrPop:
					return InstructionFieldUsage.NumValAsCodeAddress;
				case OpCode.Swap:
				case OpCode.Clean:
					return InstructionFieldUsage.NumVal | InstructionFieldUsage.NumVal2;
				case OpCode.Local:
				case OpCode.Upvalue:
					return InstructionFieldUsage.Symbol;
				case OpCode.IndexSet:
				case OpCode.IndexSetN:
				case OpCode.IndexSetL:
					return InstructionFieldUsage.Symbol | InstructionFieldUsage.Value | InstructionFieldUsage.NumVal | InstructionFieldUsage.NumVal2;
				case OpCode.StoreLcl:
				case OpCode.StoreUpv:
					return InstructionFieldUsage.Symbol | InstructionFieldUsage.NumVal | InstructionFieldUsage.NumVal2;
				case OpCode.Index:
				case OpCode.IndexL:
				case OpCode.IndexN:
				case OpCode.Literal:
					return InstructionFieldUsage.Value;
				case OpCode.Args:
					return InstructionFieldUsage.SymbolList;
				case OpCode.BeginFn:
					return InstructionFieldUsage.SymbolList | InstructionFieldUsage.NumVal | InstructionFieldUsage.NumVal2;
				case OpCode.Closure:
					return InstructionFieldUsage.SymbolList | InstructionFieldUsage.NumVal;
				case OpCode.Nop:
				case OpCode.Debug:
				case OpCode.Invalid:
					return InstructionFieldUsage.Name;
				case OpCode.Call:
				case OpCode.ThisCall:
					return InstructionFieldUsage.NumVal | InstructionFieldUsage.Name;
				case OpCode.Meta:
					return InstructionFieldUsage.NumVal | InstructionFieldUsage.NumVal2 | InstructionFieldUsage.Value | InstructionFieldUsage.Name;
				default:
					throw new NotImplementedException(string.Format("InstructionFieldUsage for instruction {0}", (int)op));
			}
		}
	}















}
