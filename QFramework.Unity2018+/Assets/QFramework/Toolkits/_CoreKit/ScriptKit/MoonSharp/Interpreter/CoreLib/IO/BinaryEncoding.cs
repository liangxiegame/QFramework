using System.Text;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	class BinaryEncoding : Encoding
	{
		public BinaryEncoding()
			: base()
		{

		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return count;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			for (int i = 0; i < charCount; i++)
			{
				bytes[byteIndex + i] = (byte)((int)chars[charIndex + i]);
			}

			return charCount;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			for (int i = 0; i < byteCount; i++)
			{
				chars[charIndex + i] = (char)((int)bytes[byteIndex + i]);
			}

			return byteCount;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}
	}
}
