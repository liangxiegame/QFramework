/****************************************************************************
 * Copyright (c) 2018 liangxie
 * 
 * http://liangxiegame.com
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

namespace QFramework.Example
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class ExtensionExample : MonoBehaviour
	{
		private void Start()
		{
			CSharpExtensions();
			UnityExtensions();
		}

		private delegate void TestDelegate();

		private void CSharpExtensions()
		{
			#region IsNull,IsNotNull

			var simpleClass = new object();

			if (simpleClass.IsNull()) // simpleClass == null
			{

			}
			else if (simpleClass.IsNotNull()) // simpleClasss != null
			{

			}

			#endregion


			#region InvokeGracefully

			// action
			Action action = () => Debug.Log("action called");
			action.InvokeGracefully(); // if (action != null) action();

			// func
			Func<int> func = () => 1;
			func.InvokeGracefully();

			/*
			public static T InvokeGracefully<T>(this Func<T> selfFunc)
			{
				return null != selfFunc ? selfFunc() : default(T);
			}
			*/

			// delegate
			TestDelegate testDelegate = () => { };
			testDelegate.InvokeGracefully();

			#endregion

			var typeName = GenericExtention.GetTypeName<string>();
			Debug.Log(typeName);

			#region ForEach

			// Array
			var testArray = new int[] {1, 2, 3};
			testArray.ForEach(number => Debug.Log(number));

			// IEnumerable<T>
			IEnumerable<int> testIenumerable = new List<int> {1, 2, 3};
			testIenumerable.ForEach(number => Debug.Log(number));

			// testList
			var testList = new List<int> {1, 2, 3};
			testList.ForEach(number => Debug.Log(number));
			testList.ForEachReverse(number => Debug.Log(number));

			#endregion

			#region Merge

			var dictionary1 = new Dictionary<string, string> {{"1", "2"}};
			var dictionary2 = new Dictionary<string, string> {{"3", "4"}};
			var dictionary3 = dictionary1.Merge(dictionary2);
			dictionary3.ForEach(pair => Debug.LogFormat("{0}:{1}", pair.Key, pair.Value));

			#endregion

			#region CreateDirIfNotExists,DeleteDirIfExists,DeleteFileIfExists

			var testDir = Application.persistentDataPath.CombinePath("TestFolder");
			testDir.CreateDirIfNotExists();

			Debug.Log(Directory.Exists(testDir));
			testDir.DeleteDirIfExists();
			Debug.Log(Directory.Exists(testDir));

			var testFile = testDir.CombinePath("test.txt");
			testDir.CreateDirIfNotExists();
			File.Create(testFile);
			testFile.DeleteFileIfExists();

			#endregion

			#region ImplementsInterface<T>

			this.ImplementsInterface<ISingleton>();

			#endregion

			#region ReflectionExtension.GetAssemblyCSharp()

			var selfType = ReflectionExtension.GetAssemblyCSharp().GetType("QFramework.Example.ExtensionExample");
			Debug.Log(selfType);

			#endregion

			#region string's extension

			var emptyStr = string.Empty;

			Debug.Log(emptyStr.IsNotNullAndEmpty());
			Debug.Log(emptyStr.IsNullOrEmpty());
			emptyStr = emptyStr.Append("appended").Append("1").ToString();
			Debug.Log(emptyStr);
			Debug.Log(emptyStr.IsNullOrEmpty());

			#endregion
		}

		void UnityExtensions()
		{

		}
	}
}