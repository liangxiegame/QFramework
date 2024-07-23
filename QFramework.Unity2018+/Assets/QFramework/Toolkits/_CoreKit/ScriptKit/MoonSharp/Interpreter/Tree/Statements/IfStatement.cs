using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;


namespace MoonSharp.Interpreter.Tree.Statements
{
	class IfStatement : Statement
	{
		private class IfBlock
		{
			public Expression Exp;
			public Statement Block;
			public RuntimeScopeBlock StackFrame;
			public SourceRef Source;
		}

		List<IfBlock> m_Ifs = new List<IfBlock>();
		IfBlock m_Else = null;
		SourceRef m_End;

		public IfStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			while (lcontext.Lexer.Current.Type != TokenType.Else && lcontext.Lexer.Current.Type != TokenType.End)
			{
				m_Ifs.Add(CreateIfBlock(lcontext));
			}

			if (lcontext.Lexer.Current.Type == TokenType.Else)
			{
				m_Else = CreateElseBlock(lcontext);
			}

			m_End = CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			lcontext.Source.Refs.Add(m_End);
		}

		IfBlock CreateIfBlock(ScriptLoadingContext lcontext)
		{
			Token type = CheckTokenType(lcontext, TokenType.If, TokenType.ElseIf);

			lcontext.Scope.PushBlock();

			var ifblock = new IfBlock();

			ifblock.Exp = Expression.Expr(lcontext);
			ifblock.Source = type.GetSourceRef(CheckTokenType(lcontext, TokenType.Then));
			ifblock.Block = new CompositeStatement(lcontext);
			ifblock.StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(ifblock.Source);


			return ifblock;
		}

		IfBlock CreateElseBlock(ScriptLoadingContext lcontext)
		{
			Token type = CheckTokenType(lcontext, TokenType.Else);

			lcontext.Scope.PushBlock();

			var ifblock = new IfBlock();
			ifblock.Block = new CompositeStatement(lcontext);
			ifblock.StackFrame = lcontext.Scope.PopBlock();
			ifblock.Source = type.GetSourceRef();
			lcontext.Source.Refs.Add(ifblock.Source);
			return ifblock;
		}


		public override void Compile(Execution.VM.ByteCode bc)
		{
			List<Instruction> endJumps = new List<Instruction>();

			Instruction lastIfJmp = null;

			foreach (var ifblock in m_Ifs)
			{
				using (bc.EnterSource(ifblock.Source))
				{
					if (lastIfJmp != null)
						lastIfJmp.NumVal = bc.GetJumpPointForNextInstruction();

					ifblock.Exp.Compile(bc);
					lastIfJmp = bc.Emit_Jump(OpCode.Jf, -1);
					bc.Emit_Enter(ifblock.StackFrame);
					ifblock.Block.Compile(bc);
				}

				using (bc.EnterSource(m_End))
					bc.Emit_Leave(ifblock.StackFrame);

				endJumps.Add(bc.Emit_Jump(OpCode.Jump, -1));
			}

			lastIfJmp.NumVal = bc.GetJumpPointForNextInstruction();

			if (m_Else != null)
			{
				using (bc.EnterSource(m_Else.Source))
				{
					bc.Emit_Enter(m_Else.StackFrame);
					m_Else.Block.Compile(bc);
				}

				using (bc.EnterSource(m_End))
					bc.Emit_Leave(m_Else.StackFrame);
			}

			foreach(var endjmp in endJumps)
				endjmp.NumVal = bc.GetJumpPointForNextInstruction();
		}



	}
}
