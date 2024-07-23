using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Execution.Scopes
{
	internal class BuildTimeScopeFrame
	{
		BuildTimeScopeBlock m_ScopeTreeRoot;
		BuildTimeScopeBlock m_ScopeTreeHead;
		RuntimeScopeFrame m_ScopeFrame = new RuntimeScopeFrame();

		public bool HasVarArgs { get; private set;}

		internal BuildTimeScopeFrame(bool hasVarArgs)
		{
			HasVarArgs = hasVarArgs;
			m_ScopeTreeHead = m_ScopeTreeRoot = new BuildTimeScopeBlock(null);
		}

		internal void PushBlock()
		{
			m_ScopeTreeHead = m_ScopeTreeHead.AddChild();
		}

		internal RuntimeScopeBlock PopBlock()
		{
			var tree = m_ScopeTreeHead;

			m_ScopeTreeHead.ResolveGotos();

			m_ScopeTreeHead = m_ScopeTreeHead.Parent;

			if (m_ScopeTreeHead == null)
				throw new InternalErrorException("Can't pop block - stack underflow");

			return tree.ScopeBlock;
		}

		internal RuntimeScopeFrame GetRuntimeFrameData()
		{
			if (m_ScopeTreeHead != m_ScopeTreeRoot)
				throw new InternalErrorException("Misaligned scope frames/blocks!");

			m_ScopeFrame.ToFirstBlock = m_ScopeTreeRoot.ScopeBlock.To;

			return m_ScopeFrame;
		}

		internal SymbolRef Find(string name)
		{
			for (var tree = m_ScopeTreeHead; tree != null; tree = tree.Parent)
			{
				SymbolRef l = tree.Find(name);

				if (l != null)
					return l;
			}

			return null;
		}

		internal SymbolRef DefineLocal(string name)
		{
			return m_ScopeTreeHead.Define(name);
		}

		internal SymbolRef TryDefineLocal(string name)
		{
			if (m_ScopeTreeHead.Find(name) != null)
			{
				m_ScopeTreeHead.Rename(name);
			}

			return m_ScopeTreeHead.Define(name);
		}

		internal void ResolveLRefs()
		{
			m_ScopeTreeRoot.ResolveGotos();

			m_ScopeTreeRoot.ResolveLRefs(this);
		}

		internal int AllocVar(SymbolRef var)
		{
			var.i_Index = m_ScopeFrame.DebugSymbols.Count;
			m_ScopeFrame.DebugSymbols.Add(var);
			return var.i_Index;
		}

		internal int GetPosForNextVar()
		{
			return m_ScopeFrame.DebugSymbols.Count;
		}

		internal void DefineLabel(LabelStatement label)
		{
			m_ScopeTreeHead.DefineLabel(label);
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			m_ScopeTreeHead.RegisterGoto(gotostat);
		}
	}
}
