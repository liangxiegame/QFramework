using System.Collections.Generic;

using MoonSharp.Interpreter.Execution;


namespace MoonSharp.Interpreter.Tree.Statements
{
	class CompositeStatement : Statement 
	{
		List<Statement> m_Statements = new List<Statement>();

		public CompositeStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			while (true)
			{
				Token t = lcontext.Lexer.Current;
				if (t.IsEndOfBlock()) break;

				bool forceLast;
				
				Statement s = Statement.CreateStatement(lcontext, out forceLast);
				m_Statements.Add(s);

				if (forceLast) break;
			}

			// eat away all superfluos ';'s
			while (lcontext.Lexer.Current.Type == TokenType.SemiColon)
				lcontext.Lexer.Next();
		}


		public override void Compile(Execution.VM.ByteCode bc)
		{
			if (m_Statements != null)
			{
				foreach (Statement s in m_Statements)
				{
					s.Compile(bc);
				}
			}
		}
	}
}
