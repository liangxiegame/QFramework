using MoonSharp.Interpreter.Execution;


namespace MoonSharp.Interpreter.Tree.Expressions
{
	class AdjustmentExpression : Expression 
	{
		private Expression expression;

		public AdjustmentExpression(ScriptLoadingContext lcontext, Expression exp)
			: base(lcontext)
		{
			expression = exp;
		}

		public override void Compile(Execution.VM.ByteCode bc)
		{
			expression.Compile(bc);
			bc.Emit_Scalar();
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			return expression.Eval(context).ToScalar();
		}
	}
}
