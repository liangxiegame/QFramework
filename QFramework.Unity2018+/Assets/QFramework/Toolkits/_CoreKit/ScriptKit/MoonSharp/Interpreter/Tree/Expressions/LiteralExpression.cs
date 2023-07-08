using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	class LiteralExpression : Expression
	{
		DynValue m_Value;

		public DynValue Value
		{
			get { return m_Value; }
		}


		public LiteralExpression(ScriptLoadingContext lcontext, DynValue value)
			: base(lcontext)
		{
			m_Value = value;
		}


		public LiteralExpression(ScriptLoadingContext lcontext, Token t)
			: base(lcontext)
		{
			switch (t.Type)
			{
				case TokenType.Number:
				case TokenType.Number_Hex:
				case TokenType.Number_HexFloat:
					m_Value = DynValue.NewNumber(t.GetNumberValue()).AsReadOnly();
					break;
				case TokenType.String:
				case TokenType.String_Long:
					m_Value = DynValue.NewString(t.Text).AsReadOnly();
					break;
				case TokenType.True:
					m_Value = DynValue.True;
					break;
				case TokenType.False:
					m_Value = DynValue.False;
					break;
				case TokenType.Nil:
					m_Value = DynValue.Nil;
					break;
				default:
					throw new InternalErrorException("type mismatch");
			}

			if (m_Value == null)
				throw new SyntaxErrorException(t, "unknown literal format near '{0}'", t.Text);

			lcontext.Lexer.Next();
		}

		public override void Compile(Execution.VM.ByteCode bc)
		{
			bc.Emit_Literal(m_Value);
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			return m_Value;
		}
	}
}
