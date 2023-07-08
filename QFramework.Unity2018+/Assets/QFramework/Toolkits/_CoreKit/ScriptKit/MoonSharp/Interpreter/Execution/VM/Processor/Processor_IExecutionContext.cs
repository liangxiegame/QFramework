
namespace MoonSharp.Interpreter.Execution.VM
{
	sealed partial class Processor
	{
		internal Table GetMetatable(DynValue value)
		{
			if (value.Type == DataType.Table)
			{
				return value.Table.MetaTable;
			}
			else if (value.Type.CanHaveTypeMetatables())
			{
				return m_Script.GetTypeMetatable(value.Type);
			}
			else
			{
				return null;
			}
		}

		internal DynValue GetBinaryMetamethod(DynValue op1, DynValue op2, string eventName)
		{
			var op1_MetaTable = GetMetatable(op1);
			if (op1_MetaTable != null)
			{
				DynValue meta1 = op1_MetaTable.RawGet(eventName);
				if (meta1 != null && meta1.IsNotNil())
					return meta1;
			}

			var op2_MetaTable = GetMetatable(op2);
			if (op2_MetaTable != null)
			{
				DynValue meta2 = op2_MetaTable.RawGet(eventName);
				if (meta2 != null && meta2.IsNotNil())
					return meta2;
			}

			if (op1.Type == DataType.UserData)
			{
				DynValue meta = op1.UserData.Descriptor.MetaIndex(this.m_Script,
					op1.UserData.Object, eventName);

				if (meta != null)
					return meta;
			}

			if (op2.Type == DataType.UserData)
			{
				DynValue meta = op2.UserData.Descriptor.MetaIndex(this.m_Script,
					op2.UserData.Object, eventName);

				if (meta != null)
					return meta;
			}

			return null;
		}

		internal DynValue GetMetamethod(DynValue value, string metamethod)
		{
			if (value.Type == DataType.UserData)
			{
				DynValue v = value.UserData.Descriptor.MetaIndex(m_Script, value.UserData.Object, metamethod);
				if (v != null)
					return v;
			}

			return GetMetamethodRaw(value, metamethod);
		}


		internal DynValue GetMetamethodRaw(DynValue value, string metamethod)
		{
			var metatable = GetMetatable(value);

			if (metatable == null)
				return null;

			var metameth = metatable.RawGet(metamethod);
			
			if (metameth == null || metameth.IsNil())
				return null;

			return metameth;
		}

		internal Script GetScript()
		{
			return m_Script;
		}
	}
}
