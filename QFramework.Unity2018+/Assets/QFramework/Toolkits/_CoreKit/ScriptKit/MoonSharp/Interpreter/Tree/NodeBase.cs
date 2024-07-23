using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree
{
	abstract class NodeBase
	{
		public Script Script { get; private set; }
		protected ScriptLoadingContext LoadingContext { get; private set; }

		public NodeBase(ScriptLoadingContext lcontext)
		{
			Script = lcontext.Script;
		}


		public abstract void Compile(ByteCode bc);


		protected static Token UnexpectedTokenType(Token t)
		{
			throw new SyntaxErrorException(t, "unexpected symbol near '{0}'", t.Text)
			{
				IsPrematureStreamTermination = (t.Type == TokenType.Eof)
			};
		}

		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType)
		{
			Token t = lcontext.Lexer.Current;
			if (t.Type != tokenType)
				return UnexpectedTokenType(t);

			lcontext.Lexer.Next();

			return t;
		}



		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType1, TokenType tokenType2)
		{
			Token t = lcontext.Lexer.Current;
			if (t.Type != tokenType1 && t.Type != tokenType2)
				return UnexpectedTokenType(t);

			lcontext.Lexer.Next();

			return t;
		}
		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType1, TokenType tokenType2, TokenType tokenType3)
		{
			Token t = lcontext.Lexer.Current;
			if (t.Type != tokenType1 && t.Type != tokenType2 && t.Type != tokenType3)
				return UnexpectedTokenType(t);

			lcontext.Lexer.Next();

			return t;
		}

		protected static void CheckTokenTypeNotNext(ScriptLoadingContext lcontext, TokenType tokenType)
		{
			Token t = lcontext.Lexer.Current;
			if (t.Type != tokenType)
				UnexpectedTokenType(t);
		}

		protected static Token CheckMatch(ScriptLoadingContext lcontext, Token originalToken, TokenType expectedTokenType, string expectedTokenText)
		{
			Token t = lcontext.Lexer.Current;
			if (t.Type != expectedTokenType)
			{
				throw new SyntaxErrorException(lcontext.Lexer.Current,
					"'{0}' expected (to close '{1}' at line {2}) near '{3}'",
					expectedTokenText, originalToken.Text, originalToken.FromLine, t.Text)
										{
											IsPrematureStreamTermination = (t.Type == TokenType.Eof)
										};
			}

			lcontext.Lexer.Next();

			return t;
		}
	}
}
