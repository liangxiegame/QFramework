using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	/// <summary>
	/// Abstract class implementing a file Lua userdata. Methods are meant to be called by Lua code.
	/// </summary>
	internal abstract class FileUserDataBase : RefIdObject
	{
		public DynValue lines(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			List<DynValue> readLines = new List<DynValue>();

			DynValue readValue = null;

			do
			{
				readValue = read(executionContext, args);
				readLines.Add(readValue);

			} while (readValue.IsNotNil());

			return DynValue.FromObject(executionContext.GetScript(), readLines.Select(s => s));
		}

		public DynValue read(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args.Count == 0)
			{
				string str = ReadLine();

				if (str == null)
					return DynValue.Nil;

				str = str.TrimEnd('\n', '\r');
				return DynValue.NewString(str);
			}
			else
			{
				List<DynValue> rets = new List<DynValue>();

				for (int i = 0; i < args.Count; i++)
				{
					DynValue v;

					if (args[i].Type == DataType.Number)
					{
						if (Eof())
							return DynValue.Nil;

						int howmany = (int)args[i].Number;

						string str = ReadBuffer(howmany);
						v = DynValue.NewString(str);
					}
					else 
					{
						string opt = args.AsType(i, "read", DataType.String, false).String;

						if (Eof())
						{
							v = opt.StartsWith("*a") ? DynValue.NewString("") : DynValue.Nil;
						}
						else if (opt.StartsWith("*n"))
						{
							double? d = ReadNumber();

							if (d.HasValue)
								v = DynValue.NewNumber(d.Value);
							else
								v = DynValue.Nil;
						}
						else if (opt.StartsWith("*a"))
						{
							string str = ReadToEnd();
							v = DynValue.NewString(str);
						}
						else if (opt.StartsWith("*l"))
						{
							string str = ReadLine();
							str = str.TrimEnd('\n', '\r');
							v = DynValue.NewString(str);
						}
						else if (opt.StartsWith("*L"))
						{
							string str = ReadLine();

							str = str.TrimEnd('\n', '\r');
							str += "\n";
							
							v = DynValue.NewString(str);
						}
						else
						{
							throw ScriptRuntimeException.BadArgument(i, "read", "invalid option");
						}
					}

					rets.Add(v);
				}

				return DynValue.NewTuple(rets.ToArray());
			}
		}


		public DynValue write(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				for (int i = 0; i < args.Count; i++)
				{
					//string str = args.AsStringUsingMeta(executionContext, i, "file:write");
					string str = args.AsType(i, "write", DataType.String, false).String;
					Write(str);
				}

				return UserData.Create(this);
			}
			catch (ScriptRuntimeException)
			{
				throw;
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message));
			}
		}

		public DynValue close(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				string msg = Close();
				if (msg == null)
					return DynValue.True;
				else
					return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(msg));
			}
			catch (ScriptRuntimeException)
			{
				throw;
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message));
			}
		}


		double? ReadNumber()
		{
			string chr = "";

			while (!Eof())
			{
				char c = Peek();
				if (char.IsWhiteSpace(c))
				{
					ReadBuffer(1);
				}
				else if (IsNumericChar(c, chr))
				{
					ReadBuffer(1);
					chr += c;
				}
				else break;
			}

			double d;

			if (double.TryParse(chr, out d))
			{
				return d;
			}
			else
			{
				return null;
			}
		}

		private bool IsNumericChar(char c, string numAsFar)
		{
			if (char.IsDigit(c))
				return true;

			if (c == '-')
				return numAsFar.Length == 0;

			if (c == '.')
				return !Framework.Do.StringContainsChar(numAsFar, '.');

			if (c == 'E' || c == 'e')
				return !(Framework.Do.StringContainsChar(numAsFar, 'E') || Framework.Do.StringContainsChar(numAsFar, 'e'));

			return false;
		}

		protected abstract bool Eof();
		protected abstract string ReadLine();
		protected abstract string ReadBuffer(int p);
		protected abstract string ReadToEnd();
		protected abstract char Peek();
		protected abstract void Write(string value);


		protected internal abstract bool isopen();
		protected abstract string Close();

		public abstract bool flush();
		public abstract long seek(string whence, long offset);
		public abstract bool setvbuf(string mode);

		public override string ToString()
		{
			if (isopen())
				return string.Format("file ({0:X8})", base.ReferenceID);
			else
				return "file (closed)";
		}
	}
}
