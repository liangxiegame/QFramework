// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------
// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------

using UnityEngine;

public class NcTransformTool
{
	public Vector3		m_vecPos;
	public Quaternion	m_vecRot;
	public Vector3		m_vecRotHint;
	public Vector3		m_vecScale;

	// --------------------------------------------------------------------------
	public NcTransformTool()
	{
		m_vecPos	= new Vector3();
		m_vecRot	= new Quaternion();
		m_vecRotHint= new Vector3();
		m_vecScale	= new Vector3(1, 1, 1);
	}

	public NcTransformTool(Transform val)
	{
		SetLocalTransform(val);
	}

	// --------------------------------------------------------------------------
	public static Vector3		GetZeroVector()		{ return Vector3.zero; }
	public static Vector3		GetUnitVector()		{ return new Vector3(1, 1, 1); }
	public static Quaternion	GetIdenQuaternion() { return Quaternion.identity; }

	// --------------------------------------------------------------------------
	public static void InitLocalTransform(Transform dst)
	{
		dst.localPosition	= GetZeroVector();
		dst.localRotation	= GetIdenQuaternion();
		dst.localScale		= GetUnitVector();
	}

	public static void InitWorldTransform(Transform dst)
	{
		dst.position	= GetZeroVector();
		dst.rotation	= GetIdenQuaternion();
		InitWorldScale(dst);
	}

	public static void InitWorldScale(Transform dst)
	{
		// (System.Single.IsInfinity(dst.lossyScale.x)
		dst.localScale = GetUnitVector();
		dst.localScale	= new Vector3((dst.lossyScale.x == 0 ? 1 : 1 / dst.lossyScale.x),
									  (dst.lossyScale.y == 0 ? 1 : 1 / dst.lossyScale.y),
									  (dst.lossyScale.z == 0 ? 1 : 1 / dst.lossyScale.z));
	}

	public static void CopyLocalTransform(Transform src, Transform dst)
	{
		dst.localPosition = src.localPosition;
		dst.localRotation = src.localRotation;
		dst.localScale = src.localScale;
	}

	public static void CopyLossyToLocalScale(Vector3 srcLossyScale, Transform dst)
	{
		// error dst.lossyScale.x == 0
		dst.localScale = GetUnitVector();
		dst.localScale = new Vector3((dst.lossyScale.x == 0 ? srcLossyScale.x : srcLossyScale.x / (dst.lossyScale.x)),
									 (dst.lossyScale.y == 0 ? srcLossyScale.y : srcLossyScale.y / (dst.lossyScale.y)),
									 (dst.lossyScale.z == 0 ? srcLossyScale.z : srcLossyScale.z / (dst.lossyScale.z)));
	}

	// --------------------------------------------------------------------------
	public void CopyToLocalTransform(Transform dst)
	{
		dst.localPosition	= m_vecPos;
		dst.localRotation	= m_vecRot;
		dst.localScale		= m_vecScale;
	}

	public void CopyToTransform(Transform dst)
	{
		dst.position	= m_vecPos;
		dst.rotation	= m_vecRot;
		CopyLossyToLocalScale(m_vecScale, dst);
	}

	// --------------------------------------------------------------------------
	public void AddLocalTransform(Transform val)
	{
		m_vecPos	= m_vecPos + val.localPosition;
		m_vecRot	= Quaternion.Euler(m_vecRot.eulerAngles + val.localRotation.eulerAngles);
		m_vecScale	= Vector3.Scale(m_vecScale, val.localScale);
	}

	public void SetLocalTransform(Transform val)
	{
		m_vecPos	= val.localPosition;
		m_vecRot	= val.localRotation;
		m_vecScale	= val.localScale;
//		Debug.Log(m_vecPos);
	}

	public bool IsLocalEquals(Transform val)
	{
		if (m_vecPos != val.localPosition) return false;
		if (m_vecRot != val.localRotation) return false;
		if (m_vecScale != val.localScale) return false;
		return true;
	}

	// --------------------------------------------------------------------------
	public void AddTransform(Transform val)
	{
		m_vecPos = m_vecPos + val.position;
		m_vecRot = Quaternion.Euler(m_vecRot.eulerAngles + val.rotation.eulerAngles);
		m_vecScale = Vector3.Scale(m_vecScale, val.lossyScale);
	}

	public void SetTransform(Transform val)
	{
		m_vecPos	= val.position;
		m_vecRot	= val.rotation;
		m_vecScale	= val.lossyScale;
	}

	public bool IsEquals(Transform val)
	{
		if (m_vecPos != val.position) return false;
		if (m_vecRot != val.rotation) return false;
		if (m_vecScale != val.lossyScale) return false;
		return true;
	}
	// --------------------------------------------------------------------------
	public void SetTransform(NcTransformTool val)
	{
		m_vecPos	= val.m_vecPos;
		m_vecRot	= val.m_vecRot;
		m_vecScale	= val.m_vecScale;
	}

	// static util --------------------------------------------------------------------------
	public static float GetTransformScaleMeanValue(Transform srcTrans)
	{
		return ((srcTrans.lossyScale.x + srcTrans.lossyScale.y + srcTrans.lossyScale.z) / 3.0f);
	}

	public static Vector3 GetTransformScaleMeanVector(Transform srcTrans)
	{
		float meanValue = GetTransformScaleMeanValue(srcTrans);
		return new Vector3(meanValue, meanValue, meanValue);
	}

}
