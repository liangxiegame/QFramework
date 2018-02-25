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
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;

	public class ExtensionExample : MonoBehaviour
	{
		private void Start()
		{
			QuickStart();
			CSharpExtensions();
			UnityExtensions();
		}

		private void QuickStart()
		{
			gameObject
				// 1. gameObject.SetActive(true)
				.Show()
				// 2. gameObject.SetActive(false)
				.Hide()
				// 3. gameObject.name = "Yeah" (this is UnityEngine.Object's API)
				.Name("Yeah")
				// 4. gameObject.layer = 10
				.Layer(0)
				// 5. gameObject.layer = LayerMask.NameToLayer("Default);
				.Layer("Default")
				// 6. Destroy(gameObject) (this is UnityEngine.Object's API)
				.DestroySelf();

			this
				// 1. this.gameObject.Show()
				.Show()
				// 2. this.gameObject.Hide()
				.Hide()
				// 3. this.gameObject.Name("Yeah")
				.Name("Yeah")
				// 4. gameObject.layer = 10
				.Layer(0)
				// 5. gameObject.layer = LayerMask.NameToLayer("Default);
				.Layer("Default")
				// 6. Destroy(this.gameObject)
				.DestroyGameObj();
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

		private void UnityExtensions()
		{
			#region Enable,Disable

			var component = gameObject.GetComponent<MonoBehaviour>();

			component.Enable(); // component.enabled = true
			component.Disable(); // component.enabled = false

			#endregion

			#region CaptureCamera

			var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
			Debug.Log(screenshotTexture2D.width);
			#endregion

			#region Color

			var color = "#C5563CFF".HtmlStringToColor();
			Debug.Log(color);
			
			#endregion

			#region GameObject

			var boxCollider = gameObject.AddComponent<BoxCollider>();

			gameObject.Show(); // gameObject.SetActive(true)
			this.Show(); // this.gameObject.SetActive(true)
			boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
			transform.Show(); // transform.gameObject.SetActive(true)

			gameObject.Hide(); // gameObject.SetActive(false)
			this.Hide(); // this.gameObject.SetActive(false)
			boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
			transform.Hide(); // transform.gameObject.SetActive(false)

			this.DestroyGameObj();
			boxCollider.DestroyGameObj();
			transform.DestroyGameObj();

			this.DestroyGameObjGracefully();
			boxCollider.DestroyGameObjGracefully();
			transform.DestroyGameObjGracefully();

			this.DestroyGameObjAfterDelay(1.0f);
			boxCollider.DestroyGameObjAfterDelay(1.0f);
			transform.DestroyGameObjAfterDelay(1.0f);

			this.DestroyGameObjAfterDelayGracefully(1.0f);
			boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
			transform.DestroyGameObjAfterDelayGracefully(1.0f);

			gameObject.Layer(0);
			this.Layer(0);
			boxCollider.Layer(0);
			transform.Layer(0);

			gameObject.Layer("Default");
			this.Layer("Default");
			boxCollider.Layer("Default");
			transform.Layer("Default");

			#endregion

			#region Graphic

			var image = gameObject.AddComponent<Image>();
			var rawImage = gameObject.AddComponent<RawImage>();

			// image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
			image.ColorAlpha(1.0f);
			rawImage.ColorAlpha(1.0f);

			#endregion

			#region Image

			var image1 = gameObject.AddComponent<Image>();

			image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;

			#endregion


			#region Object

			gameObject.Instantiate()
				.Name("ExtensionExample")
				.DestroySelf();

			gameObject.Instantiate()
				.DestroySelfGracefully();

			gameObject.Instantiate()
				.DestroySelfAfterDelay(1.0f);

			gameObject.Instantiate()
				.DestroySelfAfterDelayGracefully(1.0f);

			gameObject
				.ApplySelfTo(selfObj => Debug.Log(selfObj.name))
				.Name("TestObj")
				.ApplySelfTo(selfObj => Debug.Log(selfObj.name))
				.Name("ExtensionExample")
				.DontDestroyOnLoad();

			#endregion


			#region RectTransform



			#endregion

			#region Selectable



			#endregion

			#region Toggle


			#endregion

			#region Transform

			transform
				.Parent(null)
				.LocalIdentity()
				.LocalPositionIdentity()
				.LocalRotationIdentity()
				.LocalScaleIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPosition(0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				.LocalRotation(Quaternion.identity)
				.LocalScale(Vector3.one)
				.LocalScaleX(1.0f)
				.LocalScaleY(1.0f)
				.Identity()
				.PositionIdentity()
				.RotationIdentity()
				.Position(Vector3.zero)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				.Rotation(Quaternion.identity)
				.DestroyAllChild()
				.AsLastSibling()
				.AsFirstSibling()
				.SiblingIndex(0);

			this
				.Parent(null)
				.LocalIdentity()
				.LocalPositionIdentity()
				.LocalRotationIdentity()
				.LocalScaleIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPosition(0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				.LocalRotation(Quaternion.identity)
				.LocalScale(Vector3.one)
				.LocalScaleX(1.0f)
				.LocalScaleY(1.0f)
				.Identity()
				.PositionIdentity()
				.RotationIdentity()
				.Position(Vector3.zero)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				.Rotation(Quaternion.identity)
				.DestroyAllChild()
				.AsLastSibling()
				.AsFirstSibling()
				.SiblingIndex(0);

			#endregion

			#region UnityAction

			UnityAction action = () => { };
			UnityAction<int> actionWithInt = num => { };
			UnityAction<int, string> actionWithIntString = (num, str) => { };

			action.InvokeGracefully();
			actionWithInt.InvokeGracefully(1);
			actionWithIntString.InvokeGracefully(1, "str");

			#endregion
		}
	}
}