// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FXMakerWireframe : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public			Transform		m_BaseTrans;

	// wireframe
	public			bool			m_bWireframe		= false;
	public			bool			m_bTexture			= true;
	public			bool			m_bRoot				= false;
	public			string			m_ParentName;
	protected		bool			m_bScaledParticle	= false;
	protected		bool			m_bWorldParticle	= true;
	protected		List<Vector4>	m_LastPparticlePos	= new List<Vector4>();

	protected		Color			m_backgroundColor;
	protected		bool			m_ZWrite			= true;
	protected		bool			m_AWrite			= true;
	protected		bool			m_blend				= true;

	protected		Vector3[]		m_lines;
	protected		Material		m_lineMaterial;
	protected		MeshRenderer	m_meshRenderer;

	public static	Transform		m_CalcBaseTrans;
	public static	Transform		m_CalcChildTrans;

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------
	bool IsShuriken()
	{
		return m_BaseTrans.GetComponent<ParticleSystem>() != null;
	}

	bool IsLegacy()
	{
		return m_BaseTrans.GetComponent<ParticleEmitter>() != null && m_BaseTrans.GetComponent<ParticleEmitter>().enabled;
	}

	Component GetParticleComponent()
	{
		if (IsShuriken())
			return m_BaseTrans.GetComponent<ParticleSystem>();
		if (IsLegacy())
			return m_BaseTrans.GetComponent<ParticleEmitter>();
		return null;
	}

	// Control --------------------------------------------------------------------------
	public void InitWireframe(Transform srcTrans)
	{
		m_BaseTrans = srcTrans;
		NcParticleSystem ncPartScale = m_BaseTrans.GetComponent<NcParticleSystem>();
 		m_bScaledParticle = (ncPartScale != null && ncPartScale.enabled && ncPartScale.m_bScaleWithTransform);

// 		m_meshRenderer = GetComponent<MeshRenderer>();
// 		if (!m_meshRenderer)
// 			m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
	}

	public bool InitWireframe(Transform	srcTrans, bool bWireframe, bool bTexture, bool bRoot)
	{
		InitWireframe(srcTrans);
		return SetWireframe(bWireframe, bTexture, bRoot);
	}

	public int GetParticleCount()
	{
		if (IsShuriken())
			return m_BaseTrans.GetComponent<ParticleSystem>().particleCount;
		if (IsLegacy())
			return m_BaseTrans.GetComponent<ParticleEmitter>().particleCount;
		return 0;
	}

	public List<Vector4> GetLastParticlePostions()
	{
		return m_LastPparticlePos;
	}

	List<Vector4> GetParticlePostions(bool bPosUpdate)
	{
		List<Vector4>	listPartis	= new List<Vector4>();

		if (IsLegacy() || IsShuriken())
		{
			CreateTempObject();

			if (IsShuriken())
			{
				float fScale = 1.0f;
				ParticleSystem.Particle[]	parts = new ParticleSystem.Particle[m_BaseTrans.GetComponent<ParticleSystem>().particleCount];
				m_BaseTrans.GetComponent<ParticleSystem>().GetParticles(parts);

				NcTransformTool.InitWorldTransform(m_CalcBaseTrans);
				if (m_bWorldParticle)
 				{
					NcParticleSystem ncPartScale = m_BaseTrans.GetComponent<NcParticleSystem>();
					if (m_bScaledParticle)
						ncPartScale.ShurikenScaleParticle(parts, parts.Length, true, bPosUpdate);
				} else {
					m_CalcBaseTrans.position	= m_BaseTrans.position;
					m_CalcBaseTrans.rotation	= m_BaseTrans.rotation;

					if (m_bScaledParticle)
					{
						NcTransformTool.CopyLossyToLocalScale(NcTransformTool.GetTransformScaleMeanVector(m_BaseTrans), m_CalcBaseTrans);
						fScale = NcTransformTool.GetTransformScaleMeanValue(m_BaseTrans);
					}
				}

				foreach (ParticleSystem.Particle part in parts)
				{
					Vector3 worldPos;
					float	fSize	= part.size * fScale;
					NcTransformTool.InitLocalTransform(m_CalcChildTrans);
					m_CalcChildTrans.localPosition = part.position;
					worldPos = m_CalcChildTrans.position;

					listPartis.Add(new Vector4(worldPos.x, worldPos.y, worldPos.z, fSize));
				}
			} else {
				float fScale = 1.0f;
				Particle[]	parts = m_BaseTrans.GetComponent<ParticleEmitter>().particles;

				NcTransformTool.InitWorldTransform(m_CalcBaseTrans);
				if (m_bWorldParticle)
 				{
					// m_bLegacyRuntimeScale
					NcParticleSystem ncPartScale = m_BaseTrans.GetComponent<NcParticleSystem>();
					if (m_bScaledParticle)
						ncPartScale.LegacyScaleParticle(parts, true, bPosUpdate);
				} else {
					m_CalcBaseTrans.position	= m_BaseTrans.position;
					m_CalcBaseTrans.rotation	= m_BaseTrans.rotation;

					// m_bLegacyRuntimeScale
					NcParticleSystem ncPartScale = m_BaseTrans.GetComponent<NcParticleSystem>();
					if (m_bScaledParticle && ncPartScale.IsMeshParticleEmitter() == false)
					{
						NcTransformTool.CopyLossyToLocalScale(NcTransformTool.GetTransformScaleMeanVector(m_BaseTrans), m_CalcBaseTrans);
						fScale = NcTransformTool.GetTransformScaleMeanValue(m_BaseTrans);
					} else fScale = NcTransformTool.GetTransformScaleMeanValue(m_BaseTrans);
				}

				foreach (Particle part in parts)
				{
					if (System.Single.IsNaN(part.position.x))
						continue;
					Vector3 worldPos;
//					float	fSize	= part.size;
					float	fSize	= part.size * fScale;		// m_bLegacyRuntimeScale
					NcTransformTool.InitLocalTransform(m_CalcChildTrans);
					m_CalcChildTrans.localPosition = part.position;
					worldPos = m_CalcChildTrans.position;
					listPartis.Add(new Vector4(worldPos.x, worldPos.y, worldPos.z, fSize));
				}
			}
		}
		return listPartis;
	}

	// UpdateLoop -----------------------------------------------------------------------
	void Start()
	{
	}

	void Update()
	{
		if ((IsLegacy() || IsShuriken()) == false)
			UpdateLines();
	}

	void OnRenderObject()
	{
//		if (Camera.main == null || m_meshRenderer == null)
		if (Camera.main == null)
			return;

		if (m_BaseTrans == null)
		{
			// OnDestroy ���� ������ wire
			Destroy(gameObject);
			return;
		}

		if (IsLegacy() || IsShuriken())
			m_LastPparticlePos = GetParticlePostions(true);

		if (m_bWireframe == false)
			return;

		if (IsLegacy() || IsShuriken())
		{
			foreach (Vector4 pos in m_LastPparticlePos)
				RenderParticleWire(pos, pos.w, 0);
		}
		
		if (m_lines != null)
		{

//			m_meshRenderer.sharedMaterial.color = m_backgroundColor;
 			InitLineMaterial();
			m_lineMaterial.SetPass(0);

			Matrix4x4	matrix;
//			if (IsShuriken())
			if (IsLegacy() || IsShuriken())		// m_bLegacyRuntimeScale
			{
				NcTransformTool.InitWorldTransform(m_CalcBaseTrans);
				m_CalcBaseTrans.position	= m_BaseTrans.position;
				m_CalcBaseTrans.rotation	= m_BaseTrans.rotation;

				if (m_bScaledParticle)
					NcTransformTool.CopyLossyToLocalScale(NcTransformTool.GetTransformScaleMeanVector(m_BaseTrans), m_CalcBaseTrans);
				matrix = m_CalcBaseTrans.localToWorldMatrix;
			} else {
				// model
				matrix = m_BaseTrans.localToWorldMatrix;
			}

			GL.PushMatrix();
			GL.MultMatrix(matrix);
			GL.Begin(GL.LINES);
			GL.Color((m_bRoot ? FXMakerOption.inst.m_ColorRootWireframe : FXMakerOption.inst.m_ColorChildWireframe));

			for (int i = 0; i < m_lines.Length / 3; i++)
			{
				GL.Vertex(m_lines[i * 3]);
				GL.Vertex(m_lines[i * 3 + 1]);

				GL.Vertex(m_lines[i * 3 + 1]);
				GL.Vertex(m_lines[i * 3 + 2]);

				GL.Vertex(m_lines[i * 3 + 2]);
				GL.Vertex(m_lines[i * 3]);
			}

			GL.End();
			GL.PopMatrix();
		}
	}

	void RenderParticleWire(Vector3 pos, float fSize, float fRot)
	{
		if (System.Single.IsNaN(pos.x))
			return;

		fSize *= 0.5f;
//		m_meshRenderer.sharedMaterial.color = m_backgroundColor;
		InitLineMaterial();
		m_lineMaterial.SetPass(0);

		NcTransformTool.InitWorldTransform(m_CalcChildTrans);
		m_CalcChildTrans.LookAt(Camera.main.transform, Camera.main.transform.up);
		m_CalcChildTrans.position	= pos;


		GL.PushMatrix();
		GL.MultMatrix(m_CalcChildTrans.localToWorldMatrix);
		GL.Begin(GL.LINES);
		GL.Color((m_bRoot ? FXMakerOption.inst.m_ColorRootWireframe : FXMakerOption.inst.m_ColorChildWireframe));

		{
			GL.Vertex(new Vector3(-fSize, -fSize, 0));
			GL.Vertex(new Vector3(+fSize, -fSize, 0));

			GL.Vertex(new Vector3(+fSize, -fSize, 0));
			GL.Vertex(new Vector3(+fSize, +fSize, 0));

			GL.Vertex(new Vector3(+fSize, +fSize, 0));
			GL.Vertex(new Vector3(-fSize, +fSize, 0));

			GL.Vertex(new Vector3(-fSize, +fSize, 0));
			GL.Vertex(new Vector3(-fSize, -fSize, 0));

			GL.Vertex(new Vector3(-fSize, -fSize, 0));
			GL.Vertex(new Vector3(+fSize, +fSize, 0));
		}

		GL.End();
		GL.PopMatrix();
	}

	void CreateTempObject()
	{
		if (m_CalcBaseTrans == null)
		{
			m_CalcBaseTrans = FXMakerMain.inst.GetInstanceRoot().transform.FindChild("_TempBaseObject");
			if (m_CalcBaseTrans == null)
			{
				m_CalcBaseTrans		= NgObject.CreateGameObject(FXMakerMain.inst.GetInstanceRoot(), "_TempBaseObject").transform;
				m_CalcChildTrans	= NgObject.CreateGameObject(m_CalcBaseTrans.gameObject, "_TempChildObject").transform;
			}
		}
	}

	// Event -------------------------------------------------------------------------
	// Function ----------------------------------------------------------------------
	public bool SetWireframe(bool bWireframe, bool bTexture, bool bRoot)
	{
		if (m_BaseTrans == null || m_BaseTrans.GetComponent<Renderer>() == null)
		{
			m_bWireframe	 = false;
			return false;
		}

		m_bTexture		= bTexture;
		m_bWireframe	= bWireframe;
		m_bRoot			= bRoot;
//		m_BaseTrans.renderer.enabled = m_bTexture;
// 		if (bWireframe == false)
// 		{
// 			m_bWireframe = false;
// 			return false;
// 		}

// 		m_meshRenderer = GetComponent<MeshRenderer>();
// 		if (!m_meshRenderer)
// 			m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
// 		if (m_meshRenderer.material == null)
// 		{
// 			m_meshRenderer.material = new Material("Shader \"Lines/Background\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass {" + (m_ZWrite ? " ZWrite on " : " ZWrite off ") + (m_blend ? " Blend SrcAlpha OneMinusSrcAlpha" : " ") + (m_AWrite ? " Colormask RGBA " : " ") + "Lighting Off Offset 1, 1 Color[_Color] }}}");
// // 			m_meshRenderer.material = FXMakerMain.inst.m_LineBackMaterial;
// 		}

		UpdateLines();
 		return m_bWireframe;
	}

	void InitLineMaterial()
	{
		if (m_lineMaterial == null)
		{
// 			m_lineMaterial	= new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind \"Color\",color } ZWrite On Cull Front Fog { Mode Off } } } }");
			m_lineMaterial	= FXMakerMain.inst.m_LineColorMaterial;
			m_lineMaterial.hideFlags		= HideFlags.HideAndDontSave;
			m_lineMaterial.shader.hideFlags	= HideFlags.HideAndDontSave;
		}
	}

	void UpdateLines()
	{
		bool	bParticle = (IsLegacy() || IsShuriken());

		if ((bParticle == false) && (m_BaseTrans.GetComponent<MeshFilter>() == null || m_BaseTrans.GetComponent<MeshFilter>().mesh == null))
		{
			m_bWireframe = false;
			return;
		}

		ArrayList	linesArray	= new ArrayList();
		Mesh		mesh;

		if (bParticle)
		{
			mesh = NgSerialized.GetParticleMesh(GetParticleComponent());
			m_bWorldParticle = NgSerialized.GetSimulationSpaceWorld(m_BaseTrans);
		} else {
			mesh = m_BaseTrans.GetComponent<MeshFilter>().mesh;
		}

		if (mesh != null)
		{
			Vector3[]	vertices	= mesh.vertices;
			int[]		triangles	= mesh.triangles;

			for (int i = 0; i < triangles.Length / 3; i++)
			{
				linesArray.Add(vertices[triangles[i * 3]]);
				linesArray.Add(vertices[triangles[i * 3 + 1]]);
				linesArray.Add(vertices[triangles[i * 3 + 2]]);
			}
			m_lines = NgConvert.ToArray<Vector3>(linesArray);
		}
	}
}
#endif
