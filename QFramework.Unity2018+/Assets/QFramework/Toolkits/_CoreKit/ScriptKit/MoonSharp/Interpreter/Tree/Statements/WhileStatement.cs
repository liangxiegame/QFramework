using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;


namespace MoonSharp.Interpreter.Tree.Statements
{
	class WhileStatement : Statement
	{
		Expression m_Condition;
		Statement m_Block;
		RuntimeScopeBlock m_StackFrame;
		SourceRef m_Start, m_End;

		public WhileStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			Token whileTk = CheckTokenType(lcontext, TokenType.While);

			m_Condition = Expression.Expr(lcontext);

			m_Start = whileTk.GetSourceRefUpTo(lcontext.Lexer.Current);

			//m_Start = BuildSourceRef(context.Start, exp.Stop);
			//m_End = BuildSourceRef(context.Stop, context.END());

			lcontext.Scope.PushBlock();
			CheckTokenType(lcontext, TokenType.Do);
			m_Block = new CompositeStatement(lcontext);
			m_End = CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = lcontext.Scope.PopBlock();

			lcontext.Source.Refs.Add(m_Start);
			lcontext.Source.Refs.Add(m_End);
		}


		public override void Compile(ByteCode bc)
		{
			Loop L = new Loop()
			{
				Scope = m_StackFrame
			};


			bc.LoopTracker.Loops.Push(L);

			bc.PushSourceRef(m_Start);

			int start = bc.GetJumpPointForNextInstruction();

			m_Condition.Compile(bc);
			var jumpend = bc.Emit_Jump(OpCode.Jf, -1);

			bc.Emit_Enter(m_StackFrame);

			m_Block.Compile(bc);

			bc.PopSourceRef();
			bc.Emit_Debug("..end");
			bc.PushSourceRef(m_End);
	
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Jump(OpCode.Jump, start);
			
			bc.LoopTracker.Loops.Pop();

			int exitpoint = bc.GetJumpPointForNextInstruction();

			foreach (Instruction i in L.BreakJumps)
				i.NumVal = exitpoint;

			jumpend.NumVal = exitpoint;

			bc.PopSourceRef();
		}

	}
}
