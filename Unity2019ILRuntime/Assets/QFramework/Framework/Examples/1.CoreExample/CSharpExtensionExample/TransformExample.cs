/****************************************************************************
 * Copyright (c) 2017 liangxie
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

using UnityEngine;

namespace QFramework.Example
{
	/// <summary>
	/// CETR EXAMPLE:Transform 链式调用支持 
	/// </summary>
	public class TransformExample : MonoBehaviour
	{
		private void Start()
		{
			transform
				// 1. transform.SetParent(null)
				// .Parent(null)
				// 2. transform.localPosition = Vector3.zero;
				//    transform.localRotation = Quaternion.identity;
				//    transform.localScale = Vector3.one;
				.LocalIdentity()
				// 3. transform.localPosition = Vector3.zero;
				.LocalPositionIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				// 4. transform.localRotation = Quaternion.identity;
				.LocalRotationIdentity()
				.LocalRotation(Quaternion.identity)
				// 5. transform.localScale = Vector3.one;
				.LocalScaleIdentity()
				.LocalScale(Vector3.one)
				.LocalScaleX(1)
				.LocalScaleY(1)
				.LocalScale(1, 1)
				.LocalScale(1, 1, 1)
				// 6. transform.position = Vector3.zero;
				//    transform.rotation = Quaternion.identity;
				//    transform.localScale = Vector3.one;
				.Identity()
				// 7. transform.position = Vector3.zero
				.PositionIdentity()
				.Position(Vector3.zero)
				.Position(0, 0, 0)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				// 8. transform.rotation = Quaternion.identity;
				.RotationIdentity()
				.Rotation(Quaternion.identity)
				// 9. 删除所有子节点
				.DestroyChildren()
				// 10. transform.SetAsFirstSibling();
				//     transform.SetAsLastSibling();
				//     transform.SetSiblingIndex(1);
				.AsFirstSibling()
				.AsLastSibling()
				.SiblingIndex(1);

			var textTrans = transform.FindByPath("BtnOK.Text");
			textTrans = transform.SeekTrans("Text");
			Debug.Log(textTrans);

		}
	}
}