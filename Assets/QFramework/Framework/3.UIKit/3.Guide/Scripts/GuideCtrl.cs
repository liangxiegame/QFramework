/****************************************************************************
 * Copyright (c) 2018.3 dtknowlove@qq.com
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
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
	[RequireComponent(typeof(Image))]
	public abstract class GuideCtrl : MonoBehaviour, ICanvasRaycastFilter
	{
		[Header("高亮目标")] public Image Target;
		[Header("是否播放动画")] public bool ShowAnim;
		[Header("收缩时间")] public float ShrinkTime = 0.5f;

		protected Canvas mCanvas;
		protected Material mMaterial;
		protected string mShaderName;

		/// <summary>
		/// 区域范围缓存
		/// </summary>
		protected Vector3[] _corners = new Vector3[4];

		protected virtual void Awake()
		{
			//获取画布
			mCanvas = GameObject.FindObjectOfType<Canvas>();
			if (mCanvas == null)
			{
				Debug.LogError("There is not a Canvas!");
			}
			//材质初始化
			SetMatShader();
			mMaterial = new Material(Shader.Find(mShaderName));
			GetComponent<Image>().material = mMaterial;
			InitData();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
				Play();
			PlayShrinkAnim();
		}

#if UNITY_EDITOR
		void OnGUI()
		{
			if (GUILayout.Button("Preview Play", GUILayout.Width(200), GUILayout.Height(100)))
			{
				Play();
			}
		}
#endif

		protected virtual void SetMatShader()
		{
			mShaderName = "UI/Default";
		}

		protected virtual void InitData()
		{

		}

		protected virtual void PlayShrinkAnim()
		{

		}

		/// <summary>
		/// 世界坐标向画布坐标转换
		/// </summary>
		/// <param name="canvas">画布</param>
		/// <param name="world">世界坐标</param>
		/// <returns>返回画布上的二维坐标</returns>
		protected Vector2 World2CanvasPos(Canvas canvas, Vector3 worldPos)
		{
			Vector2 position;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
				worldPos, canvas.GetComponent<Camera>(), out position);
			return position;
		}

		public virtual void Play()
		{
			ShowAnim = true;
			InitData();
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			if (Target == null)
				return true;

			return !RectTransformUtility.RectangleContainsScreenPoint(Target.rectTransform, sp, eventCamera);
		}
	}
}
