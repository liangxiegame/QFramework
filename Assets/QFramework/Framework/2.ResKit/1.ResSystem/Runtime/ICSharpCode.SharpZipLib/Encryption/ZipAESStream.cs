using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
	/// <summary>
	/// Encrypts and decrypts AES ZIP
	/// </summary>
	/// <remarks>
	/// Based on information from http://www.winzip.com/aes_info.htm
	/// and http://www.gladman.me.uk/cryptography_technology/fileencrypt/
	/// </remarks>
	internal class ZipAESStream : CryptoStream
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
		/// <param name="transform">Instance of ZipAESTransform</param>
		/// <param name="mode">Read or Write</param>
		public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode)
			: base(stream, transform, mode)
		{

			_stream = stream;
			_transform = transform;
			_slideBuffer = new byte[1024];

			_blockAndAuth = CRYPTO_BLOCK_SIZE + AUTH_CODE_LENGTH;

			// mode:
			//  CryptoStreamMode.Read means we read from "stream" and pass decrypted to our Read() method.
			//  Write bypasses this stream and uses the Transform directly.
			if (mode != CryptoStreamMode.Read) {
				throw new Exception("ZipAESStream only for read");
			}
		}

		// The final n bytes of the AES stream contain the Auth Code.
		private const int AUTH_CODE_LENGTH = 10;

		private Stream _stream;
		private ZipAESTransform _transform;
		private byte[] _slideBuffer;
		private int _slideBufStartPos;
		private int _slideBufFreePos;
		// Blocksize is always 16 here, even for AES-256 which has transform.InputBlockSize of 32.
		private const int CRYPTO_BLOCK_SIZE = 16;
		private int _blockAndAuth;

		/// <summary>
		/// Reads a sequence of bytes from the current CryptoStream into buffer,
		/// and advances the position within the stream by the number of bytes read.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int nBytes = 0;
			while (nBytes < count) {
				// Calculate buffer quantities vs read-ahead size, and check for sufficient free space
				int byteCount = _slideBufFreePos - _slideBufStartPos;

				// Need to handle final block and Auth Code specially, but don't know total data length.
				// Maintain a read-ahead equal to the length of (crypto block + Auth Code). 
				// When that runs out we can detect these final sections.
				int lengthToRead = _blockAndAuth - byteCount;
				if (_slideBuffer.Length - _slideBufFreePos < lengthToRead) {
					// Shift the data to the beginning of the buffer
					int iTo = 0;
					for (int iFrom = _slideBufStartPos; iFrom < _slideBufFreePos; iFrom++, iTo++) {
						_slideBuffer[iTo] = _slideBuffer[iFrom];
					}
					_slideBufFreePos -= _slideBufStartPos;      // Note the -=
					_slideBufStartPos = 0;
				}
				int obtained = _stream.Read(_slideBuffer, _slideBufFreePos, lengthToRead);
				_slideBufFreePos += obtained;

				// Recalculate how much data we now have
				byteCount = _slideBufFreePos - _slideBufStartPos;
				if (byteCount >= _blockAndAuth) {
					// At least a 16 byte block and an auth code remains.
					_transform.TransformBlock(_slideBuffer,
											  _slideBufStartPos,
											  CRYPTO_BLOCK_SIZE,
											  buffer,
											  offset);
					nBytes += CRYPTO_BLOCK_SIZE;
					offset += CRYPTO_BLOCK_SIZE;
					_slideBufStartPos += CRYPTO_BLOCK_SIZE;
				} else {
					// Last round.
					if (byteCount > AUTH_CODE_LENGTH) {
						// At least one byte of data plus auth code
						int finalBlock = byteCount - AUTH_CODE_LENGTH;
						_transform.TransformBlock(_slideBuffer,
												  _slideBufStartPos,
												  finalBlock,
												  buffer,
												  offset);

						nBytes += finalBlock;
						_slideBufStartPos += finalBlock;
					} else if (byteCount < AUTH_CODE_LENGTH)
						throw new Exception("Internal error missed auth code"); // Coding bug
																				// Final block done. Check Auth code.
					byte[] calcAuthCode = _transform.GetAuthCode();
					for (int i = 0; i < AUTH_CODE_LENGTH; i++) {
						if (calcAuthCode[i] != _slideBuffer[_slideBufStartPos + i]) {
							throw new Exception("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\n"
								+ "The file may be damaged.");
						}
					}

					break;  // Reached the auth code
				}
			}
			return nBytes;
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream. </param>
		/// <param name="offset">The byte offset in buffer at which to begin copying bytes to the current stream. </param>
		/// <param name="count">The number of bytes to be written to the current stream. </param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			// ZipAESStream is used for reading but not for writing. Writing uses the ZipAESTransform directly.
			throw new NotImplementedException();
		}
	}
}
