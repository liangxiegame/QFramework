/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
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


namespace QFramework.Test.Core
{
	using UnityEngine;
	using UnityEditor;
	using NUnit.Framework;

	public class IOUtilsTest
	{
		public static string testPath = Application.dataPath + "/PTUGame/PTFramework/Core/Pool/";

		[Test]
		public void IOUtilsTest_GetDirSubFilePathList_Count()
		{
			//Arrange
			int count = 0;

			//Act
			//Try to rename the GameObject
			count = IOExtension.GetDirSubFilePathList(testPath,true,".cs").Count;

			//Assert
			//The object has a new name
			Assert.AreEqual(4,count);
		}
		
		[Test]
		public void IOUtilsTest_GetDirSubDirNameList_Count()
		{
			//Arrange
			int count = 0;

			//Act
			//Try to rename the GameObject
			count = IOExtension.GetDirSubDirNameList(testPath).Count;

			//Assert
			//The object has a new name
			Assert.AreEqual(1,count);
		}
	}
}
