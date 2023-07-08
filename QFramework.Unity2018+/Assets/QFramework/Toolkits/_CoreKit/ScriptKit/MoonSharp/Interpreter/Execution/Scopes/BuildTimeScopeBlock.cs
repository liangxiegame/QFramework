using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Execution.Scopes
{
	internal class BuildTimeScopeBlock
	{
		internal BuildTimeScopeBlock Parent { get; private set; }
		internal List<BuildTimeScopeBlock> ChildNodes { get; private set; }

		internal RuntimeScopeBlock ScopeBlock { get; private set; }

		Dictionary<string, SymbolRef> m_DefinedNames = new Dictionary<string, SymbolRef>();



		internal void Rename(string name)
		{
			SymbolRef sref = m_DefinedNames[name];
			m_DefinedNames.Remove(name);
			m_DefinedNames.Add(string.Format("@{0}_{1}", name, Guid.NewGuid().ToString("N")), sref);
		}

		internal BuildTimeScopeBlock(BuildTimeScopeBlock parent)
		{
			Parent = parent;
			ChildNodes = new List<BuildTimeScopeBlock>();
			ScopeBlock = new RuntimeScopeBlock();
		}


		internal BuildTimeScopeBlock AddChild()
		{
			BuildTimeScopeBlock block = new BuildTimeScopeBlock(this);
			ChildNodes.Add(block);
			return block;
		}

		internal SymbolRef Find(string name)
		{
			return m_DefinedNames.GetOrDefault(name);
		}

		internal SymbolRef Define(string name)
		{
			SymbolRef l = SymbolRef.Local(name, -1);
			m_DefinedNames.Add(name, l);
			m_LastDefinedName = name;
			return l;
		}

		internal int ResolveLRefs(BuildTimeScopeFrame buildTimeScopeFrame)
		{
			int firstVal = -1;
			int lastVal = -1;

			foreach (SymbolRef lref in m_DefinedNames.Values)
			{
				int pos = buildTimeScopeFrame.AllocVar(lref);

				if (firstVal < 0)
					firstVal = pos;

				lastVal = pos;
			}

			this.ScopeBlock.From = firstVal;
			this.ScopeBlock.ToInclusive = this.ScopeBlock.To = lastVal;

			if (firstVal < 0)
				this.ScopeBlock.From = buildTimeScopeFrame.GetPosForNextVar();

			foreach (var child in ChildNodes)
			{
				this.ScopeBlock.ToInclusive = Math.Max(this.ScopeBlock.ToInclusive, child.ResolveLRefs(buildTimeScopeFrame));
			}

			if (m_LocalLabels != null)
				foreach (var label in m_LocalLabels.Values)
					label.SetScope(this.ScopeBlock);

			return this.ScopeBlock.ToInclusive;
		}


		List<GotoStatement> m_PendingGotos;
		Dictionary<string, LabelStatement> m_LocalLabels;
		string m_LastDefinedName;

		internal void DefineLabel(LabelStatement label)
		{
			if (m_LocalLabels == null)
				m_LocalLabels = new Dictionary<string, LabelStatement>();

			if (m_LocalLabels.ContainsKey(label.Label))
			{
				throw new SyntaxErrorException(label.NameToken, "label '{0}' already defined on line {1}", label.Label, m_LocalLabels[label.Label].SourceRef.FromLine);
			}
			else
			{
				m_LocalLabels.Add(label.Label, label);
				label.SetDefinedVars(m_DefinedNames.Count, m_LastDefinedName);
			}
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			if (m_PendingGotos == null)
				m_PendingGotos = new List<GotoStatement>();

			m_PendingGotos.Add(gotostat);
			gotostat.SetDefinedVars(m_DefinedNames.Count, m_LastDefinedName);
		}

		internal void ResolveGotos()
		{
			if (m_PendingGotos == null)
				return;

			foreach (GotoStatement gotostat in m_PendingGotos)
			{
				LabelStatement label;

				if (m_LocalLabels != null && m_LocalLabels.TryGetValue(gotostat.Label, out label))
				{
					if (label.DefinedVarsCount > gotostat.DefinedVarsCount)
						throw new SyntaxErrorException(gotostat.GotoToken,
							"<goto {0}> at line {1} jumps into the scope of local '{2}'", gotostat.Label, 
							gotostat.GotoToken.FromLine,
							label.LastDefinedVarName);

					label.RegisterGoto(gotostat);
				}
				else
				{
					if (Parent == null)
						throw new SyntaxErrorException(gotostat.GotoToken, "no visible label '{0}' for <goto> at line {1}", gotostat.Label,
							gotostat.GotoToken.FromLine);

					Parent.RegisterGoto(gotostat);
				}
			}

			m_PendingGotos.Clear();
		}
	}
}
