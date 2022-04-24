using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	/// <summary>
	/// ZipException represents exceptions specific to Zip classes and code.
	/// </summary>
	public class ZipException : SharpZipBaseException
	{

		/// <summary>
		/// Initialise a new instance of <see cref="ZipException" />.
		/// </summary>
		public ZipException()
		{
		}

		/// <summary>
		/// Initialise a new instance of <see cref="ZipException" /> with its message string.
		/// </summary>
		/// <param name="message">A <see cref="string"/> that describes the error.</param>
		public ZipException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initialise a new instance of <see cref="ZipException" />.
		/// </summary>
		/// <param name="message">A <see cref="string"/> that describes the error.</param>
		/// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
		public ZipException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
