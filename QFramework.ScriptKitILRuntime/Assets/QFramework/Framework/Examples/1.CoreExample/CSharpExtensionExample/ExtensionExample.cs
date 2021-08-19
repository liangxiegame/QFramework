/****************************************************************************
 * Copyright (c) 2018 liangxie
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

namespace QFramework.Example
{
	using UnityEngine;

	public class ExtensionExample : MonoBehaviour
	{
		private void Start()
		{
			QuickStart();
			CSharpExtensions();
			UnityExtensions();
		}

		private static void QuickStart()
		{
			// traditional style
			var playerPrefab = Resources.Load<GameObject>("no prefab don't run");
			var playerObj = Instantiate(playerPrefab);

			playerObj.transform.SetParent(null);
			playerObj.transform.localRotation = Quaternion.identity;
			playerObj.transform.localPosition = Vector3.left;
			playerObj.transform.localScale = Vector3.one;
			playerObj.layer = 1;
			playerObj.layer = LayerMask.GetMask("Default");

			Debug.Log("playerPrefab instantiated");

			// Extension's Style,same as above 
			Resources.Load<GameObject>("playerPrefab")
				.Instantiate()
				.transform
				// .Parent(null)
				.LocalRotationIdentity()
				.LocalPosition(Vector3.left)
				.LocalScaleIdentity()
				.Layer(1)
				.Layer("Default")
				.ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
		}

		private static void CSharpExtensions()
		{
			// ClassExtention.Example();
			// FuncOrActionOrEventExtension.Example();
			// GenericExtention.Example();
//			IEnumerableExtension.Example();
//			IOExtension.Example();
			OOPExtension.Example();
			ReflectionExtension.Example();
			StringExtention.Example();
		}

		private static void UnityExtensions()
		{
			BehaviourExtension.Example();
			CameraExtension.Example();
			ColorExtension.Example();
			QFramework.GameObjectExtension.Example();
			GraphicExtension.Example();
			ImageExtension.Example();
			ObjectExtension.Example();
			UnityActionExtension.Example();
			
			#region RectTransform

			#endregion

			#region Selectable

			#endregion

			#region Toggle

			#endregion
		}
	}
}