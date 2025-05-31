// Disable warnings about XML documentation
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.CoreLib.IO;
using MoonSharp.Interpreter.Platforms;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing io Lua functions. Proper support requires a compatible IPlatformAccessor
	/// </summary>
	[MoonSharpModule(Namespace = "io")]
	public class IoModule
	{
		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			UserData.RegisterType<FileUserDataBase>(InteropAccessMode.Default, "file");

			Table meta = new Table(ioTable.OwnerScript);
			DynValue __index = DynValue.NewCallback(new CallbackFunction(__index_callback, "__index_callback"));
			meta.Set("__index", __index);
			ioTable.MetaTable = meta;

			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdIn, globalTable.OwnerScript.Options.Stdin);
			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdOut, globalTable.OwnerScript.Options.Stdout);
			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdErr, globalTable.OwnerScript.Options.Stderr);
		}

		private static DynValue __index_callback(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string name = args[1].CastToString();

			if (name == "stdin")
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdIn);
			else if (name == "stdout")
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdOut);
			else if (name == "stderr")
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdErr);
			else
				return DynValue.Nil;
		}

		private static DynValue GetStandardFile(Script S, StandardFileType file)
		{
			Table R = S.Registry;

			DynValue ff = R.Get("853BEAAF298648839E2C99D005E1DF94_STD_" + file.ToString());
			return ff;
		}

		private static void SetStandardFile(Script S, StandardFileType file, Stream optionsStream)
		{
			Table R = S.Registry;

			optionsStream = optionsStream ?? Script.GlobalOptions.Platform.IO_GetStandardStream(file);

			FileUserDataBase udb = null;

			if (file == StandardFileType.StdIn)
				udb = StandardIOFileUserDataBase.CreateInputStream(optionsStream);
			else
				udb = StandardIOFileUserDataBase.CreateOutputStream(optionsStream);

			R.Set("853BEAAF298648839E2C99D005E1DF94_STD_" + file.ToString(), UserData.Create(udb));
		}


		static FileUserDataBase GetDefaultFile(ScriptExecutionContext executionContext, StandardFileType file)
		{
			Table R = executionContext.GetScript().Registry;

			DynValue ff = R.Get("853BEAAF298648839E2C99D005E1DF94_" + file.ToString());

			if (ff.IsNil())
			{
				ff = GetStandardFile(executionContext.GetScript(), file);
			}

			return ff.CheckUserDataType<FileUserDataBase>("getdefaultfile(" + file.ToString() + ")");
		}


		static void SetDefaultFile(ScriptExecutionContext executionContext, StandardFileType file, FileUserDataBase fileHandle)
		{
			SetDefaultFile(executionContext.GetScript(), file, fileHandle);
		}

		internal static void SetDefaultFile(Script script, StandardFileType file, FileUserDataBase fileHandle)
		{
			Table R = script.Registry;
			R.Set("853BEAAF298648839E2C99D005E1DF94_" + file.ToString(), UserData.Create(fileHandle));
		}

		public static void SetDefaultFile(Script script, StandardFileType file, Stream stream)
		{
			if (file == StandardFileType.StdIn)
				SetDefaultFile(script, file, StandardIOFileUserDataBase.CreateInputStream(stream));
			else
				SetDefaultFile(script, file, StandardIOFileUserDataBase.CreateOutputStream(stream));
		}


		[MoonSharpModuleMethod]
		public static DynValue close(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			FileUserDataBase outp = args.AsUserData<FileUserDataBase>(0, "close", true) ?? GetDefaultFile(executionContext, StandardFileType.StdOut);
			return outp.close(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue flush(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			FileUserDataBase outp = args.AsUserData<FileUserDataBase>(0, "close", true) ?? GetDefaultFile(executionContext, StandardFileType.StdOut);
			outp.flush();
			return DynValue.True;
		}


		[MoonSharpModuleMethod]
		public static DynValue input(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return HandleDefaultStreamSetter(executionContext, args, StandardFileType.StdIn);
		}

		[MoonSharpModuleMethod]
		public static DynValue output(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return HandleDefaultStreamSetter(executionContext, args, StandardFileType.StdOut);
		}

		private static DynValue HandleDefaultStreamSetter(ScriptExecutionContext executionContext, CallbackArguments args, StandardFileType defaultFiles)
		{
			if (args.Count == 0 || args[0].IsNil())
			{
				var file = GetDefaultFile(executionContext, defaultFiles);
				return UserData.Create(file);
			}

			FileUserDataBase inp = null;

			if (args[0].Type == DataType.String || args[0].Type == DataType.Number)
			{
				string fileName = args[0].CastToString();
				inp = Open(executionContext, fileName, GetUTF8Encoding(), defaultFiles == StandardFileType.StdIn ? "r" : "w");
			}
			else
			{
				inp = args.AsUserData<FileUserDataBase>(0, defaultFiles == StandardFileType.StdIn ? "input" : "output", false);
			}

			SetDefaultFile(executionContext, defaultFiles, inp);

			return UserData.Create(inp);
		}

		private static Encoding GetUTF8Encoding()
		{
			return new System.Text.UTF8Encoding(false); 
		}

		[MoonSharpModuleMethod]
		public static DynValue lines(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string filename = args.AsType(0, "lines", DataType.String, false).String;

			try
			{
				List<DynValue> readLines = new List<DynValue>();

				using (var stream = Script.GlobalOptions.Platform.IO_OpenFile(executionContext.GetScript(), filename, null, "r"))
				{
					using (var reader = new System.IO.StreamReader(stream))
					{
						while (!reader.EndOfStream)
						{
							string line = reader.ReadLine();
							readLines.Add(DynValue.NewString(line));
						}
					}
				}

				readLines.Add(DynValue.Nil);

				return DynValue.FromObject(executionContext.GetScript(), readLines.Select(s => s));
			}
			catch (Exception ex)
			{
				throw new ScriptRuntimeException(IoExceptionToLuaMessage(ex, filename));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue open(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string filename = args.AsType(0, "open", DataType.String, false).String;
			DynValue vmode = args.AsType(1, "open", DataType.String, true);
			DynValue vencoding = args.AsType(2, "open", DataType.String, true);

			string mode = vmode.IsNil() ? "r" : vmode.String;

			string invalidChars = mode.Replace("+", "")
				.Replace("r", "")
				.Replace("a", "")
				.Replace("w", "")
				.Replace("b", "")
				.Replace("t", "");

			if (invalidChars.Length > 0)
				throw ScriptRuntimeException.BadArgument(1, "open", "invalid mode");


			try
			{
				string encoding = vencoding.IsNil() ? null : vencoding.String;

				// list of codes: http://msdn.microsoft.com/en-us/library/vstudio/system.text.encoding%28v=vs.90%29.aspx.
				// In addition, "binary" is available.
				Encoding e = null;
				bool isBinary = Framework.Do.StringContainsChar(mode, 'b');

				if (encoding == "binary")
				{
					isBinary = true;
					e = new BinaryEncoding();
				}
				else if (encoding == null)
				{
					if (!isBinary) e = GetUTF8Encoding();
					else e = new BinaryEncoding();
				}
				else
				{
					if (isBinary)
						throw new ScriptRuntimeException("Can't specify encodings other than nil or 'binary' for binary streams.");

					e = Encoding.GetEncoding(encoding);
				}

				return UserData.Create(Open(executionContext, filename, e, mode));
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil,
					DynValue.NewString(IoExceptionToLuaMessage(ex, filename)));
			}

		}

		public static string IoExceptionToLuaMessage(Exception ex, string filename)
		{
			if (ex is System.IO.FileNotFoundException)
				return string.Format("{0}: No such file or directory", filename);
			else
				return ex.Message;
		}

		[MoonSharpModuleMethod]
		public static DynValue type(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.UserData)
				return DynValue.Nil;

			FileUserDataBase file = args[0].UserData.Object as FileUserDataBase;

			if (file == null)
				return DynValue.Nil;
			else if (file.isopen())
				return DynValue.NewString("file");
			else
				return DynValue.NewString("closed file");
		}

		[MoonSharpModuleMethod]
		public static DynValue read(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			FileUserDataBase file = GetDefaultFile(executionContext, StandardFileType.StdIn);
			return file.read(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue write(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			FileUserDataBase file = GetDefaultFile(executionContext, StandardFileType.StdOut);
			return file.write(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue tmpfile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string tmpfilename = Script.GlobalOptions.Platform.IO_OS_GetTempFilename();
			FileUserDataBase file = Open(executionContext, tmpfilename, GetUTF8Encoding(), "w");
			return UserData.Create(file);
		}

		private static FileUserDataBase Open(ScriptExecutionContext executionContext, string filename, Encoding encoding, string mode)
		{
			return new FileUserData(executionContext.GetScript(), filename, encoding, mode);
		}


	}

}
