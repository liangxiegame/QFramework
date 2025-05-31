using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	class FunctionDefinitionExpression : Expression, IClosureBuilder
	{
		SymbolRef[] m_ParamNames = null;
		Statement m_Statement;
		RuntimeScopeFrame m_StackFrame;
		List<SymbolRef> m_Closure = new List<SymbolRef>();
		bool m_HasVarArgs = false;
		Instruction m_ClosureInstruction = null;

		bool m_UsesGlobalEnv;
		SymbolRef m_Env;

		SourceRef m_Begin, m_End;


		public FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool usesGlobalEnv)
			: this(lcontext, false, usesGlobalEnv, false)
		{ }

		public FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool pushSelfParam, bool isLambda)
			: this(lcontext, pushSelfParam, false, isLambda)
		{ }


		private FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool pushSelfParam, bool usesGlobalEnv, bool isLambda)
			: base(lcontext)
		{
			if (m_UsesGlobalEnv = usesGlobalEnv)
				CheckTokenType(lcontext, TokenType.Function);

			// here lexer should be at the '(' or at the '|'
			Token openRound = CheckTokenType(lcontext, isLambda ? TokenType.Lambda : TokenType.Brk_Open_Round);

			List<string> paramnames = BuildParamList(lcontext, pushSelfParam, openRound, isLambda);
			// here lexer is at first token of body

			m_Begin = openRound.GetSourceRefUpTo(lcontext.Lexer.Current);

			// create scope
			lcontext.Scope.PushFunction(this, m_HasVarArgs);

			if (m_UsesGlobalEnv)
			{
				m_Env = lcontext.Scope.DefineLocal(WellKnownSymbols.ENV);
			}
			else
			{
				lcontext.Scope.ForceEnvUpValue();
			}

			m_ParamNames = DefineArguments(paramnames, lcontext);

			if(isLambda)
				m_Statement = CreateLambdaBody(lcontext);
			else
				m_Statement = CreateBody(lcontext);

			m_StackFrame = lcontext.Scope.PopFunction();

			lcontext.Source.Refs.Add(m_Begin);
			lcontext.Source.Refs.Add(m_End);

		}


		private Statement CreateLambdaBody(ScriptLoadingContext lcontext)
		{
			Token start = lcontext.Lexer.Current;
			Expression e = Expression.Expr(lcontext);
			Token end = lcontext.Lexer.Current;
			SourceRef sref = start.GetSourceRefUpTo(end);
			Statement s = new ReturnStatement(lcontext, e, sref);
			return s;
		}


		private Statement CreateBody(ScriptLoadingContext lcontext)
		{
			Statement s = new CompositeStatement(lcontext);

			if (lcontext.Lexer.Current.Type != TokenType.End)
				throw new SyntaxErrorException(lcontext.Lexer.Current, "'end' expected near '{0}'", lcontext.Lexer.Current.Text)
				{
					IsPrematureStreamTermination = (lcontext.Lexer.Current.Type == TokenType.Eof)
				};

			m_End = lcontext.Lexer.Current.GetSourceRef();

			lcontext.Lexer.Next();
			return s;
		}

		private List<string> BuildParamList(ScriptLoadingContext lcontext, bool pushSelfParam, Token openBracketToken, bool isLambda)
		{
			TokenType closeToken = isLambda ? TokenType.Lambda : TokenType.Brk_Close_Round;

			List<string> paramnames = new List<string>();

			// method decls with ':' must push an implicit 'self' param
			if (pushSelfParam)
				paramnames.Add("self");

			while (lcontext.Lexer.Current.Type != closeToken)
			{
				Token t = lcontext.Lexer.Current;

				if (t.Type == TokenType.Name)
				{
					paramnames.Add(t.Text);
				}
				else if (t.Type == TokenType.VarArgs)
				{
					m_HasVarArgs = true;
					paramnames.Add(WellKnownSymbols.VARARGS);
				}
				else
					UnexpectedTokenType(t);

				lcontext.Lexer.Next();

				t = lcontext.Lexer.Current;

				if (t.Type == TokenType.Comma)
				{
					lcontext.Lexer.Next();
				}
				else
				{
					CheckMatch(lcontext, openBracketToken, closeToken, isLambda ? "|" : ")");
					break;
				}
			}

			if (lcontext.Lexer.Current.Type == closeToken)
				lcontext.Lexer.Next();

			return paramnames;
		}

		private SymbolRef[] DefineArguments(List<string> paramnames, ScriptLoadingContext lcontext)
		{
			HashSet<string> names = new HashSet<string>();

			SymbolRef[] ret = new SymbolRef[paramnames.Count];

			for (int i = paramnames.Count - 1; i >= 0; i--)
			{
				if (!names.Add(paramnames[i]))
					paramnames[i] = paramnames[i] + "@" + i.ToString();

				ret[i] = lcontext.Scope.DefineLocal(paramnames[i]);
			}

			return ret;
		}

		public SymbolRef CreateUpvalue(BuildTimeScope scope, SymbolRef symbol)
		{
			for (int i = 0; i < m_Closure.Count; i++)
			{
				if (m_Closure[i].i_Name == symbol.i_Name)
				{
					return SymbolRef.Upvalue(symbol.i_Name, i);
				}
			}

			m_Closure.Add(symbol);

			if (m_ClosureInstruction != null)
			{
				m_ClosureInstruction.SymbolList = m_Closure.ToArray();
			}

			return SymbolRef.Upvalue(symbol.i_Name, m_Closure.Count - 1);
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			throw new DynamicExpressionException("Dynamic Expressions cannot define new functions.");
		}

		public int CompileBody(ByteCode bc, string friendlyName)
		{
			string funcName = friendlyName ?? ("<" + this.m_Begin.FormatLocation(bc.Script, true) + ">");

			bc.PushSourceRef(m_Begin);

			Instruction I = bc.Emit_Jump(OpCode.Jump, -1);

			Instruction meta = bc.Emit_Meta(funcName, OpCodeMetadataType.FunctionEntrypoint);
			int metaip = bc.GetJumpPointForLastInstruction();

			bc.Emit_BeginFn(m_StackFrame);

			bc.LoopTracker.Loops.Push(new LoopBoundary());

			int entryPoint = bc.GetJumpPointForLastInstruction();

			if (m_UsesGlobalEnv)
			{
				bc.Emit_Load(SymbolRef.Upvalue(WellKnownSymbols.ENV, 0));
				bc.Emit_Store(m_Env, 0, 0);
				bc.Emit_Pop();
			}

			if (m_ParamNames.Length > 0)
				bc.Emit_Args(m_ParamNames);

			m_Statement.Compile(bc);

			bc.PopSourceRef();
			bc.PushSourceRef(m_End);

			bc.Emit_Ret(0);

			bc.LoopTracker.Loops.Pop();

			I.NumVal = bc.GetJumpPointForNextInstruction();
			meta.NumVal = bc.GetJumpPointForLastInstruction() - metaip;

			bc.PopSourceRef();

			return entryPoint;
		}

		public int Compile(ByteCode bc, Func<int> afterDecl, string friendlyName)
		{
			using (bc.EnterSource(m_Begin))
			{
				SymbolRef[] symbs = m_Closure
					//.Select((s, idx) => s.CloneLocalAndSetFrame(m_ClosureFrames[idx]))
					.ToArray();

				m_ClosureInstruction = bc.Emit_Closure(symbs, bc.GetJumpPointForNextInstruction());
				int ops = afterDecl();

				m_ClosureInstruction.NumVal += 2 + ops;
			}

			return CompileBody(bc, friendlyName);
		}


		public override void Compile(ByteCode bc)
		{
			Compile(bc, () => 0, null);
		}
	}
}
