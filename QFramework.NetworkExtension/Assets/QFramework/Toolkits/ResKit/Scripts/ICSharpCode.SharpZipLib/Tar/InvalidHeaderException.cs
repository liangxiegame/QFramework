using System;

namespace ICSharpCode.SharpZipLib.Tar
{
	/// <summary>
	/// This exception is used to indicate that there is a problem
	/// with a TAR archive header.
	/// </summary>
	public class InvalidHeaderException : TarException
	{
		/// <summary>
		/// Initialise a new instance of the InvalidHeaderException class.
		/// </summary>
		public InvalidHeaderException()
		{
		}

		/// <summary>
		/// Initialises a new instance of the InvalidHeaderException class with a specified message.
		/// </summary>
		/// <param name="message">Message describing the exception cause.</param>
		public InvalidHeaderException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initialise a new instance of InvalidHeaderException
		/// </summary>
		/// <param name="message">Message describing the problem.</param>
		/// <param name="exception">The exception that is the cause of the current exception.</param>
		public InvalidHeaderException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
