using System;
using System.IO;

namespace MoonSharp.Interpreter.IO
{
	/// <summary>
	/// An adapter over Stream which bypasses the Dispose and Close methods.
	/// Used to work around the pesky wrappers .NET has over Stream (BinaryReader, StreamWriter, etc.) which think they
	/// own the Stream and close them when they shouldn't. Damn.
	/// </summary>
	public class UndisposableStream : Stream
	{
		Stream m_Stream;

		public UndisposableStream(Stream stream)
		{
			m_Stream = stream;
		}

		protected override void Dispose(bool disposing)
		{
		}

#if !(PCL || ENABLE_DOTNET || NETFX_CORE)
		public override void Close()
		{
		}
#endif


		public override bool CanRead
		{
			get { return m_Stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return m_Stream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return m_Stream.CanWrite; }
		}

		public override void Flush()
		{
			m_Stream.Flush();
		}

		public override long Length
		{
			get { return m_Stream.Length; }
		}

		public override long Position
		{
			get { return m_Stream.Position; }
			set { m_Stream.Position = value; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return m_Stream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return m_Stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			m_Stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			m_Stream.Write(buffer, offset, count);
		}

#if (!(NETFX_CORE))
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return m_Stream.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return m_Stream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			m_Stream.EndWrite(asyncResult);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return m_Stream.EndRead(asyncResult);
		}
#endif
		public override bool CanTimeout
		{
			get { return m_Stream.CanTimeout; }
		}


		public override bool Equals(object obj)
		{
			return m_Stream.Equals(obj);
		}

		public override int GetHashCode()
		{
			return m_Stream.GetHashCode();
		}


		public override int ReadByte()
		{
			return m_Stream.ReadByte();
		}

		public override int ReadTimeout
		{
			get
			{
				return m_Stream.ReadTimeout;
			}
			set
			{
				m_Stream.ReadTimeout = value;
			}
		}

		public override string ToString()
		{
			return m_Stream.ToString();
		}

		public override void WriteByte(byte value)
		{
			m_Stream.WriteByte(value);
		}

		public override int WriteTimeout
		{
			get
			{
				return m_Stream.WriteTimeout;
			}
			set
			{
				m_Stream.WriteTimeout = value;
			}
		}


	}
}
