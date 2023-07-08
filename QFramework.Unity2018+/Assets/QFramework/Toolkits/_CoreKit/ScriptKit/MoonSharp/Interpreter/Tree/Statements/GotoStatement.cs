using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	class GotoStatement : Statement
	{
		internal SourceRef SourceRef { get; private set; }
		internal Token GotoToken { get; private set; }
		public string Label { get; private set; }

		internal int DefinedVarsCount { get; private set; }
		internal string LastDefinedVarName { get; private set; }

		Instruction m_Jump;
		int m_LabelAddress = -1;

		public GotoStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			GotoToken = CheckTokenType(lcontext, TokenType.Goto);
			Token name = CheckTokenType(lcontext, TokenType.Name);

			SourceRef = GotoToken.GetSourceRef(name);

			Label = name.Text;

			lcontext.Scope.RegisterGoto(this);
		}

		public override void Compile(ByteCode bc)
		{
			m_Jump = bc.Emit_Jump(OpCode.Jump, m_LabelAddress);
		}

		internal void SetDefinedVars(int definedVarsCount, string lastDefinedVarsName)
		{
			DefinedVarsCount = definedVarsCount;
			LastDefinedVarName = lastDefinedVarsName;
		}


		internal void SetAddress(int labelAddress)
		{
			m_LabelAddress = labelAddress;

			if (m_Jump != null)
				m_Jump.NumVal = labelAddress;
		}

	}
}
