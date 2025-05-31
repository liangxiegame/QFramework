#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
	internal interface IAsyncDebuggerClient
	{
		void SendStopEvent();
		void OnWatchesUpdated(WatchType watchType);
		void OnSourceCodeChanged(int sourceID);
		void OnExecutionEnded();
		void OnException(ScriptRuntimeException ex);
		void Unbind();
	}
}

#endif