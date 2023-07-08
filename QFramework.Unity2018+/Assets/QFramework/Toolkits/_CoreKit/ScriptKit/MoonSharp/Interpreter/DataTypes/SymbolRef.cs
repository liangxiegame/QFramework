using System.Collections.Generic;
using System.IO;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// This class stores a possible l-value (that is a potential target of an assignment)
	/// </summary>
	public class SymbolRef
	{
		private static SymbolRef s_DefaultEnv = new SymbolRef() { i_Type = SymbolRefType.DefaultEnv };

		// Fields are internal - direct access by the executor was a 10% improvement at profiling here!
		internal SymbolRefType i_Type;
		internal SymbolRef i_Env;
		internal int i_Index;
		internal string i_Name;

		/// <summary>
		/// Gets the type of this symbol reference
		/// </summary>
		public SymbolRefType Type { get { return i_Type; } }
		/// <summary>
		/// Gets the index of this symbol in its scope context
		/// </summary>
		public int Index { get { return i_Index; } }
		/// <summary>
		/// Gets the name of this symbol
		/// </summary>
		public string Name { get { return i_Name; } }
		/// <summary>
		/// Gets the environment this symbol refers to (for global symbols only)
		/// </summary>
		public SymbolRef Environment { get { return i_Env; } }


		/// <summary>
		/// Gets the default _ENV.
		/// </summary>
		public static SymbolRef DefaultEnv { get { return s_DefaultEnv; } }

		/// <summary>
		/// Creates a new symbol reference pointing to a global var
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="envSymbol">The _ENV symbol.</param>
		/// <returns></returns>
		public static SymbolRef Global(string name, SymbolRef envSymbol)
		{
			return new SymbolRef() { i_Index = -1, i_Type = SymbolRefType.Global, i_Env = envSymbol, i_Name = name };
		}

		/// <summary>
		/// Creates a new symbol reference pointing to a local var
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="index">The index of the var in local scope.</param>
		/// <returns></returns>
		internal static SymbolRef Local(string name, int index)
		{
			//Debug.Assert(index >= 0, "Symbol Index < 0");
			return new SymbolRef() { i_Index = index, i_Type = SymbolRefType.Local, i_Name = name };
		}

		/// <summary>
		/// Creates a new symbol reference pointing to an upvalue var
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="index">The index of the var in closure scope.</param>
		/// <returns></returns>
		internal static SymbolRef Upvalue(string name, int index)
		{
			//Debug.Assert(index >= 0, "Symbol Index < 0");
			return new SymbolRef() { i_Index = index, i_Type = SymbolRefType.Upvalue, i_Name = name };
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (i_Type == SymbolRefType.DefaultEnv)
				return "(default _ENV)";
			else
			if (i_Type == SymbolRefType.Global)
				return string.Format("{2} : {0} / {1}", i_Type, i_Env, i_Name);
			else
				return string.Format("{2} : {0}[{1}]", i_Type, i_Index, i_Name);
		}

		/// <summary>
		/// Writes this instance to a binary stream
		/// </summary>
		internal void WriteBinary(BinaryWriter bw)
		{
			bw.Write((byte)this.i_Type);
			bw.Write(i_Index);
			bw.Write(i_Name);
		}

		/// <summary>
		/// Reads a symbolref from a binary stream 
		/// </summary>
		internal static SymbolRef ReadBinary(BinaryReader br)
		{
			SymbolRef that = new SymbolRef();
			that.i_Type = (SymbolRefType)br.ReadByte();
			that.i_Index = br.ReadInt32();
			that.i_Name = br.ReadString();
			return that;
		}

		internal void WriteBinaryEnv(BinaryWriter bw, Dictionary<SymbolRef, int> symbolMap)
		{
			if (this.i_Env != null)
				bw.Write(symbolMap[i_Env]);
			else
				bw.Write(-1);
		}

		internal void ReadBinaryEnv(BinaryReader br, SymbolRef[] symbolRefs)
		{
			int idx = br.ReadInt32();

			if (idx >= 0)
				i_Env = symbolRefs[idx];
		}
	}
}
