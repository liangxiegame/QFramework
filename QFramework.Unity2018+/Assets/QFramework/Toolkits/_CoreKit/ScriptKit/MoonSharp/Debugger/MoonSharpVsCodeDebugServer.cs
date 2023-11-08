#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MoonSharp.VsCodeDebugger.DebuggerLogic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger
{
	/// <summary>
	/// Class implementing a debugger allowing attaching from a Visual Studio Code debugging session.
	/// </summary>
	public class MoonSharpVsCodeDebugServer : IDisposable
	{
		object m_Lock = new object();
		List<AsyncDebugger> m_DebuggerList = new List<AsyncDebugger>();
		AsyncDebugger m_Current = null;
		ManualResetEvent m_StopEvent = new ManualResetEvent(false);
		bool m_Started = false;
		int m_Port;

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpVsCodeDebugServer" /> class.
		/// </summary>
		/// <param name="port">The port on which the debugger listens. It's recommended to use 41912.</param>
		public MoonSharpVsCodeDebugServer(int port = 41912)
		{
			m_Port = port;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpVsCodeDebugServer" /> class with a default script.
		/// Note that for this specific script, it will NOT attach the debugger to the script.
		/// </summary>
		/// <param name="script">The script object to debug.</param>
		/// <param name="port">The port on which the debugger listens. It's recommended to use 41912 unless you are going to keep more than one script object around.</param>
		/// <param name="sourceFinder">A function which gets in input a source code and returns the path to
		/// source file to use. It can return null and in that case (or if the file cannot be found)
		/// a temporary file will be generated on the fly.</param>
		[Obsolete("Use the constructor taking only a port, and the 'Attach' method instead.")]
		public MoonSharpVsCodeDebugServer(Script script, int port, Func<SourceCode, string> sourceFinder = null)
		{
			m_Port = port;
			m_Current = new AsyncDebugger(script, sourceFinder ?? (s => s.Name), "Default script");
			m_DebuggerList.Add(m_Current);
		}

		/// <summary>
		/// Attaches the specified script to the debugger
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="name">The name of the script.</param>
		/// <param name="sourceFinder">A function which gets in input a source code and returns the path to
		/// source file to use. It can return null and in that case (or if the file cannot be found)
		/// a temporary file will be generated on the fly.</param>
		/// <exception cref="ArgumentException">If the script has already been attached to this debugger.</exception>
		public void AttachToScript(Script script, string name, Func<SourceCode, string> sourceFinder = null)
		{
			lock (m_Lock)
			{
				if (m_DebuggerList.Any(d => d.Script == script))
					throw new ArgumentException("Script already attached to this debugger.");

				var debugger = new AsyncDebugger(script, sourceFinder ?? (s => s.Name), name);
				script.AttachDebugger(debugger);
				m_DebuggerList.Add(debugger);

				if (m_Current == null)
					m_Current = debugger;
			}
		}

		/// <summary>
		/// Gets a list of the attached debuggers by id and name
		/// </summary>
		public IEnumerable<KeyValuePair<int, string>> GetAttachedDebuggersByIdAndName()
		{
			lock (m_Lock)
				return m_DebuggerList
					.OrderBy(d => d.Id)
					.Select(d => new KeyValuePair<int, string>(d.Id, d.Name))
					.ToArray();
		}


		/// <summary>
		/// Gets or sets the current script by ID (see GetAttachedDebuggersByIdAndName). 
		/// New vscode connections will attach to this debugger ID. Changing the current ID does NOT disconnect
		/// connected clients.
		/// </summary>
		public int? CurrentId
		{
			get { lock (m_Lock) return m_Current != null ? m_Current.Id : (int?)null; }
			set
			{
				lock (m_Lock)
				{
					if (value == null)
					{
						m_Current = null;
						return;
					}

					var current = (m_DebuggerList.FirstOrDefault(d => d.Id == value));

					if (current == null)
						throw new ArgumentException("Cannot find debugger with given Id.");

					m_Current = current;
				}
			}
		}


		/// <summary>
		/// Gets or sets the current script. New vscode connections will attach to this script. Changing the current script does NOT disconnect
		/// connected clients.
		/// </summary>
		public Script Current
		{
			get { lock(m_Lock) return m_Current != null ? m_Current.Script : null; }
			set
			{
				lock (m_Lock)
				{
					if (value == null)
					{
						m_Current = null;
						return;
					}

					var current = (m_DebuggerList.FirstOrDefault(d => d.Script == value));

					if (current == null)
						throw new ArgumentException("Cannot find debugger with given script associated.");

					m_Current = current;
				}
			}
		}

		/// <summary>
		/// Detaches the specified script. The debugger attached to that script will get disconnected.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <exception cref="ArgumentException">Thrown if the script cannot be found.</exception>
		public void Detach(Script script)
		{
			lock (m_Lock)
			{
				var removed = m_DebuggerList.FirstOrDefault(d => d.Script == script);

				if (removed == null)
					throw new ArgumentException("Cannot detach script - not found.");

				removed.Client = null;

				m_DebuggerList.Remove(removed);

				if (m_Current == removed)
				{
					if (m_DebuggerList.Count > 0)
						m_Current = m_DebuggerList[m_DebuggerList.Count - 1];
					else
						m_Current = null;
				}

			}
		}

		/// <summary>
		/// Gets or sets a delegate which will be called when logging messages are generated
		/// </summary>
		public Action<string> Logger { get; set; }

		
		/// <summary>
		/// Gets the debugger object. Obsolete, use the new interface using the Attach method instead.
		/// </summary>
		[Obsolete("Use the Attach method instead.")]
		public IDebugger GetDebugger()
		{
			lock(m_Lock)
				return m_Current;
		}

		/// <summary>
		/// Stops listening
		/// </summary>
		/// <exception cref="InvalidOperationException">Cannot stop; server was not started.</exception>
		public void Dispose()
		{
			m_StopEvent.Set();
		}

		/// <summary>
		/// Starts listening on the localhost for incoming connections.
		/// </summary>
		public MoonSharpVsCodeDebugServer Start()
		{
			lock (m_Lock)
			{
				if (m_Started)
					throw new InvalidOperationException("Cannot start; server has already been started.");

				m_StopEvent.Reset();

				TcpListener serverSocket = null;
				serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), m_Port);
				serverSocket.Start();

				SpawnThread("VsCodeDebugServer_" + m_Port.ToString(), () => ListenThread(serverSocket));

				m_Started = true;

				return this;
			}
		}


		private void ListenThread(TcpListener serverSocket)
		{
			try
			{
				while (!m_StopEvent.WaitOne(0))
				{
#if DOTNET_CORE
					var task = serverSocket.AcceptSocketAsync();
					task.Wait();
					var clientSocket = task.Result;
#else
					var clientSocket = serverSocket.AcceptSocket();
#endif

					if (clientSocket != null)
					{
						string sessionId = Guid.NewGuid().ToString("N");
						Log("[{0}] : Accepted connection from client {1}", sessionId, clientSocket.RemoteEndPoint);

						SpawnThread("VsCodeDebugSession_" + sessionId, () =>
						{
							using (var networkStream = new NetworkStream(clientSocket))
							{
								try
								{
									RunSession(sessionId, networkStream);
								}
								catch (Exception ex)
								{
									Log("[{0}] : Error : {1}", ex.Message);
								}
							}

#if DOTNET_CORE
							clientSocket.Dispose();
#else
							clientSocket.Close();
#endif
							Log("[{0}] : Client connection closed", sessionId);
						});
					}
				}
			}
			catch (Exception e)
			{
				Log("Fatal error in listening thread : {0}", e.Message);
			}
			finally
			{
				if (serverSocket != null)
					serverSocket.Stop();
			}
		}


		private void RunSession(string sessionId, NetworkStream stream)
		{
			DebugSession debugSession = null;

			lock (m_Lock)
			{
				if (m_Current != null)
					debugSession = new MoonSharpDebugSession(this, m_Current);
				else
					debugSession = new EmptyDebugSession(this);
			}

			debugSession.ProcessLoop(stream, stream);
		}

		private void Log(string format, params object[] args)
		{
			Action<string> logger = Logger;

			if (logger != null)
			{
				string msg = string.Format(format, args);
				logger(msg);
			}
		}


		private static void SpawnThread(string name, Action threadProc)
		{
#if DOTNET_CORE
			System.Threading.Tasks.Task.Run(() => threadProc());
#else
			new System.Threading.Thread(() => threadProc())
			{
				IsBackground = true,
				Name = name
			}
			.Start();
#endif
		}
	}
}

#else
						using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.VsCodeDebugger
{
	public class MoonSharpVsCodeDebugServer : IDisposable
	{
		public MoonSharpVsCodeDebugServer(int port = 41912)
		{
		}

		[Obsolete("Use the constructor taking only a port, and the 'Attach' method instead.")]
		public MoonSharpVsCodeDebugServer(Script script, int port, Func<SourceCode, string> sourceFinder = null)
		{
		}

		public void AttachToScript(Script script, string name, Func<SourceCode, string> sourceFinder = null)
		{
		}

		public IEnumerable<KeyValuePair<int, string>> GetAttachedDebuggersByIdAndName()
		{
			yield break;
		}


		public int? CurrentId
		{
			get { return null; }
			set { }
		}


		public Script Current
		{
			get { return null; }
			set { }
		}

		/// <summary>
		/// Detaches the specified script. The debugger attached to that script will get disconnected.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <exception cref="ArgumentException">Thrown if the script cannot be found.</exception>
		public void Detach(Script script)
		{

		}

		public Action<string> Logger { get; set; }

		
		[Obsolete("Use the Attach method instead.")]
		public IDebugger GetDebugger()
		{
			return null;
		}

		public void Dispose()
		{
		}

		public MoonSharpVsCodeDebugServer Start()
		{
			return this;
		}

	}
}
#endif