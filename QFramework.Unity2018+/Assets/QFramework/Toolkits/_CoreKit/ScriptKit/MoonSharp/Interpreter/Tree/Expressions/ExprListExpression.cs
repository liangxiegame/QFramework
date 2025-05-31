using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	class ExprListExpression : Expression 
	{
		List<Expression> expressions;

		public ExprListExpression(List<Expression> exps, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			expressions = exps;
		}


		public Expression[] GetExpressions()
		{
			return expressions.ToArray();
		}

		public override void Compile(Execution.VM.ByteCode bc)
		{
			foreach (var exp in expressions)
				exp.Compile(bc);

			if (expressions.Count > 1)
				bc.Emit_MkTuple(expressions.Count);
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			if (expressions.Count >= 1)
				return expressions[0].Eval(context);

			return DynValue.Void;
		}
	}
}
