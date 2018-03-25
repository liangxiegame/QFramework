/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/
 
using UnityEngine;

namespace QFramework.Guide
{
	/// <summary>
	/// 圆形遮罩引导
	/// </summary>
	public class CircleGuideCtrl : GuideCtrl
	{
		/// <summary>
		/// 镂空区域半径
		/// </summary>
		private float _radius;
		/// <summary>
		/// 当前高亮区域的半径
		/// </summary>
		private float _currentRadius = 0f;
		
		protected override void SetMatShader()
		{
			_shaderName = "UI/Guide/CircleGuide";
		}

		protected override void InitData()
		{
			Target.rectTransform.GetWorldCorners(_corners);
			//获取最终高亮区域半径
			_radius = Vector2.Distance(World2CanvasPos(_canvas, _corners[0]), World2CanvasPos(_canvas, _corners[3])) / 2f;
			//计算圆心
			float x = _corners[0].x + (_corners[3].x - _corners[0].x) / 2f;
			float y = _corners[0].y + (_corners[1].y - _corners[0].y) / 2f;
			Vector3 centerWorld = new Vector3(x, y, 0);
			Vector2 center = World2CanvasPos(_canvas, centerWorld);
			//Apply 设置数据到shader中
			Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
			_material.SetVector("_Center", centerMat);
			//计算当前高亮显示区域半径
			RectTransform canvasRectTransform = _canvas.transform as RectTransform;
			canvasRectTransform.GetWorldCorners(_corners);
			foreach (Vector3 corner in _corners)
			{
				_currentRadius = Mathf.Max(Vector3.Distance(World2CanvasPos(_canvas, corner), corner), _currentRadius);
			}
			float initRadius = ShowAnim ? _currentRadius : _radius;
			_material.SetFloat("_Slider", initRadius);
		}

		private float _shrinkVelocity = 0f;
		protected override void PlayShrinkAnim()
		{
			if (!ShowAnim)
				return;
			float value = Mathf.SmoothDamp(_currentRadius, _radius, ref _shrinkVelocity, ShrinkTime);
			if (!Mathf.Approximately(value, _currentRadius))
			{
				_currentRadius = value;
				_material.SetFloat("_Slider", _currentRadius);
			}
		}
	}
}
