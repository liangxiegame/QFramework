using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter.Tree.Statements
{
	class ScopeBlockStatement : Statement
	{
		Statement m_Block;
		RuntimeScopeBlock m_StackFrame;
		SourceRef m_Do, m_End;

		public ScopeBlockStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			lcontext.Scope.PushBlock();

			m_Do = CheckTokenType(lcontext, TokenType.Do).GetSourceRef();

			m_Block = new CompositeStatement(lcontext);

			m_End = CheckTokenType(lcontext, TokenType.End).GetSourceRef();

			m_StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(m_Do);
			lcontext.Source.Refs.Add(m_End);
		}



		public override void Compile(Execution.VM.ByteCode bc)
		{
			using(bc.EnterSource(m_Do))
				bc.Emit_Enter(m_StackFrame);

			m_Block.Compile(bc);

			using (bc.EnterSource(m_End))
				bc.Emit_Leave(m_StackFrame);
		}

	}
}
