
//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

//#define SHOW_HIDDEN_OBJECTS

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is an internally-created script used by the UI system. You shouldn't be attaching it manually.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();
	static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	[System.Obsolete("Use UIDrawCall.activeList")]
	static public BetterList<UIDrawCall> list { get { return mActiveList; } }

	/// <summary>
	/// List of active draw calls.
	/// </summary>

	static public BetterList<UIDrawCall> activeList { get { return mActiveList; } }

	/// <summary>
	/// List of inactive draw calls. Only used at run-time in order to avoid object creation/destruction.
	/// </summary>

	static public BetterList<UIDrawCall> inactiveList { get { return mInactiveList; } }

	public enum Clipping : int
	{
		None = 0,
		TextureMask = 1,			// Clipped using a texture rather than math
		SoftClip = 3,				// Alpha-based clipping with a softened edge
		ConstrainButDontClip = 4,	// No actual clipping, but does have an area
	}

	[HideInInspector][System.NonSerialized] public int widgetCount = 0;
	[HideInInspector][System.NonSerialized] public int depthStart = int.MaxValue;
	[HideInInspector][System.NonSerialized] public int depthEnd = int.MinValue;
	[HideInInspector][System.NonSerialized] public UIPanel manager;
	[HideInInspector][System.NonSerialized] public UIPanel panel;
	[HideInInspector][System.NonSerialized] public Texture2D clipTexture;
	[HideInInspector][System.NonSerialized] public bool alwaysOnScreen = false;
	[HideInInspector][System.NonSerialized] public BetterList<Vector3> verts = new BetterList<Vector3>();
	[HideInInspector][System.NonSerialized] public BetterList<Vector3> norms = new BetterList<Vector3>();
	[HideInInspector][System.NonSerialized] public BetterList<Vector4> tans = new BetterList<Vector4>();
	[HideInInspector][System.NonSerialized] public BetterList<Vector2> uvs = new BetterList<Vector2>();
	[HideInInspector][System.NonSerialized] public BetterList<Color32> cols = new BetterList<Color32>();

	Material		mMaterial;		// Material used by this draw call
	Texture			mTexture;		// Main texture used by the material
	Shader			mShader;		// Shader used by the dynamically created material
	int				mClipCount = 0;	// Number of times the draw call's content is getting clipped
	Transform		mTrans;			// Cached transform
	Mesh			mMesh;			// First generated mesh
	MeshFilter		mFilter;		// Mesh filter for this draw call
	MeshRenderer	mRenderer;		// Mesh renderer for this screen
	Material		mDynamicMat;	// Instantiated material
	int[]			mIndices;		// Cached indices

	bool mRebuildMat = true;
	bool mLegacyShader = false;
	int mRenderQueue = 3000;
	int mTriangles = 0;

	/// <summary>
	/// Whether the draw call has changed recently.
	/// </summary>

	[System.NonSerialized]
	public bool isDirty = false;

	[System.NonSerialized]
	bool mTextureClip = false;

	public delegate void OnRenderCallback (Material mat);

	/// <summary>
	/// Callback that will be triggered at OnWillRenderObject() time.
	/// </summary>

	public OnRenderCallback onRender;

	/// <summary>
	/// Render queue used by the draw call.
	/// </summary>

	public int renderQueue
	{
		get
		{
			return mRenderQueue;
		}
		set
		{
			if (mRenderQueue != value)
			{
				mRenderQueue = value;

				if (mDynamicMat != null)
				{
					mDynamicMat.renderQueue = value;
#if UNITY_EDITOR
					if (mRenderer != null) mRenderer.enabled = isActive;
#endif
				}
			}
		}
	}

	/// <summary>
	/// Renderer's sorting order, to be used with Unity's 2D system.
	/// </summary>

	public int sortingOrder
	{
		get { return (mRenderer != null) ? mRenderer.sortingOrder : 0; }
		set { if (mRenderer != null && mRenderer.sortingOrder != value) mRenderer.sortingOrder = value; }
	}

	/// <summary>
	/// Final render queue used to draw the draw call's geometry.
	/// </summary>

	public int finalRenderQueue
	{
		get
		{
			return (mDynamicMat != null) ? mDynamicMat.renderQueue : mRenderQueue;
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// Whether the draw call is currently active.
	/// </summary>

	public bool isActive
	{
		get
		{
			return mActive;
		}
		set
		{
			if (mActive != value)
			{
				mActive = value;

				if (mRenderer != null)
				{
					mRenderer.enabled = value;
					NGUITools.SetDirty(gameObject);
				}
			}
		}
	}
	bool mActive = true;
#endif

	/// <summary>
	/// Transform is cached for speed and efficiency.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	/// <summary>
	/// Material used by this screen.
	/// </summary>

	public Material baseMaterial
	{
		get
		{
			return mMaterial;
		}
		set
		{
			if (mMaterial != value)
			{
				mMaterial = value;
				mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// Dynamically created material used by the draw call to actually draw the geometry.
	/// </summary>

	public Material dynamicMaterial { get { return mDynamicMat; } }

	/// <summary>
	/// Texture used by the material.
	/// </summary>

	public Texture mainTexture
	{
		get
		{
			return mTexture;
		}
		set
		{
			mTexture = value;
			if (mDynamicMat != null) mDynamicMat.mainTexture = value;
		}
	}

	/// <summary>
	/// Shader used by the material.
	/// </summary>

	public Shader shader
	{
		get
		{
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// The number of triangles in this draw call.
	/// </summary>

	public int triangles { get { return (mMesh != null) ? mTriangles : 0; } }

	/// <summary>
	/// Whether the draw call is currently using a clipped shader.
	/// </summary>

	public bool isClipped { get { return mClipCount != 0; } }

	/// <summary>
	/// Create an appropriate material for the draw call.
	/// </summary>

	void CreateMaterial ()
	{
		mTextureClip = false;
		mLegacyShader = false;
		mClipCount = (panel != null) ? panel.clipCount : 0;

		string shaderName = (mShader != null) ? mShader.name :
			((mMaterial != null) ? mMaterial.shader.name : "Unlit/Transparent Colored");

		// Figure out the normal shader's name
		shaderName = shaderName.Replace("GUI/Text Shader", "Unlit/Text");

		if (shaderName.Length > 2)
		{
			if (shaderName[shaderName.Length - 2] == ' ')
			{
				int index = shaderName[shaderName.Length - 1];
				if (index > '0' && index <= '9') shaderName = shaderName.Substring(0, shaderName.Length - 2);
			}
		}

		if (shaderName.StartsWith("Hidden/"))
			shaderName = shaderName.Substring(7);

		// Legacy functionality
		const string soft = " (SoftClip)";
		shaderName = shaderName.Replace(soft, "");

		const string textureClip = " (TextureClip)";
		shaderName = shaderName.Replace(textureClip, "");

		if (panel != null && panel.clipping == Clipping.TextureMask)
		{
			mTextureClip = true;
			shader = Shader.Find("Hidden/" + shaderName + textureClip);
		}
		else if (mClipCount != 0)
		{
			shader = Shader.Find("Hidden/" + shaderName + " " + mClipCount);
			if (shader == null) shader = Shader.Find(shaderName + " " + mClipCount);

			// Legacy functionality
			if (shader == null && mClipCount == 1)
			{
				mLegacyShader = true;
				shader = Shader.Find(shaderName + soft);
			}
		}
		else shader = Shader.Find(shaderName);

		// Always fallback to the default shader
		if (shader == null) shader = Shader.Find("Unlit/Transparent Colored");

		if (mMaterial != null)
		{
			mDynamicMat = new Material(mMaterial);
			mDynamicMat.name = "[NGUI] " + mMaterial.name;
			mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
			mDynamicMat.CopyPropertiesFromMaterial(mMaterial);
#if !UNITY_FLASH
			string[] keywords = mMaterial.shaderKeywords;
			for (int i = 0; i < keywords.Length; ++i)
				mDynamicMat.EnableKeyword(keywords[i]);
#endif
			// If there is a valid shader, assign it to the custom material
			if (shader != null)
			{
				mDynamicMat.shader = shader;
			}
			else if (mClipCount != 0)
			{
				Debug.LogError(shaderName + " shader doesn't have a clipped shader version for " + mClipCount + " clip regions");
			}
		}
		else
		{
			mDynamicMat = new Material(shader);
			mDynamicMat.name = "[NGUI] " + shader.name;
			mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
		}
	}

	/// <summary>
	/// Rebuild the draw call's material.
	/// </summary>

	Material RebuildMaterial ()
	{
		// Destroy the old material
		NGUITools.DestroyImmediate(mDynamicMat);

		// Create a new material
		CreateMaterial();
		mDynamicMat.renderQueue = mRenderQueue;

		// Assign the main texture
		if (mTexture != null) mDynamicMat.mainTexture = mTexture;

		// Update the renderer
		if (mRenderer != null) mRenderer.sharedMaterials = new Material[] { mDynamicMat };
		return mDynamicMat;
	}

	/// <summary>
	/// Update the renderer's materials.
	/// </summary>

	void UpdateMaterials ()
	{
		if (panel == null) return;

		// If clipping should be used, we need to find a replacement shader
		if (mRebuildMat || mDynamicMat == null || mClipCount != panel.clipCount || mTextureClip != (panel.clipping == Clipping.TextureMask))
		{
			RebuildMaterial();
			mRebuildMat = false;
		}
		else if (mRenderer.sharedMaterial != mDynamicMat)
		{
#if UNITY_EDITOR
			Debug.LogError("Hmm... This point got hit!");
#endif
			mRenderer.sharedMaterials = new Material[] { mDynamicMat };
		}
	}

	/// <summary>
	/// Set the draw call's geometry.
	/// </summary>

	public void UpdateGeometry (int widgetCount)
	{
		this.widgetCount = widgetCount;
		int count = verts.size;

		// Safety check to ensure we get valid values
		if (count > 0 && (count == uvs.size && count == cols.size) && (count % 4) == 0)
		{
			// Cache all components
			if (mFilter == null) mFilter = gameObject.GetComponent<MeshFilter>();
			if (mFilter == null) mFilter = gameObject.AddComponent<MeshFilter>();

			if (verts.size < 65000)
			{
				// Populate the index buffer
				int indexCount = (count >> 1) * 3;
				bool setIndices = (mIndices == null || mIndices.Length != indexCount);

				// Create the mesh
				if (mMesh == null)
				{
					mMesh = new Mesh();
					mMesh.hideFlags = HideFlags.DontSave;
					mMesh.name = (mMaterial != null) ? "[NGUI] " + mMaterial.name : "[NGUI] Mesh";
					mMesh.MarkDynamic();
					setIndices = true;
				}
#if !UNITY_FLASH
				// If the buffer length doesn't match, we need to trim all buffers
				bool trim = (uvs.buffer.Length != verts.buffer.Length) ||
					(cols.buffer.Length != verts.buffer.Length) ||
					(norms.buffer != null && norms.buffer.Length != verts.buffer.Length) ||
					(tans.buffer != null && tans.buffer.Length != verts.buffer.Length);

				// Non-automatic render queues rely on Z position, so it's a good idea to trim everything
				if (!trim && panel != null && panel.renderQueue != UIPanel.RenderQueue.Automatic)
					trim = (mMesh == null || mMesh.vertexCount != verts.buffer.Length);

				// NOTE: Apparently there is a bug with Adreno devices:
				// http://www.tasharen.com/forum/index.php?topic=8415.0
#if !UNITY_ANDROID
				// If the number of vertices in the buffer is less than half of the full buffer, trim it
				if (!trim && (verts.size << 1) < verts.buffer.Length) trim = true;
#endif
				mTriangles = (verts.size >> 1);

				if (trim || verts.buffer.Length > 65000)
				{
					if (trim || mMesh.vertexCount != verts.size)
					{
						mMesh.Clear();
						setIndices = true;
					}

					mMesh.vertices = verts.ToArray();
					mMesh.uv = uvs.ToArray();
					mMesh.colors32 = cols.ToArray();

					if (norms != null) mMesh.normals = norms.ToArray();
					if (tans != null) mMesh.tangents = tans.ToArray();
				}
				else
				{
					if (mMesh.vertexCount != verts.buffer.Length)
					{
						mMesh.Clear();
						setIndices = true;
					}

					mMesh.vertices = verts.buffer;
					mMesh.uv = uvs.buffer;
					mMesh.colors32 = cols.buffer;

					if (norms != null) mMesh.normals = norms.buffer;
					if (tans != null) mMesh.tangents = tans.buffer;
				}
#else
				mTriangles = (verts.size >> 1);

				if (mMesh.vertexCount != verts.size)
				{
					mMesh.Clear();
					setIndices = true;
				}

				mMesh.vertices = verts.ToArray();
				mMesh.uv = uvs.ToArray();
				mMesh.colors32 = cols.ToArray();

				if (norms != null) mMesh.normals = norms.ToArray();
				if (tans != null) mMesh.tangents = tans.ToArray();
#endif
				if (setIndices)
				{
					mIndices = GenerateCachedIndexBuffer(count, indexCount);
					mMesh.triangles = mIndices;
				}

#if !UNITY_FLASH
				if (trim || !alwaysOnScreen)
#endif
					mMesh.RecalculateBounds();

				mFilter.mesh = mMesh;
			}
			else
			{
				mTriangles = 0;
				if (mFilter.mesh != null) mFilter.mesh.Clear();
				Debug.LogError("Too many vertices on one panel: " + verts.size);
			}

			if (mRenderer == null) mRenderer = gameObject.GetComponent<MeshRenderer>();

			if (mRenderer == null)
			{
				mRenderer = gameObject.AddComponent<MeshRenderer>();
#if UNITY_EDITOR
				mRenderer.enabled = isActive;
#endif
			}
			UpdateMaterials();
		}
		else
		{
			if (mFilter.mesh != null) mFilter.mesh.Clear();
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + count);
		}

		verts.Clear();
		uvs.Clear();
		cols.Clear();
		norms.Clear();
		tans.Clear();
	}

	const int maxIndexBufferCache = 10;

#if UNITY_FLASH
	List<int[]> mCache = new List<int[]>(maxIndexBufferCache);
#else
	static List<int[]> mCache = new List<int[]>(maxIndexBufferCache);
#endif

	/// <summary>
	/// Generates a new index buffer for the specified number of vertices (or reuses an existing one).
	/// </summary>

	int[] GenerateCachedIndexBuffer (int vertexCount, int indexCount)
	{
		for (int i = 0, imax = mCache.Count; i < imax; ++i)
		{
			int[] ids = mCache[i];
			if (ids != null && ids.Length == indexCount)
				return ids;
		}

		int[] rv = new int[indexCount];
		int index = 0;

		for (int i = 0; i < vertexCount; i += 4)
		{
			rv[index++] = i;
			rv[index++] = i + 1;
			rv[index++] = i + 2;

			rv[index++] = i + 2;
			rv[index++] = i + 3;
			rv[index++] = i;
		}

		if (mCache.Count > maxIndexBufferCache) mCache.RemoveAt(0);
		mCache.Add(rv);
		return rv;
	}

	/// <summary>
	/// This function is called when it's clear that the object will be rendered.
	/// We want to set the shader used by the material, creating a copy of the material in the process.
	/// We also want to update the material's properties before it's actually used.
	/// </summary>

	void OnWillRenderObject ()
	{
		UpdateMaterials();

		if (onRender != null) onRender(mDynamicMat ?? mMaterial);
		if (mDynamicMat == null || mClipCount == 0) return;

		if (mTextureClip)
		{
			Vector4 cr = panel.drawCallClipRange;
			Vector2 soft = panel.clipSoftness;

			Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
			if (soft.x > 0f) sharpness.x = cr.z / soft.x;
			if (soft.y > 0f) sharpness.y = cr.w / soft.y;

			mDynamicMat.SetVector(ClipRange[0], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
			mDynamicMat.SetTexture("_ClipTex", clipTexture);
		}
		else if (!mLegacyShader)
		{
			UIPanel currentPanel = panel;

			for (int i = 0; currentPanel != null; )
			{
				if (currentPanel.hasClipping)
				{
					float angle = 0f;
					Vector4 cr = currentPanel.drawCallClipRange;

					// Clipping regions past the first one need additional math
					if (currentPanel != panel)
					{
						Vector3 pos = currentPanel.cachedTransform.InverseTransformPoint(panel.cachedTransform.position);
						cr.x -= pos.x;
						cr.y -= pos.y;

						Vector3 v0 = panel.cachedTransform.rotation.eulerAngles;
						Vector3 v1 = currentPanel.cachedTransform.rotation.eulerAngles;
						Vector3 diff = v1 - v0;

						diff.x = NGUIMath.WrapAngle(diff.x);
						diff.y = NGUIMath.WrapAngle(diff.y);
						diff.z = NGUIMath.WrapAngle(diff.z);

						if (Mathf.Abs(diff.x) > 0.001f || Mathf.Abs(diff.y) > 0.001f)
							Debug.LogWarning("Panel can only be clipped properly if X and Y rotation is left at 0", panel);

						angle = diff.z;
					}

					// Pass the clipping parameters to the shader
					SetClipping(i++, cr, currentPanel.clipSoftness, angle);
				}
				currentPanel = currentPanel.parentPanel;
			}
		}
		else // Legacy functionality
		{
			Vector2 soft = panel.clipSoftness;
			Vector4 cr = panel.drawCallClipRange;
			Vector2 v0 = new Vector2(-cr.x / cr.z, -cr.y / cr.w);
			Vector2 v1 = new Vector2(1f / cr.z, 1f / cr.w);

			Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
			if (soft.x > 0f) sharpness.x = cr.z / soft.x;
			if (soft.y > 0f) sharpness.y = cr.w / soft.y;

			mDynamicMat.mainTextureOffset = v0;
			mDynamicMat.mainTextureScale = v1;
			mDynamicMat.SetVector("_ClipSharpness", sharpness);
		}
	}

	static int[] ClipRange = null;
	static int[] ClipArgs = null;

	/// <summary>
	/// Set the shader clipping parameters.
	/// </summary>

	void SetClipping (int index, Vector4 cr, Vector2 soft, float angle)
	{
		angle *= -Mathf.Deg2Rad;

		Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
		if (soft.x > 0f) sharpness.x = cr.z / soft.x;
		if (soft.y > 0f) sharpness.y = cr.w / soft.y;

		if (index < ClipRange.Length)
		{
			mDynamicMat.SetVector(ClipRange[index], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
			mDynamicMat.SetVector(ClipArgs[index], new Vector4(sharpness.x, sharpness.y, Mathf.Sin(angle), Mathf.Cos(angle)));
		}
	}

	/// <summary>
	/// Cache the property IDs.
	/// </summary>

	void Awake ()
	{
		if (ClipRange == null)
		{
			ClipRange = new int[]
			{
				Shader.PropertyToID("_ClipRange0"),
				Shader.PropertyToID("_ClipRange1"),
				Shader.PropertyToID("_ClipRange2"),
				Shader.PropertyToID("_ClipRange4"),
			};
		}

		if (ClipArgs == null)
		{
			ClipArgs = new int[]
			{
				Shader.PropertyToID("_ClipArgs0"),
				Shader.PropertyToID("_ClipArgs1"),
				Shader.PropertyToID("_ClipArgs2"),
				Shader.PropertyToID("_ClipArgs3"),
			};
		}
	}

	/// <summary>
	/// The material should be rebuilt when the draw call is enabled.
	/// </summary>

	void OnEnable () { mRebuildMat = true; }

	/// <summary>
	/// Clear all references.
	/// </summary>

	void OnDisable ()
	{
		depthStart = int.MaxValue;
		depthEnd = int.MinValue;
		panel = null;
		manager = null;
		mMaterial = null;
		mTexture = null;
		clipTexture = null;

		if (mRenderer != null)
			mRenderer.sharedMaterials = new Material[] {};

		NGUITools.DestroyImmediate(mDynamicMat);
		mDynamicMat = null;
	}

	/// <summary>
	/// Cleanup.
	/// </summary>

	void OnDestroy ()
	{
		NGUITools.DestroyImmediate(mMesh);
		mMesh = null;
	}

	/// <summary>
	/// Return an existing draw call.
	/// </summary>

	static public UIDrawCall Create (UIPanel panel, Material mat, Texture tex, Shader shader)
	{
#if UNITY_EDITOR
		string name = null;
		if (tex != null) name = tex.name;
		else if (shader != null) name = shader.name;
		else if (mat != null) name = mat.name;
		return Create(name, panel, mat, tex, shader);
#else
		return Create(null, panel, mat, tex, shader);
#endif
	}

	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

	static UIDrawCall Create (string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall dc = Create(name);
		dc.gameObject.layer = pan.cachedGameObject.layer;
		dc.baseMaterial = mat;
		dc.mainTexture = tex;
		dc.shader = shader;
		dc.renderQueue = pan.startingRenderQueue;
		dc.sortingOrder = pan.sortingOrder;
		dc.manager = pan;
		return dc;
	}

	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

	static UIDrawCall Create (string name)
	{
#if SHOW_HIDDEN_OBJECTS && UNITY_EDITOR
		name = (name != null) ? "_UIDrawCall [" + name + "]" : "DrawCall";
#endif
		if (mInactiveList.size > 0)
		{
			UIDrawCall dc = mInactiveList.Pop();
			mActiveList.Add(dc);
			if (name != null) dc.name = name;
			NGUITools.SetActive(dc.gameObject, true);
			return dc;
		}

#if UNITY_EDITOR
		// If we're in the editor, create the game object with hide flags set right away
		GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name,
 #if SHOW_HIDDEN_OBJECTS
			HideFlags.DontSave | HideFlags.NotEditable, typeof(UIDrawCall));
 #else
			HideFlags.HideAndDontSave, typeof(UIDrawCall));
 #endif
		UIDrawCall newDC = go.GetComponent<UIDrawCall>();
#else
		GameObject go = new GameObject(name);
		DontDestroyOnLoad(go);
		UIDrawCall newDC = go.AddComponent<UIDrawCall>();
#endif
		// Create the draw call
		mActiveList.Add(newDC);
		return newDC;
	}

	/// <summary>
	/// Clear all draw calls.
	/// </summary>

	static public void ClearAll ()
	{
		bool playing = Application.isPlaying;

		for (int i = mActiveList.size; i > 0; )
		{
			UIDrawCall dc = mActiveList[--i];

			if (dc)
			{
				if (playing) NGUITools.SetActive(dc.gameObject, false);
				else NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
		mActiveList.Clear();
	}

	/// <summary>
	/// Immediately destroy all draw calls.
	/// </summary>

	static public void ReleaseAll ()
	{
		ClearAll();
		ReleaseInactive();
	}

	/// <summary>
	/// Immediately destroy all inactive draw calls (draw calls that have been recycled and are waiting to be re-used).
	/// </summary>

	static public void ReleaseInactive()
	{
		for (int i = mInactiveList.size; i > 0; )
		{
			UIDrawCall dc = mInactiveList[--i];
			if (dc) NGUITools.DestroyImmediate(dc.gameObject);
		}
		mInactiveList.Clear();
	}

	/// <summary>
	/// Count all draw calls managed by the specified panel.
	/// </summary>

	static public int Count (UIPanel panel)
	{
		int count = 0;
		for (int i = 0; i < mActiveList.size; ++i)
			if (mActiveList[i].manager == panel) ++count;
		return count;
	}

	/// <summary>
	/// Destroy the specified draw call.
	/// </summary>

	static public void Destroy (UIDrawCall dc)
	{
		if (dc)
		{
			dc.onRender = null;

			if (Application.isPlaying)
			{
				if (mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);
					mInactiveList.Add(dc);
				}
			}
			else
			{
				mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}
}
