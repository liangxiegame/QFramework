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
using System.Net;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.VsCodeDebugger.SDK
{
	internal class Utilities
	{
		private static readonly Regex VARIABLE = new Regex(@"\{(\w+)\}");


		/*
		 * Resolve hostname, dotted-quad notation for IPv4, or colon-hexadecimal notation for IPv6 to IPAddress.
		 * Returns null on failure.
		 */

		public static string ExpandVariables(string format, object variables, bool underscoredOnly = true)
		{
			if (variables == null)
			{
				variables = new { };
			}
			Type type = variables.GetType();
			return VARIABLE.Replace(format, match => {
				string name = match.Groups[1].Value;
				if (!underscoredOnly || name.StartsWith("_"))
				{

					PropertyInfo property = Framework.Do.GetProperty(type, name);
					if (property != null)
					{
						object value = property.GetValue(variables, null);
						return value.ToString();
					}
					return '{' + name + ": not found}";
				}
				return match.Groups[0].Value;
			});
		}

		/**
		 * converts the given absPath into a path that is relative to the given dirPath.
		 */
		public static string MakeRelativePath(string dirPath, string absPath)
		{
			if (!dirPath.EndsWith("/"))
			{
				dirPath += "/";
			}
			if (absPath.StartsWith(dirPath))
			{
				return absPath.Replace(dirPath, "");
			}
			return absPath;
			/*
			Uri uri1 = new Uri(path);
			Uri uri2 = new Uri(dir_path);
			return uri2.MakeRelativeUri(uri1).ToString();
			*/
		}
	}
}

#endif
