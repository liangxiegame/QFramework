using System.IO;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	/// <summary>
	/// Abstract class implementing a file Lua userdata. Methods are meant to be called by Lua code.
	/// </summary>
	internal abstract class StreamFileUserDataBase : FileUserDataBase
	{
		protected Stream m_Stream;
		protected StreamReader m_Reader;
		protected StreamWriter m_Writer;
		protected bool m_Closed = false;


		protected void Initialize(Stream stream, StreamReader reader, StreamWriter writer)
		{
			m_Stream = stream;
			m_Reader = reader;
			m_Writer = writer;
		}


		private void CheckFileIsNotClosed()
		{
			if (m_Closed)
				throw new ScriptRuntimeException("attempt to use a closed file");
		}


		protected override bool Eof()
		{
			CheckFileIsNotClosed();

			if (m_Reader != null)
				return m_Reader.EndOfStream;
			else
				return false;
		}

		protected override string ReadLine()
		{
			CheckFileIsNotClosed();
			return m_Reader.ReadLine();
		}

		protected override string ReadToEnd()
		{
			CheckFileIsNotClosed();
			return m_Reader.ReadToEnd();
		}

		protected override string ReadBuffer(int p)
		{
			CheckFileIsNotClosed();
			char[] buffer = new char[p];
			int length = m_Reader.ReadBlock(buffer, 0, p);
			return new string(buffer, 0, length);
		}

		protected override char Peek()
		{
			CheckFileIsNotClosed();
			return (char)m_Reader.Peek();
		}

		protected override void Write(string value)
		{
			CheckFileIsNotClosed();
			m_Writer.Write(value);
		}

		protected override string Close()
		{
			CheckFileIsNotClosed();

			if (m_Writer != null)
				m_Writer.Dispose();

			if (m_Reader != null)
				m_Reader.Dispose();

			m_Stream.Dispose();

			m_Closed = true;

			return null;
		}

		public override bool flush()
		{
			CheckFileIsNotClosed();

			if (m_Writer != null)
				m_Writer.Flush();

			return true;
		}

		public override long seek(string whence, long offset)
		{
			CheckFileIsNotClosed();
			if (whence != null)
			{
				if (whence == "set")
				{
					m_Stream.Seek(offset, SeekOrigin.Begin);
				}
				else if (whence == "cur")
				{
					m_Stream.Seek(offset, SeekOrigin.Current);
				}
				else if (whence == "end")
				{
					m_Stream.Seek(offset, SeekOrigin.End);
				}
				else
				{
					throw ScriptRuntimeException.BadArgument(0, "seek", "invalid option '" + whence + "'");
				}
			}

			return m_Stream.Position;
		}

		public override bool setvbuf(string mode)
		{
			CheckFileIsNotClosed();
			if (m_Writer != null)
				m_Writer.AutoFlush = (mode == "no" || mode == "line");
			return true;
		}

		protected internal override bool isopen()
		{
			return !m_Closed;
		}

	}
}
