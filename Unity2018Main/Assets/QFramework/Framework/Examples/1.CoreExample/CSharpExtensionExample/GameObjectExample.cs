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
	/// CEGO EXAMPLE:GameObject 链式调用支持 
	/// </summary>
	public class GameObjectExample : MonoBehaviour
	{
		private void Start()
		{
			gameObject
				// 1. gameObject.SetActive(true)
				.Show()
				// 2. gameObject.SetActive(false)
				.Hide()
				// 3. gameObject.name = "Yeah" (这是UnityEngine.Object的API)
				.Name("Yeah")
				// 4. gameObject.layer = 10
				.Layer(0)
				// 5. gameObject.layer = LayerMask.NameToLayer("Default);
				.Layer("Default")
				// 6. Destroy(gameObject) (这是UnityEngine.Object的API)
				.DestroySelf();

			// 这里到会断掉，因为GameObject销毁之后就不希望再有操作了
			gameObject
				// 7. if (gameObject) Destroy(gameObject)
				.DestroySelfGracefully();


			GameObject instantiatedObj = null;

			gameObject
				// 8. Destroy(gameObject,1.5f)
				.DestroySelfAfterDelay(1.5f)
				// 9. if (gameObject) Destroy(gameObject,1.5f)
				.DestroySelfAfterDelayGracefully(1.5f)
				// 10. instantiatedObj = Instantiate(gameObject)
				.ApplySelfTo(selfObj => instantiatedObj = selfObj.Instantiate());

            Debug.Log(instantiatedObj);

			#region 通过MonoBehaviour去调用GameObject相关的API

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

			this
				// 7. if(this != null && this.gameObject) Destroy(this.gameObject)
				.DestroyGameObjGracefully();

			this
				// 8. this.gameObject.DestroySelfAfterDelay(1.5f)
				.DestroyGameObjAfterDelay(1.5f)
				// 9. if (this && this.gameObject0 this.gameObject.DestroySelfAfterDelay(1.5f)
				.DestroyGameObjAfterDelayGracefully(1.5f)
				// 10. instantiatedObj = Instantiate(this.gameObject)
				.ApplySelfTo(selfScript => instantiatedObj = selfScript.gameObject.Instantiate());

			#endregion


			#region 也可以使用Transform,因为Transform继承了Component,而Core里的所有的链式扩展都默认支持了Component

			transform
				// 1. transform.gameObject.SetActive(true)
				.Show()
				// 2. transform.gameObject.SetActive(false)
				.Hide()
				// 3. transform.name = "Yeah" (这是UnityEngine.Object的API)
				.Name("Yeah")
				// 4. transform.gameObject.layer = 10
				.Layer(0)
				// 5. transform.gameObject.layer = LayerMask.NameToLayer("Default);
				.Layer("Default")
				// 6. Destroy(transform.gameObject) (这是UnityEngine.Object的API)
				.DestroyGameObj();

			// 这里到会断掉，因为GameObject销毁之后就不希望再有操作了
			transform
				// 7. if (transform && gameObject) Destroy(transform.gameObject)
				.DestroyGameObjGracefully();

			transform
				// 8. Destroy(transform.gameObject,1.5f)
				.DestroyGameObjAfterDelay(1.5f)
				// 9. if (transform.gameObject) Destroy(gameObject,1.5f)
				.DestroyGameObjAfterDelayGracefully(1.5f)
				// 10. instantiatedTrans = Instantiate(transform.gameObject)
				.ApplySelfTo(selfTrans => instantiatedObj = selfTrans.gameObject.Instantiate());

			#endregion
		}
	}
}