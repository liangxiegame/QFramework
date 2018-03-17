/****************************************************************************
 * Copyright (c) 2017 yuanhuibin@putao.com
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
	using NUnit.Framework;

	public class PathHelperTest
	{
		[Test]
		public void PathHelperTest_FileNameWithoutSuffix()
		{
			//Arrange
			string nameWhithoutSuffix = "test.md";

			//Act
			//Try to rename the GameObject
			string fileName1 = "test";
			string fileName2 = PathHelper.FileNameWithoutSuffix(nameWhithoutSuffix);
			//Assert
			//The object has a new name
			Assert.AreEqual(fileName1, fileName2);
		}

		[Test]
		public void PathHelperTest_FullAssetPath2Name()
		{
			//Arrange
			string nameFullPath = "Assets/test.md";

			//Act
			//Try to rename the GameObject
			string fileName1 = "test";
			string fileName2 = PathHelper.FullAssetPath2Name(nameFullPath);
			//Assert
			//The object has a new name
			Assert.AreEqual(fileName1, fileName2);
		}
	}
}