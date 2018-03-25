/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class GuideCtrl : MonoBehaviour, ICanvasRaycastFilter
{
	[Header("高亮目标")] public Image Target;
	[Header("是否播放动画")]public bool ShowAnim;
	[Header("收缩时间")] public float ShrinkTime = 0.5f;

	protected Canvas _canvas;
	protected Material _material;
	protected string _shaderName;
	/// <summary>
	/// 区域范围缓存
	/// </summary>
	protected Vector3[] _corners = new Vector3[4];

	protected virtual void Awake()
	{
		//获取画布
		_canvas = GameObject.FindObjectOfType<Canvas>();
		if (_canvas == null)
		{
			Debug.LogError("There is not a Canvas!");
		}
		//材质初始化
		SetMatShader();
		_material = new Material(Shader.Find(_shaderName));
		GetComponent<Image>().material = _material;
		InitData();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
			Play();
		PlayShrinkAnim();
	}

	#if UNITY_EDITOR
	void OnGUI()
	{
		if (GUILayout.Button("Preview Play",GUILayout.Width(200),GUILayout.Height(100)))
		{
			Play();
		}
	}
	#endif

	protected virtual void SetMatShader()
	{
		_shaderName = "UI/Default";
	}
	
	protected virtual void InitData () 
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
