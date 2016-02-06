//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generated geometry class. All widgets have one.
/// This class separates the geometry creation into several steps, making it possible to perform
/// actions selectively depending on what has changed. For example, the widget doesn't need to be
/// rebuilt unless something actually changes, so its geometry can be cached. Likewise, the widget's
/// transformed coordinates only change if the widget's transform moves relative to the panel,
/// so that can be cached as well. In the end, using this class means using more memory, but at
/// the same time it allows for significant performance gains, especially when using widgets that
/// spit out a lot of vertices, such as UILabels.
/// </summary>

public class UIGeometry
{
	/// <summary>
	/// Widget's vertices (before they get transformed).
	/// </summary>

	public BetterList<Vector3> verts = new BetterList<Vector3>();

	/// <summary>
	/// Widget's texture coordinates for the geometry's vertices.
	/// </summary>

	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	/// <summary>
	/// Array of colors for the geometry's vertices.
	/// </summary>

	public BetterList<Color32> cols = new BetterList<Color32>();

	// Relative-to-panel vertices, normal, and tangent
	BetterList<Vector3> mRtpVerts = new BetterList<Vector3>();
	Vector3 mRtpNormal;
	Vector4 mRtpTan;

	/// <summary>
	/// Whether the geometry contains usable vertices.
	/// </summary>

	public bool hasVertices { get { return (verts.size > 0); } }

	/// <summary>
	/// Whether the geometry has usable transformed vertex data.
	/// </summary>

	public bool hasTransformed { get { return (mRtpVerts != null) && (mRtpVerts.size > 0) && (mRtpVerts.size == verts.size); } }

	/// <summary>
	/// Step 1: Prepare to fill the buffers -- make them clean and valid.
	/// </summary>

	public void Clear ()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	/// <summary>
	/// Step 2: Transform the vertices by the provided matrix.
	/// </summary>

	public void ApplyTransform (Matrix4x4 widgetToPanel, bool generateNormals = true)
	{
		if (verts.size > 0)
		{
			mRtpVerts.Clear();
			for (int i = 0, imax = verts.size; i < imax; ++i) mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(verts[i]));

			// Calculate the widget's normal and tangent
			if (generateNormals)
			{
				mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
				Vector3 tangent = widgetToPanel.MultiplyVector(Vector3.right).normalized;
				mRtpTan = new Vector4(tangent.x, tangent.y, tangent.z, -1f);
			}
		}
		else mRtpVerts.Clear();
	}

	/// <summary>
	/// Step 3: Fill the specified buffer using the transformed values.
	/// </summary>

	public void WriteToBuffers (BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		if (mRtpVerts != null && mRtpVerts.size > 0)
		{
			if (n == null)
			{
				for (int i = 0; i < mRtpVerts.size; ++i)
				{
					v.Add(mRtpVerts.buffer[i]);
					u.Add(uvs.buffer[i]);
					c.Add(cols.buffer[i]);
				}
			}
			else
			{
				for (int i = 0; i < mRtpVerts.size; ++i)
				{
					v.Add(mRtpVerts.buffer[i]);
					u.Add(uvs.buffer[i]);
					c.Add(cols.buffer[i]);
					n.Add(mRtpNormal);
					t.Add(mRtpTan);
				}
			}
		}
	}
}
