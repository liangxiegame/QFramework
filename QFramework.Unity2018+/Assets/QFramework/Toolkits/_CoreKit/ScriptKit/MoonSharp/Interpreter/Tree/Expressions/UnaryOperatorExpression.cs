using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	class UnaryOperatorExpression : Expression
	{
		Expression m_Exp;
		string m_OpText;

		public UnaryOperatorExpression(ScriptLoadingContext lcontext, Expression subExpression, Token unaryOpToken)
			: base(lcontext)
		{
			m_OpText = unaryOpToken.Text;
			m_Exp = subExpression;
		}



		public override void Compile(ByteCode bc)
		{
			m_Exp.Compile(bc);

			switch (m_OpText)
			{
				case "not":
					bc.Emit_Operator(OpCode.Not);
					break;
				case "#":
					bc.Emit_Operator(OpCode.Len);
					break;
				case "-":
					bc.Emit_Operator(OpCode.Neg);
					break;
				default:
					throw new InternalErrorException("Unexpected unary operator '{0}'", m_OpText);
			}


		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			DynValue v = m_Exp.Eval(context).ToScalar();

			switch (m_OpText)
			{
				case "not":
					return DynValue.NewBoolean(!v.CastToBool());
				case "#":
					return v.GetLength();
				case "-":
					{
						double? d = v.CastToNumber();

						if (d.HasValue)
							return DynValue.NewNumber(-d.Value);

						throw new DynamicExpressionException("Attempt to perform arithmetic on non-numbers.");
					}
				default:
					throw new DynamicExpressionException("Unexpected unary operator '{0}'", m_OpText);
			}
		}
	}
}
