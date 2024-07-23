using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Tree.Expressions;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Tree
{
	abstract class Statement : NodeBase
	{
		public Statement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{ }


		protected static Statement CreateStatement(ScriptLoadingContext lcontext, out bool forceLast)
		{
			Token tkn = lcontext.Lexer.Current;

			forceLast = false;

			switch (tkn.Type)
			{
				case TokenType.DoubleColon:
					return new LabelStatement(lcontext);
				case TokenType.Goto:
					return new GotoStatement(lcontext);
				case TokenType.SemiColon:
					lcontext.Lexer.Next();
					return new EmptyStatement(lcontext);
				case TokenType.If:
					return new IfStatement(lcontext);
				case TokenType.While:
					return new WhileStatement(lcontext);
				case TokenType.Do:
					return new ScopeBlockStatement(lcontext);
				case TokenType.For:
					return DispatchForLoopStatement(lcontext);
				case TokenType.Repeat:
					return new RepeatStatement(lcontext);
				case TokenType.Function:
					return new FunctionDefinitionStatement(lcontext, false, null);
				case TokenType.Local:
					Token localToken = lcontext.Lexer.Current;
					lcontext.Lexer.Next();
					if (lcontext.Lexer.Current.Type == TokenType.Function)
						return new FunctionDefinitionStatement(lcontext, true, localToken);
					else
						return new AssignmentStatement(lcontext, localToken);
				case TokenType.Return:
					forceLast = true;
					return new ReturnStatement(lcontext);
				case TokenType.Break:
					return new BreakStatement(lcontext);
				default:
					{
						Token l = lcontext.Lexer.Current;
						Expression exp = Expression.PrimaryExp(lcontext);
						FunctionCallExpression fnexp = exp as FunctionCallExpression;

						if (fnexp != null)
							return new FunctionCallStatement(lcontext, fnexp);
						else
							return new AssignmentStatement(lcontext, exp, l);
					}
			}
		}

		private static Statement DispatchForLoopStatement(ScriptLoadingContext lcontext)
		{
			//	for Name ‘=’ exp ‘,’ exp [‘,’ exp] do block end | 
			//	for namelist in explist do block end | 		

			Token forTkn = CheckTokenType(lcontext, TokenType.For);

			Token name = CheckTokenType(lcontext, TokenType.Name);

			if (lcontext.Lexer.Current.Type == TokenType.Op_Assignment)
				return new ForLoopStatement(lcontext, name, forTkn);
			else
				return new ForEachLoopStatement(lcontext, name, forTkn);
		}




	}



}
