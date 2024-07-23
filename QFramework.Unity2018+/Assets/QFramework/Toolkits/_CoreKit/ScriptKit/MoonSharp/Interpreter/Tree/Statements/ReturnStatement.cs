using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;

using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	class ReturnStatement: Statement
	{
		Expression m_Expression = null;
		SourceRef m_Ref;

		public ReturnStatement(ScriptLoadingContext lcontext, Expression e, SourceRef sref)
			: base(lcontext)
		{
			m_Expression = e;
			m_Ref = sref;
			lcontext.Source.Refs.Add(sref);
		}




		public ReturnStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			Token ret = lcontext.Lexer.Current;

			lcontext.Lexer.Next();

			Token cur = lcontext.Lexer.Current;

			if (cur.IsEndOfBlock() || cur.Type == TokenType.SemiColon)
			{
				m_Expression = null;
				m_Ref = ret.GetSourceRef();
			}
			else
			{
				m_Expression = new ExprListExpression(Expression.ExprList(lcontext), lcontext);
				m_Ref = ret.GetSourceRefUpTo(lcontext.Lexer.Current);
			}
			lcontext.Source.Refs.Add(m_Ref);
		}



		public override void Compile(Execution.VM.ByteCode bc)
		{
			using (bc.EnterSource(m_Ref))
			{
				if (m_Expression != null)
				{
					m_Expression.Compile(bc);
					bc.Emit_Ret(1);
				}
				else
				{
					bc.Emit_Ret(0);
				}
			}
		}
	}
}
