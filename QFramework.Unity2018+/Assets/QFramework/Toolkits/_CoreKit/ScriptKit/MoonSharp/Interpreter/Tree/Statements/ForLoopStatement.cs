using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	class ForLoopStatement : Statement
	{
		//for' NAME '=' exp ',' exp (',' exp)? 'do' block 'end'
		RuntimeScopeBlock m_StackFrame;
		Statement m_InnerBlock;
		SymbolRef m_VarName;
		Expression m_Start, m_End, m_Step;
		SourceRef m_RefFor, m_RefEnd;

		public ForLoopStatement(ScriptLoadingContext lcontext, Token nameToken, Token forToken)
			: base(lcontext)
		{
			//	for Name ‘=’ exp ‘,’ exp [‘,’ exp] do block end | 

			// lexer already at the '=' ! [due to dispatching vs for-each]
			CheckTokenType(lcontext, TokenType.Op_Assignment);

			m_Start = Expression.Expr(lcontext);
			CheckTokenType(lcontext, TokenType.Comma);
			m_End = Expression.Expr(lcontext);

			if (lcontext.Lexer.Current.Type == TokenType.Comma)
			{
				lcontext.Lexer.Next();
				m_Step = Expression.Expr(lcontext);
			}
			else
			{
				m_Step = new LiteralExpression(lcontext, DynValue.NewNumber(1));
			}

			lcontext.Scope.PushBlock();
			m_VarName = lcontext.Scope.DefineLocal(nameToken.Text);
			m_RefFor = forToken.GetSourceRef(CheckTokenType(lcontext, TokenType.Do));
			m_InnerBlock = new CompositeStatement(lcontext);
			m_RefEnd = CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = lcontext.Scope.PopBlock();

			lcontext.Source.Refs.Add(m_RefFor);
			lcontext.Source.Refs.Add(m_RefEnd);
		}		


		public override void Compile(ByteCode bc)
		{
			bc.PushSourceRef(m_RefFor);

			Loop L = new Loop()
			{
				Scope = m_StackFrame
			};

			bc.LoopTracker.Loops.Push(L);

			m_End.Compile(bc);
			bc.Emit_ToNum(3);
			m_Step.Compile(bc);
			bc.Emit_ToNum(2);
			m_Start.Compile(bc);
			bc.Emit_ToNum(1);

			int start = bc.GetJumpPointForNextInstruction();
			var jumpend = bc.Emit_Jump(OpCode.JFor, -1);
			bc.Emit_Enter(m_StackFrame);
			//bc.Emit_SymStorN(m_VarName);

			bc.Emit_Store(m_VarName, 0, 0);

			m_InnerBlock.Compile(bc);

			bc.PopSourceRef();
			bc.PushSourceRef(m_RefEnd);

			bc.Emit_Debug("..end");
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Incr(1);
			bc.Emit_Jump(OpCode.Jump, start);

			bc.LoopTracker.Loops.Pop();

			int exitpoint = bc.GetJumpPointForNextInstruction();

			foreach (Instruction i in L.BreakJumps)
				i.NumVal = exitpoint;

			jumpend.NumVal = exitpoint;
			bc.Emit_Pop(3);

			bc.PopSourceRef();
		}

	}
}
