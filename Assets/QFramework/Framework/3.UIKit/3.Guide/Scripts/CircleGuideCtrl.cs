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

namespace QFramework
{
	/// <summary>
	/// 圆形遮罩引导
	/// </summary>
	public class CircleGuideCtrl : GuideCtrl
	{
		/// <summary>
		/// 镂空区域半径
		/// </summary>
		private float mRadius;
		/// <summary>
		/// 当前高亮区域的半径
		/// </summary>
		private float mCurrentRadius = 0f;
		
		protected override void SetMatShader()
		{
			mShaderName = "UI/Guide/CircleGuide";
		}

		protected override void InitData()
		{
			Target.rectTransform.GetWorldCorners(_corners);
			//获取最终高亮区域半径
			mRadius = Vector2.Distance(World2CanvasPos(mCanvas, _corners[0]), World2CanvasPos(mCanvas, _corners[3])) / 2f;
			//计算圆心
			float x = _corners[0].x + (_corners[3].x - _corners[0].x) / 2f;
			float y = _corners[0].y + (_corners[1].y - _corners[0].y) / 2f;
			Vector3 centerWorld = new Vector3(x, y, 0);
			Vector2 center = World2CanvasPos(mCanvas, centerWorld);
			//Apply 设置数据到shader中
			Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
			mMaterial.SetVector("_Center", centerMat);
			//计算当前高亮显示区域半径
			RectTransform canvasRectTransform = mCanvas.transform as RectTransform;
			canvasRectTransform.GetWorldCorners(_corners);
			foreach (Vector3 corner in _corners)
			{
				mCurrentRadius = Mathf.Max(Vector3.Distance(World2CanvasPos(mCanvas, corner), corner), mCurrentRadius);
			}
			float initRadius = ShowAnim ? mCurrentRadius : mRadius;
			mMaterial.SetFloat("_Slider", initRadius);
		}

		private float shrinkVelocity = 0f;
		protected override void PlayShrinkAnim()
		{
			if (!ShowAnim)
				return;
			float value = Mathf.SmoothDamp(mCurrentRadius, mRadius, ref shrinkVelocity, ShrinkTime);
			if (!Mathf.Approximately(value, mCurrentRadius))
			{
				mCurrentRadius = value;
				mMaterial.SetFloat("_Slider", mCurrentRadius);
			}
		}
	}
}
