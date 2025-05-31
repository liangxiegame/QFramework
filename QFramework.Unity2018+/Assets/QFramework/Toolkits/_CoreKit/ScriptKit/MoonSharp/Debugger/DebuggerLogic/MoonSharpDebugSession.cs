#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.VsCodeDebugger.SDK;


namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
	internal class MoonSharpDebugSession : DebugSession, IAsyncDebuggerClient
	{
		AsyncDebugger m_Debug;
		MoonSharpVsCodeDebugServer m_Server;
		List<DynValue> m_Variables = new List<DynValue>();
		bool m_NotifyExecutionEnd = false;

		const int SCOPE_LOCALS = 65536;
		const int SCOPE_SELF = 65537;


		internal MoonSharpDebugSession(MoonSharpVsCodeDebugServer server, AsyncDebugger debugger)
			: base(true, false)
		{
			m_Server = server;
			m_Debug = debugger;
		}

		public override void Initialize(Response response, Table args)
		{
#if DOTNET_CORE
			SendText("Connected to MoonSharp {0} [{1}]",
					 Script.VERSION,
					 Script.GlobalOptions.Platform.GetPlatformName());
#else
			SendText("Connected to MoonSharp {0} [{1}] on process {2} (PID {3})",
					 Script.VERSION,
					 Script.GlobalOptions.Platform.GetPlatformName(),
					 System.Diagnostics.Process.GetCurrentProcess().ProcessName,
					 System.Diagnostics.Process.GetCurrentProcess().Id);
#endif

			SendText("Debugging script '{0}'; use the debug console to debug another script.", m_Debug.Name);

			SendText("Type '!help' in the Debug Console for available commands.");

			SendResponse(response, new Capabilities()
			{
				// This debug adapter does not need the configurationDoneRequest.
				supportsConfigurationDoneRequest = false,

				// This debug adapter does not support function breakpoints.
				supportsFunctionBreakpoints = false,

				// This debug adapter doesn't support conditional breakpoints.
				supportsConditionalBreakpoints = false,

				// This debug adapter does not support a side effect free evaluate request for data hovers.
				supportsEvaluateForHovers = false,

				// This debug adapter does not support exception breakpoint filters
				exceptionBreakpointFilters = new object[0]
			});

			// Debugger is ready to accept breakpoints immediately
			SendEvent(new InitializedEvent());

			m_Debug.Client = this;
		}

		public override void Attach(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Continue(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction() { Action = DebuggerAction.ActionType.Run });
			SendResponse(response);
		}

		public override void Disconnect(Response response, Table arguments)
		{
			m_Debug.Client = null;
			SendResponse(response);
		}

		private static string getString(Table args, string property, string dflt = null)
		{
			var s = (string)args[property];
			if (s == null)
			{
				return dflt;
			}
			s = s.Trim();
			if (s.Length == 0)
			{
				return dflt;
			}
			return s;
		}

		public override void Evaluate(Response response, Table args)
		{
			var expression = getString(args, "expression");
			var frameId = getInt(args, "frameId", 0);
			var context = getString(args, "context") ?? "hover";

			if (frameId != 0 && context != "repl")
				SendText("Warning : Evaluation of variables/watches is always done with the top-level scope.");

			if (context == "repl" && expression.StartsWith("!"))
			{
				ExecuteRepl(expression.Substring(1));
				SendResponse(response);
				return;
			}

			DynValue v = m_Debug.Evaluate(expression) ?? DynValue.Nil;
			m_Variables.Add(v);

			SendResponse(response, new EvaluateResponseBody(v.ToDebugPrintString(), m_Variables.Count - 1)
			{
				type = v.Type.ToLuaDebuggerString()
			});
		}

		private void ExecuteRepl(string cmd)
		{
			bool showHelp = false;
			cmd = cmd.Trim();
			if (cmd == "help")
			{
				showHelp = true;
			}
			else if (cmd.StartsWith("geterror"))
			{
				SendText("Current error regex : {0}", m_Debug.ErrorRegex.ToString());
			}
			else if (cmd.StartsWith("seterror"))
			{
				string regex = cmd.Substring("seterror".Length).Trim();

				try
				{
					Regex rx = new Regex(regex);
					m_Debug.ErrorRegex = rx;
					SendText("Current error regex : {0}", m_Debug.ErrorRegex.ToString());
				}
				catch (Exception ex)
				{
					SendText("Error setting regex: {0}", ex.Message);
				}
			}
			else if (cmd.StartsWith("execendnotify"))
			{
				string val = cmd.Substring("execendnotify".Length).Trim();

				if (val == "off")
				{
					m_NotifyExecutionEnd = false;
				}
				else if (val == "on")
				{
					m_NotifyExecutionEnd = true;
				}
				else if (val.Length > 0)
					SendText("Error : expected 'on' or 'off'");

				SendText("Notifications of execution end are : {0}", m_NotifyExecutionEnd ? "enabled" : "disabled");
			}
			else if (cmd == "list")
			{
				int currId = m_Server.CurrentId ?? -1000;

				foreach (var pair in m_Server.GetAttachedDebuggersByIdAndName())
				{
					string isthis = (pair.Key == m_Debug.Id) ? " (this)" : "";
					string isdef = (pair.Key == currId) ? " (default)" : "";

					SendText("{0} : {1}{2}{3}", pair.Key.ToString().PadLeft(9), pair.Value, isdef, isthis);
				}
			}
			else if (cmd.StartsWith("select") || cmd.StartsWith("switch"))
			{
				string arg = cmd.Substring("switch".Length).Trim();

				try
				{
					int id = int.Parse(arg);
					m_Server.CurrentId = id;

					if (cmd.StartsWith("switch"))
						Unbind();
					else
						SendText("Next time you'll attach the debugger, it will be atteched to script #{0}", id);
				}
				catch (Exception ex)
				{
					SendText("Error setting regex: {0}", ex.Message);
				}
			}
			else
			{
				SendText("Syntax error : {0}\n", cmd);
				showHelp = true;
			}

			if (showHelp)
			{
				SendText("Available commands : ");
				SendText("    !help - gets this help");
				SendText("    !list - lists the other scripts which can be debugged");
				SendText("    !select <id> - select another script for future sessions");
				SendText("    !switch <id> - switch to another script (same as select + disconnect)");
				SendText("    !seterror <regex> - sets the regex which tells which errors to trap");
				SendText("    !geterror - gets the current value of the regex which tells which errors to trap");
				SendText("    !execendnotify [on|off] - sets the notification of end of execution on or off (default = off)");
				SendText("    ... or type an expression to evaluate it on the fly.");
			}
		}


		public override void Launch(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Next(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction() { Action = DebuggerAction.ActionType.StepOver });
			SendResponse(response);
		}

		private StoppedEvent CreateStoppedEvent(string reason, string text = null)
		{
			return new StoppedEvent(0, reason, text);
		}

		public override void Pause(Response response, Table arguments)
		{
			m_Debug.PauseRequested = true;
			SendResponse(response);
			SendText("Pause pending -- will pause at first script statement.");
		}

		public override void Scopes(Response response, Table arguments)
		{
			var scopes = new List<Scope>();

			scopes.Add(new Scope("Locals", SCOPE_LOCALS));
			scopes.Add(new Scope("Self", SCOPE_SELF));

			SendResponse(response, new ScopesResponseBody(scopes));
		}

		public override void SetBreakpoints(Response response, Table args)
		{
			string path = null;

			Table args_source = args["source"] as Table;

			if (args_source != null)
			{
				string p = args_source["path"].ToString();
				if (p != null && p.Trim().Length > 0)
					path = p;
			}

			if (path == null)
			{
				SendErrorResponse(response, 3010, "setBreakpoints: property 'source' is empty or misformed", null, false, true);
				return;
			}

			path = ConvertClientPathToDebugger(path);

			SourceCode src = m_Debug.FindSourceByName(path);

			if (src == null)
			{
				// we only support breakpoints in files mono can handle
				SendResponse(response, new SetBreakpointsResponseBody());
				return;
			}

			Table clientLines = args.Get("lines").Table;

			var lin = new HashSet<int>(clientLines.Values.Select(jt => ConvertClientLineToDebugger(jt.ToObject<int>())).ToArray());

			var lin2 = m_Debug.DebugService.ResetBreakPoints(src, lin);

			var breakpoints = new List<Breakpoint>();
			foreach (var l in lin)
			{
				breakpoints.Add(new Breakpoint(lin2.Contains(l), l));
			}

			response.SetBody(new SetBreakpointsResponseBody(breakpoints)); SendResponse(response);
		}

		public override void StackTrace(Response response, Table args)
		{
			int maxLevels = getInt(args, "levels", 10);
			//int threadReference = getInt(args, "threadId", 0);

			var stackFrames = new List<StackFrame>();

			var stack = m_Debug.GetWatches(WatchType.CallStack);

			var coroutine = m_Debug.GetWatches(WatchType.Threads).LastOrDefault();

			int level = 0;
			int max = Math.Min(maxLevels - 3, stack.Count);

			while (level < max)
			{
				WatchItem frame = stack[level];

				string name = frame.Name;
				SourceRef sourceRef = frame.Location ?? DefaultSourceRef;
				int sourceIdx = sourceRef.SourceIdx;
				string path = sourceRef.IsClrLocation ? "(native)" : (m_Debug.GetSourceFile(sourceIdx) ?? "???");
				string sourceName = Path.GetFileName(path);

				var source = new Source(sourceName, path); // ConvertDebuggerPathToClient(path));

				stackFrames.Add(new StackFrame(level, name, source,
					ConvertDebuggerLineToClient(sourceRef.FromLine), sourceRef.FromChar,
					ConvertDebuggerLineToClient(sourceRef.ToLine), sourceRef.ToChar));

				level++;
			}

			if (stack.Count > maxLevels - 3)
				stackFrames.Add(new StackFrame(level++, "(...)", null, 0));

			if (coroutine != null)
				stackFrames.Add(new StackFrame(level++, "(" + coroutine.Name + ")", null, 0));
			else
				stackFrames.Add(new StackFrame(level++, "(main coroutine)", null, 0));

			stackFrames.Add(new StackFrame(level++, "(native)", null, 0));

			SendResponse(response, new StackTraceResponseBody(stackFrames));
		}

		readonly SourceRef DefaultSourceRef = new SourceRef(-1, 0, 0, 0, 0, false);


		private int getInt(Table args, string propName, int defaultValue)
		{
			var jo = args.Get(propName);

			if (jo.Type != DataType.Number)
				return defaultValue;
			else
				return jo.ToObject<int>();
		}


		public override void StepIn(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction() { Action = DebuggerAction.ActionType.StepIn });
			SendResponse(response);
		}

		public override void StepOut(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction() { Action = DebuggerAction.ActionType.StepOut });
			SendResponse(response);
		}

		public override void Threads(Response response, Table arguments)
		{
			var threads = new List<Thread>() { new Thread(0, "Main Thread") };
			SendResponse(response, new ThreadsResponseBody(threads));
		}


		public override void Variables(Response response, Table arguments)
		{
			int index = getInt(arguments, "variablesReference", -1);

			var variables = new List<Variable>();

			if (index == SCOPE_SELF)
			{
				DynValue v = m_Debug.Evaluate("self");
				VariableInspector.InspectVariable(v, variables);
			}
			else if (index == SCOPE_LOCALS)
			{
				foreach (var w in m_Debug.GetWatches(WatchType.Locals))
					variables.Add(new Variable(w.Name, (w.Value ?? DynValue.Void).ToDebugPrintString()));
			}
			else if (index < 0 || index >= m_Variables.Count)
			{
				variables.Add(new Variable("<error>", null));
			}
			else
			{
				VariableInspector.InspectVariable(m_Variables[index], variables);
			}

			SendResponse(response, new VariablesResponseBody(variables));
		}

		void IAsyncDebuggerClient.SendStopEvent()
		{
			SendEvent(CreateStoppedEvent("step"));
		}


		void IAsyncDebuggerClient.OnWatchesUpdated(WatchType watchType)
		{
			if (watchType == WatchType.CallStack)
				m_Variables.Clear();
		}

		void IAsyncDebuggerClient.OnSourceCodeChanged(int sourceID)
		{
			if (m_Debug.IsSourceOverride(sourceID))
				SendText("Loaded source '{0}' -> '{1}'", m_Debug.GetSource(sourceID).Name, m_Debug.GetSourceFile(sourceID));
			else
				SendText("Loaded source '{0}'", m_Debug.GetSource(sourceID).Name);
		}

		public void OnExecutionEnded()
		{
			if (m_NotifyExecutionEnd)
				SendText("Execution ended.");
		}

		private void SendText(string msg, params object[] args)
		{
			msg = string.Format(msg, args);
			// SendEvent(new OutputEvent("console", DateTime.Now.ToString("u") + ": " + msg + "\n"));
			SendEvent(new OutputEvent("console", msg + "\n"));
		}

		public void OnException(ScriptRuntimeException ex)
		{
			SendText("runtime error : {0}", ex.DecoratedMessage);
		}

		public void Unbind()
		{
			SendText("Debug session has been closed by the hosting process.");
			SendText("Bye.");
			SendEvent(new TerminatedEvent());
		}
	}
}
#endif