using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	class SymbolRefExpression : Expression, IVariable
	{
		SymbolRef m_Ref;
		string m_VarName;

		public SymbolRefExpression(Token T, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			m_VarName = T.Text;

			if (T.Type == TokenType.VarArgs)
			{
				m_Ref = lcontext.Scope.Find(WellKnownSymbols.VARARGS);

				if (!lcontext.Scope.CurrentFunctionHasVarArgs())
					throw new SyntaxErrorException(T, "cannot use '...' outside a vararg function");

				if (lcontext.IsDynamicExpression)
					throw new DynamicExpressionException("cannot use '...' in a dynamic expression.");
			}
			else
			{
				if (!lcontext.IsDynamicExpression)
					m_Ref = lcontext.Scope.Find(m_VarName);
			}

			lcontext.Lexer.Next();
		}

		public SymbolRefExpression(ScriptLoadingContext lcontext, SymbolRef refr)
			: base(lcontext)
		{
			m_Ref = refr;

			if (lcontext.IsDynamicExpression)
			{
				throw new DynamicExpressionException("Unsupported symbol reference expression detected.");
			}
		}

		public override void Compile(Execution.VM.ByteCode bc)
		{
			bc.Emit_Load(m_Ref);
		}


		public void CompileAssignment(Execution.VM.ByteCode bc, int stackofs, int tupleidx)
		{
			bc.Emit_Store(m_Ref, stackofs, tupleidx);
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			return context.EvaluateSymbolByName(m_VarName);
		}

		public override SymbolRef FindDynamic(ScriptExecutionContext context)
		{
			return context.FindSymbolByName(m_VarName);
		}
	}
}
