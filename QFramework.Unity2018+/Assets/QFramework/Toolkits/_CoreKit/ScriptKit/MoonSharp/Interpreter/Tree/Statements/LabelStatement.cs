using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter.Tree.Statements
{
	class LabelStatement : Statement
	{
		public string Label { get; private set; }
		public int Address { get; private set; }
		public SourceRef SourceRef { get; private set; }
		public Token NameToken { get; private set; }

		internal int DefinedVarsCount { get; private set; }
		internal string LastDefinedVarName { get; private set; }

		List<GotoStatement> m_Gotos = new List<GotoStatement>();
		RuntimeScopeBlock m_StackFrame;


		public LabelStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			CheckTokenType(lcontext, TokenType.DoubleColon);
			NameToken = CheckTokenType(lcontext, TokenType.Name);
			CheckTokenType(lcontext, TokenType.DoubleColon);

			SourceRef = NameToken.GetSourceRef();
			Label = NameToken.Text;

			lcontext.Scope.DefineLabel(this);
		}

		internal void SetDefinedVars(int definedVarsCount, string lastDefinedVarsName)
		{
			DefinedVarsCount = definedVarsCount;
			LastDefinedVarName = lastDefinedVarsName;
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			m_Gotos.Add(gotostat);
		}


		public override void Compile(Execution.VM.ByteCode bc)
		{
			bc.Emit_Clean(m_StackFrame);

			Address = bc.GetJumpPointForLastInstruction();

			foreach (var gotostat in m_Gotos)
				gotostat.SetAddress(this.Address);
		}

		internal void SetScope(RuntimeScopeBlock runtimeScopeBlock)
		{
			m_StackFrame = runtimeScopeBlock;
		}
	}
}

