/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework.Test.Core
{
	using UnityEngine;
	using UnityEditor;
	using NUnit.Framework;
	using System.Collections.Generic;

	public class FilePathTest
	{

		[Test]
		public void FilePathTest_GetResPathInPersistentOrStream_Name()
		{
			//Arrange
			string name = FilePath.GetResPathInPersistentOrStream("");
			
			//Act
			//Try to rename the GameObject
			name = name.Substring(name.LastIndexOf("/") + 1);
			
			//Assert
			//The object has a new name
			Assert.IsEmpty(name);
			
			name=FilePath.GetResPathInPersistentOrStream("Despacito");
			name = name.Substring(name.LastIndexOf("/") + 1);
			Assert.AreEqual("Despacito",name);
		}

		[Test]
		public void FilePathTest_GetParentDir_Name()
		{
			string name = FilePath.GetParentDir(FilePath.PersistentDataPath4Res);
			name = name.Substring(name.LastIndexOf("/") + 1);
			Assert.AreEqual("Res",name);
			
			name=FilePath.GetParentDir(FilePath.PersistentDataPath4Res,2);
			name=name+"/Res";
			name = name.Substring(name.LastIndexOf("/") + 1);
			Assert.AreEqual("Res",name);
		}

		[Test]
		public void FilePathTest_GetFileInFolder_Name()
		{
			List<string> testStrs=new List<string>();
			string testPath = Application.dataPath + "/QUGame/QFramework/Core/Pool/";

			FilePath.GetFileInFolder(testPath,"ObjectPool.cs",testStrs);
			
			Assert.AreEqual("ObjectPool.cs",testStrs[0].Substring(testStrs[0].LastIndexOf("/")+1));
		}
	}
}
