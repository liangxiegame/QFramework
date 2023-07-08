#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

/*---------------------------------------------------------------------------------------------
Copyright (c) Microsoft Corporation

All rights reserved. 

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization.Json;
using MoonSharp.Interpreter.Serialization;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ProtocolMessage
	{
		public int seq;
		public string type { get; private set; }

		public ProtocolMessage(string typ)
		{
			type = typ;
		}

		public ProtocolMessage(string typ, int sq)
		{
			type = typ;
			seq = sq;
		}
	}

	public class Request : ProtocolMessage
	{
		public string command;
		public Table arguments;

		public Request(int id, string cmd, Table arg) : base("request", id)
		{
			command = cmd;
			arguments = arg;
		}
	}

	/*
	 * subclasses of ResponseBody are serialized as the body of a response.
	 * Don't change their instance variables since that will break the debug protocol.
	 */
	public class ResponseBody
	{
		// empty
	}

	public class Response : ProtocolMessage
	{
		public bool success { get; private set; }
		public string message { get; private set; }
		public int request_seq { get; private set; }
		public string command { get; private set; }
		public ResponseBody body { get; private set; }

		public Response(Table req) : base("response")
		{
			success = true;
			request_seq = req.Get("seq").ToObject<int>();
			command = req.Get("command").ToObject<string>();
		}

		public void SetBody(ResponseBody bdy)
		{
			success = true;
			body = bdy;
		}

		public void SetErrorBody(string msg, ResponseBody bdy = null)
		{
			success = false;
			message = msg;
			body = bdy;
		}
	}

	public class Event : ProtocolMessage
	{
		public string @event { get; private set; }
		public object body { get; private set; }

		public Event(string type, object bdy = null) : base("event")
		{
			@event = type;
			body = bdy;
		}
	}

	/*
     * The ProtocolServer can be used to implement a server that uses the VSCode debug protocol.
     */
	public abstract class ProtocolServer
	{
		public bool TRACE;
		public bool TRACE_RESPONSE;

		protected const int BUFFER_SIZE = 4096;
		protected const string TWO_CRLF = "\r\n\r\n";
		protected static readonly Regex CONTENT_LENGTH_MATCHER = new Regex(@"Content-Length: (\d+)");

		protected static readonly Encoding Encoding = System.Text.Encoding.UTF8;

		private int _sequenceNumber;

		private Stream _outputStream;

		private ByteBuffer _rawData;
		private int _bodyLength;

		private bool _stopRequested;


		public ProtocolServer()
		{
			_sequenceNumber = 1;
			_bodyLength = -1;
			_rawData = new ByteBuffer();
		}

		public void ProcessLoop(Stream inputStream, Stream outputStream)
		{
			_outputStream = outputStream;

			byte[] buffer = new byte[BUFFER_SIZE];

			_stopRequested = false;
			while (!_stopRequested)
			{
				var read = inputStream.Read(buffer, 0, buffer.Length);

				if (read == 0)
				{
					// end of stream
					break;
				}

				if (read > 0)
				{
					_rawData.Append(buffer, read);
					ProcessData();
				}
			}
		}

		public void Stop()
		{
			_stopRequested = true;
		}

		public void SendEvent(Event e)
		{
			SendMessage(e);
		}

		protected abstract void DispatchRequest(string command, Table args, Response response);

		// ---- private ------------------------------------------------------------------------

		private void ProcessData()
		{
			while (true)
			{
				if (_bodyLength >= 0)
				{
					if (_rawData.Length >= _bodyLength)
					{
						var buf = _rawData.RemoveFirst(_bodyLength);

						_bodyLength = -1;

						Dispatch(Encoding.GetString(buf));

						continue;   // there may be more complete messages to process
					}
				}
				else
				{
					string s = _rawData.GetString(Encoding);
					var idx = s.IndexOf(TWO_CRLF);
					if (idx != -1)
					{
						Match m = CONTENT_LENGTH_MATCHER.Match(s);
						if (m.Success && m.Groups.Count == 2)
						{
							_bodyLength = Convert.ToInt32(m.Groups[1].ToString());

							_rawData.RemoveFirst(idx + TWO_CRLF.Length);

							continue;   // try to handle a complete message
						}
					}
				}
				break;
			}
		}

		private void Dispatch(string req)
		{
			try
			{
				Table request = JsonTableConverter.JsonToTable(req);
				if (request != null && request["type"].ToString() == "request")
				{
					if (TRACE)
						Console.Error.WriteLine(string.Format("C {0}: {1}", request["command"], req));

					var response = new Response(request);

					DispatchRequest(request.Get("command").String, request.Get("arguments").Table, response);

					SendMessage(response);
				}
			}
			catch
			{
			}
		}

		protected void SendMessage(ProtocolMessage message)
		{
			message.seq = _sequenceNumber++;

			if (TRACE_RESPONSE && message.type == "response")
			{
				Console.Error.WriteLine(string.Format(" R: {0}", JsonTableConverter.ObjectToJson(message)));
			}
			if (TRACE && message.type == "event")
			{
				Event e = (Event)message;
				Console.Error.WriteLine(string.Format("E {0}: {1}", e.@event, JsonTableConverter.ObjectToJson(e.body)));
			}

			var data = ConvertToBytes(message);
			try
			{
				_outputStream.Write(data, 0, data.Length);
				_outputStream.Flush();
			}
			catch (Exception)
			{
				// ignore
			}
		}


		private static byte[] ConvertToBytes(ProtocolMessage request)
		{
			var asJson = JsonTableConverter.ObjectToJson(request);
			byte[] jsonBytes = Encoding.GetBytes(asJson);

			string header = string.Format("Content-Length: {0}{1}", jsonBytes.Length, TWO_CRLF);
			byte[] headerBytes = Encoding.GetBytes(header);

			byte[] data = new byte[headerBytes.Length + jsonBytes.Length];
			System.Buffer.BlockCopy(headerBytes, 0, data, 0, headerBytes.Length);
			System.Buffer.BlockCopy(jsonBytes, 0, data, headerBytes.Length, jsonBytes.Length);

			return data;
		}
	}

	//--------------------------------------------------------------------------------------

	class ByteBuffer
	{
		private byte[] _buffer;

		public ByteBuffer()
		{
			_buffer = new byte[0];
		}

		public int Length
		{
			get { return _buffer.Length; }
		}

		public string GetString(Encoding enc)
		{
			return enc.GetString(_buffer);
		}

		public void Append(byte[] b, int length)
		{
			byte[] newBuffer = new byte[_buffer.Length + length];
			System.Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _buffer.Length);
			System.Buffer.BlockCopy(b, 0, newBuffer, _buffer.Length, length);
			_buffer = newBuffer;
		}

		public byte[] RemoveFirst(int n)
		{
			byte[] b = new byte[n];
			System.Buffer.BlockCopy(_buffer, 0, b, 0, n);
			byte[] newBuffer = new byte[_buffer.Length - n];
			System.Buffer.BlockCopy(_buffer, n, newBuffer, 0, _buffer.Length - n);
			_buffer = newBuffer;
			return b;
		}












	}
}
#endif