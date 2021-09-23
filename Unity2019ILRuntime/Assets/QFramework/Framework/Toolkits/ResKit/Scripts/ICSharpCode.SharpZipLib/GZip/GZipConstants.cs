namespace ICSharpCode.SharpZipLib.GZip
{
	/// <summary>
	/// This class contains constants used for gzip.
	/// </summary>
	sealed public class GZipConstants
	{
		/// <summary>
		/// Magic number found at start of GZIP header
		/// </summary>
		public const int GZIP_MAGIC = 0x1F8B;

		/*  The flag byte is divided into individual bits as follows:
			
			bit 0   FTEXT
			bit 1   FHCRC
			bit 2   FEXTRA
			bit 3   FNAME
			bit 4   FCOMMENT
			bit 5   reserved
			bit 6   reserved
			bit 7   reserved
		 */

		/// <summary>
		/// Flag bit mask for text
		/// </summary>
		public const int FTEXT = 0x1;

		/// <summary>
		/// Flag bitmask for Crc
		/// </summary>
		public const int FHCRC = 0x2;

		/// <summary>
		/// Flag bit mask for extra
		/// </summary>
		public const int FEXTRA = 0x4;

		/// <summary>
		/// flag bitmask for name
		/// </summary>
		public const int FNAME = 0x8;

		/// <summary>
		/// flag bit mask indicating comment is present
		/// </summary>
		public const int FCOMMENT = 0x10;

		/// <summary>
		/// Initialise default instance.
		/// </summary>
		/// <remarks>Constructor is private to prevent instances being created.</remarks>
		GZipConstants()
		{
		}
	}
}
