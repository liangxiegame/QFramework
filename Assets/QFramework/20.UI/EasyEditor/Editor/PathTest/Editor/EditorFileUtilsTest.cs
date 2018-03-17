/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
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

namespace QFramework.Test.Core
{
	using UnityEngine;
	using NUnit.Framework;
	using Editor;

	public class EditorFileUtilsTest
	{
		public static string testPath = Application.dataPath + "/PTUGame/PTFramework/Core/Pool/";

		[Test]
		public void EditorFileUtilsTest_GetDirSubFilePathList_Count()
		{
			//Arrange
			string t1 = testPath;

			//Act
			//Try to rename the GameObject
			int length = EditorFileUtils.GetDirSubFilePathList(testPath, true, "cs").Count;

			//Assert
			//The object has a new name
			Assert.AreEqual(4, length);

			int length1 = EditorFileUtils.GetDirSubFilePathList(testPath, false, "cs").Count;
			Assert.AreEqual(2, length1);
		}

		[Test]
		public void EditorFileUtilsTest_GetDirSubDirNameList_Count()
		{
			//Arrange
			string t1 = testPath;

			//Act
			//Try to rename the GameObject
			int length = EditorFileUtils.GetDirSubDirNameList(t1).Count;

			//Assert
			//The object has a new name
			Assert.AreEqual(1, length);
		}

		[Test]
		public void EditorFileUtilsTest_GetFileName_Name()
		{
			//Arrange
			string t1 = testPath + "ObjectPool.cs";

			//Act
			//Try to rename the GameObject
			string name = EditorFileUtils.GetFileName(t1);

			//Assert
			//The object has a new name
			Assert.AreEqual("ObjectPool.cs", name);
		}

		[Test]
		public void EditorFileUtilsTest_GetFileNameWithoutExtend_Name()
		{
			//Arrange
			string t1 = testPath + "ObjectPool.cs";

			//Act
			//Try to rename the GameObject
			string name = EditorFileUtils.GetFileNameWithoutExtend(t1);

			//Assert
			//The object has a new name
			Assert.AreEqual("ObjectPool", name);
		}

		[Test]
		public void EditorFileUtilsTest_GetFileExtendName_Name()
		{
			//Arrange
			string t1 = testPath + "ObjectPool.cs";

			//Act
			//Try to rename the GameObject
			string name = EditorFileUtils.GetFileExtendName(t1);

			//Assert
			//The object has a new name
			Assert.AreEqual(".cs", name);
		}

		[Test]
		public void EditorFileUtilsTest_GetDirPath_Name()
		{
			//Arrange
			string t1 = testPath;

			//Act
			//Try to rename the GameObject
			string name = EditorFileUtils.GetDirPath(t1);

			//Assert
			//The object has a new name
			Assert.AreEqual(t1, name);
		}
	}
}