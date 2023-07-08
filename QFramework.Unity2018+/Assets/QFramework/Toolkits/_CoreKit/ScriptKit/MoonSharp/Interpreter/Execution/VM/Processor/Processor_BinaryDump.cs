using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.IO;

namespace MoonSharp.Interpreter.Execution.VM
{
	sealed partial class Processor
	{
		const ulong DUMP_CHUNK_MAGIC = 0x1A0D234E4F4F4D1D;
		const int DUMP_CHUNK_VERSION = 0x150;

		internal static bool IsDumpStream(Stream stream)
		{
			if (stream.Length >= 8)
			{
				using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8))
				{
					ulong magic = br.ReadUInt64();
					stream.Seek(-8, SeekOrigin.Current);
					return magic == DUMP_CHUNK_MAGIC;
				}
			}
			return false;
		}

		internal int Dump(Stream stream, int baseAddress, bool hasUpvalues)
		{
			using (BinaryWriter bw = new BinDumpBinaryWriter(stream, Encoding.UTF8))
			{
				Dictionary<SymbolRef, int> symbolMap = new Dictionary<SymbolRef, int>();

				Instruction meta = FindMeta(ref baseAddress);

				if (meta == null)
					throw new ArgumentException("baseAddress");

				bw.Write(DUMP_CHUNK_MAGIC);
				bw.Write(DUMP_CHUNK_VERSION);
				bw.Write(hasUpvalues);
				bw.Write(meta.NumVal);

				for (int i = 0; i <= meta.NumVal; i++)
				{
					SymbolRef[] symbolList;
					SymbolRef symbol;

					m_RootChunk.Code[baseAddress + i].GetSymbolReferences(out symbolList, out symbol);

					if (symbol != null)
						AddSymbolToMap(symbolMap, symbol);

					if (symbolList != null)
						foreach (var s in symbolList)
							AddSymbolToMap(symbolMap, s);
				}

				foreach (SymbolRef sr in symbolMap.Keys.ToArray())
				{
					if (sr.i_Env != null)
						AddSymbolToMap(symbolMap, sr.i_Env);
				}

				SymbolRef[] allSymbols = new SymbolRef[symbolMap.Count];

				foreach (KeyValuePair<SymbolRef, int> pair in symbolMap)
				{
					allSymbols[pair.Value] = pair.Key;
				}

				bw.Write(symbolMap.Count);

				foreach (SymbolRef sym in allSymbols)
					sym.WriteBinary(bw);

				foreach (SymbolRef sym in allSymbols)
					sym.WriteBinaryEnv(bw, symbolMap);

				for (int i = 0; i <= meta.NumVal; i++)
					m_RootChunk.Code[baseAddress + i].WriteBinary(bw, baseAddress, symbolMap);

				return meta.NumVal + baseAddress + 1;
			}
		}

		private void AddSymbolToMap(Dictionary<SymbolRef, int> symbolMap, SymbolRef s)
		{
			if (!symbolMap.ContainsKey(s))
				symbolMap.Add(s, symbolMap.Count);
		}

		internal int Undump(Stream stream, int sourceID, Table envTable, out bool hasUpvalues)
		{
			int baseAddress = m_RootChunk.Code.Count;
			SourceRef sourceRef = new SourceRef(sourceID, 0, 0, 0, 0, false);

			using (BinaryReader br = new BinDumpBinaryReader(stream, Encoding.UTF8))
			{
				ulong headerMark = br.ReadUInt64();

				if (headerMark != DUMP_CHUNK_MAGIC)
					throw new ArgumentException("Not a MoonSharp chunk");

				int version = br.ReadInt32();

				if (version != DUMP_CHUNK_VERSION)
					throw new ArgumentException("Invalid version");

				hasUpvalues = br.ReadBoolean();

				int len = br.ReadInt32();

				int numSymbs = br.ReadInt32();
				SymbolRef[] allSymbs = new SymbolRef[numSymbs];

				for (int i = 0; i < numSymbs; i++)
					allSymbs[i] = SymbolRef.ReadBinary(br);

				for (int i = 0; i < numSymbs; i++)
					allSymbs[i].ReadBinaryEnv(br, allSymbs);

				for (int i = 0; i <= len; i++)
				{
					Instruction I = Instruction.ReadBinary(sourceRef, br, baseAddress, envTable, allSymbs);
					m_RootChunk.Code.Add(I);
				}

				return baseAddress;
			}
		}
	}
}
