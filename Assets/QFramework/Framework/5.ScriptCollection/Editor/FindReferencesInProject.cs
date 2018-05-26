/****************************************************************************
 * Copyright (c) 2018 huibinye
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
	public class FindReferencesInProject
	{

#if UNITY_EDITOR_OSX

		[MenuItem("Assets/Find References In Project", false, 2000)]
		private static void FindProjectReferences()
		{
			var appDataPath = Application.dataPath;
			var output = "";
			var selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			var references = new List<string>();

			var guid = AssetDatabase.AssetPathToGUID(selectedAssetPath);

			var psi = new System.Diagnostics.ProcessStartInfo
			{
				WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized,
				FileName = "/usr/bin/mdfind",
				Arguments = "-onlyin " + Application.dataPath + " " + guid,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			var process = new System.Diagnostics.Process {StartInfo = psi};

			process.OutputDataReceived += (sender, e) =>
			{
				if (string.IsNullOrEmpty(e.Data))
					return;

				var relativePath = "Assets" + e.Data.Replace(appDataPath, "");

				// skip the meta file of whatever we have selected
				if (relativePath == selectedAssetPath + ".meta")
					return;

				references.Add(relativePath);

			};

			process.ErrorDataReceived += (sender, e) =>
			{
				if (string.IsNullOrEmpty(e.Data))
					return;

				output += "Error: " + e.Data + "\n";
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			process.WaitForExit(2000);

			foreach (var file in references)
			{
				output += file + "\n";
				Debug.Log(file, AssetDatabase.LoadMainAssetAtPath(file));
			}

			Debug.LogWarning(references.Count + " references found for object " + Selection.activeObject.name + "\n\n" + output);
		}

#endif
	}
}