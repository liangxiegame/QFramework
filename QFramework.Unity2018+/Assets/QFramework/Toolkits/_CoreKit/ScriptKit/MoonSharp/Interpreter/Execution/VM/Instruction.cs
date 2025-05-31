using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter.Execution.VM
{
	internal class Instruction
	{
		internal OpCode OpCode;
		internal SymbolRef Symbol;
		internal SymbolRef[] SymbolList;
		internal string Name;
		internal DynValue Value;
		internal int NumVal;
		internal int NumVal2;
		internal SourceRef SourceCodeRef;

		internal Instruction(SourceRef sourceref)
		{
			SourceCodeRef = sourceref;
		}

		public override string ToString()
		{
			string append = this.OpCode.ToString().ToUpperInvariant();

			int usage = (int)OpCode.GetFieldUsage();

			if (usage != 0)
				append += GenSpaces();

			if ((this.OpCode == VM.OpCode.Meta) ||((usage & ((int)InstructionFieldUsage.NumValAsCodeAddress)) == (int)InstructionFieldUsage.NumValAsCodeAddress))
				append += " " + NumVal.ToString("X8");
			else if ((usage & ((int)InstructionFieldUsage.NumVal)) != 0)
				append += " " + NumVal.ToString();

			if ((usage & ((int)InstructionFieldUsage.NumVal2)) != 0)
				append += " " + NumVal2.ToString();

			if ((usage & ((int)InstructionFieldUsage.Name)) != 0)
				append += " " + Name;

			if ((usage & ((int)InstructionFieldUsage.Value)) != 0)
				append += " " + PurifyFromNewLines(Value);

			if ((usage & ((int)InstructionFieldUsage.Symbol)) != 0)
				append += " " + Symbol;

			if (((usage & ((int)InstructionFieldUsage.SymbolList)) != 0) && (SymbolList != null))
				append += " " + string.Join(",", SymbolList.Select(s => s.ToString()).ToArray());

			return append;
		}

		private string PurifyFromNewLines(DynValue Value)
		{
			if (Value == null)
				return "";

			return Value.ToString().Replace('\n', ' ').Replace('\r', ' ');
		}

		private string GenSpaces()
		{
			return new string(' ', 10 - this.OpCode.ToString().Length);
		}

		internal void WriteBinary(BinaryWriter wr, int baseAddress, Dictionary<SymbolRef, int> symbolMap)
		{
			wr.Write((byte)this.OpCode);

			int usage = (int)OpCode.GetFieldUsage();

			if ((usage & ((int)InstructionFieldUsage.NumValAsCodeAddress)) == (int)InstructionFieldUsage.NumValAsCodeAddress)
				wr.Write(this.NumVal - baseAddress);
			else if ((usage & ((int)InstructionFieldUsage.NumVal)) != 0)
				wr.Write(this.NumVal);

			if ((usage & ((int)InstructionFieldUsage.NumVal2)) != 0)
				wr.Write(this.NumVal2);

			if ((usage & ((int)InstructionFieldUsage.Name)) != 0)
				wr.Write(Name ?? "");

			if ((usage & ((int)InstructionFieldUsage.Value)) != 0)
				DumpValue(wr, Value);

			if ((usage & ((int)InstructionFieldUsage.Symbol)) != 0)
				WriteSymbol(wr, Symbol, symbolMap);

			if ((usage & ((int)InstructionFieldUsage.SymbolList)) != 0)
			{
				wr.Write(this.SymbolList.Length);
				for (int i = 0; i < this.SymbolList.Length; i++)
					WriteSymbol(wr, SymbolList[i], symbolMap);
			}
		}

		private static void WriteSymbol(BinaryWriter wr, SymbolRef symbolRef, Dictionary<SymbolRef, int> symbolMap)
		{
			int id = (symbolRef == null) ? -1 : symbolMap[symbolRef];
			wr.Write(id);
		}

		private static SymbolRef ReadSymbol(BinaryReader rd, SymbolRef[] deserializedSymbols)
		{
			int id = rd.ReadInt32();

			if (id < 0) return null;
			return deserializedSymbols[id];
		}

		internal static Instruction ReadBinary(SourceRef chunkRef, BinaryReader rd, int baseAddress, Table envTable, SymbolRef[] deserializedSymbols)
		{
			Instruction that = new Instruction(chunkRef);

			that.OpCode = (OpCode)rd.ReadByte();

			int usage = (int)that.OpCode.GetFieldUsage();

			if ((usage & ((int)InstructionFieldUsage.NumValAsCodeAddress)) == (int)InstructionFieldUsage.NumValAsCodeAddress)
				that.NumVal = rd.ReadInt32() + baseAddress;
			else if ((usage & ((int)InstructionFieldUsage.NumVal)) != 0)
				that.NumVal = rd.ReadInt32();

			if ((usage & ((int)InstructionFieldUsage.NumVal2)) != 0)
				that.NumVal2 = rd.ReadInt32();

			if ((usage & ((int)InstructionFieldUsage.Name)) != 0)
				that.Name = rd.ReadString();

			if ((usage & ((int)InstructionFieldUsage.Value)) != 0)
				that.Value = ReadValue(rd, envTable);

			if ((usage & ((int)InstructionFieldUsage.Symbol)) != 0)
				that.Symbol = ReadSymbol(rd, deserializedSymbols);

			if ((usage & ((int)InstructionFieldUsage.SymbolList)) != 0)
			{
				int len = rd.ReadInt32();
				that.SymbolList = new SymbolRef[len];

				for (int i = 0; i < that.SymbolList.Length; i++)
					that.SymbolList[i] = ReadSymbol(rd, deserializedSymbols);
			}

			return that;
		}

		private static DynValue ReadValue(BinaryReader rd, Table envTable)
		{
			bool isnull = !rd.ReadBoolean();

			if (isnull) return null;

			DataType dt = (DataType)rd.ReadByte();

			switch (dt)
			{
				case DataType.Nil:
					return DynValue.NewNil();
				case DataType.Void:
					return DynValue.Void;
				case DataType.Boolean:
					return DynValue.NewBoolean(rd.ReadBoolean());
				case DataType.Number:
					return DynValue.NewNumber(rd.ReadDouble());
				case DataType.String:
					return DynValue.NewString(rd.ReadString());
				case DataType.Table :
					return DynValue.NewTable(envTable);
				default:
					throw new NotSupportedException(string.Format("Unsupported type in chunk dump : {0}", dt));
			}
		}


		private void DumpValue(BinaryWriter wr, DynValue value)
		{
			if (value == null)
			{
				wr.Write(false);
				return;
			}

			wr.Write(true);
			wr.Write((byte)value.Type);

			switch (value.Type)
			{
				case DataType.Nil:
				case DataType.Void:
				case DataType.Table:
					break;
				case DataType.Boolean:
					wr.Write(value.Boolean);
					break;
				case DataType.Number:
					wr.Write(value.Number);
					break;
				case DataType.String:
					wr.Write(value.String);
					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported type in chunk dump : {0}", value.Type));
			}
		}

		internal void GetSymbolReferences(out SymbolRef[] symbolList, out SymbolRef symbol)
		{
			int usage = (int)OpCode.GetFieldUsage();

			symbol = null;
			symbolList = null;

			if ((usage & ((int)InstructionFieldUsage.Symbol)) != 0)
				symbol = this.Symbol;

			if ((usage & ((int)InstructionFieldUsage.SymbolList)) != 0)
				symbolList = this.SymbolList;

		}
	}
}
