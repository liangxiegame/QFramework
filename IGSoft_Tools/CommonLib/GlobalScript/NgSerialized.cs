// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NgSerialized
{
	// Attribute ------------------------------------------------------------------------

#if UNITY_EDITOR
	// SerializedObject ---------------------------------------------------------------------------------------------------------------------------------------------
	public static SerializedObject GetSerializedObject(Object obj)
	{
		return new SerializedObject(obj);
	}

	public static void CopyProperty(Object srcObj, Object tarObj, string name)
	{
		SerializedObject srcSo = GetSerializedObject(srcObj);
		SerializedObject tarSo = GetSerializedObject(tarObj);
		SetPropertyValue(tarSo, name, GetPropertyValue(srcSo, name), true);
	}

	public static string[] LogPropertis(Object obj, bool enterChildren)
	{
		SerializedObject so = new SerializedObject(obj);
		return LogPropertis(so, enterChildren);
	}

	public static string[] LogPropertis(SerializedObject serObj, bool enterChildren)
	{
		return LogPropertis(serObj, "", enterChildren);
	}

	public static string[] LogPropertis(SerializedObject serObj, string startPropertyName, bool enterChildren)
	{
		if (serObj == null)
			return null;

		string				log = "";
		SerializedProperty	sp;
		bool				bFirst = true;

		if (startPropertyName != null && startPropertyName != "")
			sp = serObj.FindProperty(startPropertyName);
		else sp = serObj.GetIterator();

		while (true)
		{
			object value = GetPropertyValue(sp);
			log += string.Format("{0}{3}{4}{5}   {1,-30}        {2, 20} {6}\r\n", sp.depth, NgConvert.GetTabSpace(sp.depth+1) + sp.name, (value == null ? "null" : value.ToString()), sp.editable, sp.isExpanded, sp.isArray, sp.propertyPath);

			if (sp.Next(bFirst) == false)
				break;
			bFirst = enterChildren;
		}

		{
// 			log = "=====================================================================\r\n" + log;
// 			log = log + "=====================================================================\r\n";
// 			Debug.Log(log);
		}
		return null;
	}

	public static bool IsPropertyValue(SerializedObject serObj, string propertyName)
	{
		SerializedProperty sp = serObj.FindProperty(propertyName);
		return (sp != null);
	}

	public static object GetPropertyValue(SerializedObject serObj, string propertyName)
	{
		SerializedProperty sp = serObj.FindProperty(propertyName);
		if (sp == null)
			return null;
		return GetPropertyValue(sp);
	}

	public static void SetPropertyValue(SerializedObject serObj, string propertyName, object value, bool bUpdate)
	{
		SerializedProperty sp = serObj.FindProperty(propertyName);
		if (sp == null)
		{
			Debug.LogError("SetPropertyValue error - " + propertyName);
			return;
		}
		SetPropertyValue(sp, value);
		if (bUpdate)
		{
 			if (serObj.ApplyModifiedProperties() == false)
 				NgUtil.LogDevelop("Failed - ApplyModifiedProperties");		// dup
		}
	}

	public static object GetPropertyValue(SerializedProperty sp)
	{
		object value = null;

		switch (sp.propertyType)
		{
			case SerializedPropertyType.Integer			:	value = sp.intValue;						 break;
			case SerializedPropertyType.Boolean			:	value = sp.boolValue;						 break;
			case SerializedPropertyType.Float			:	value = sp.floatValue;						 break;
			case SerializedPropertyType.String			:	value = sp.stringValue;						 break;
			case SerializedPropertyType.Color			:	value = sp.colorValue;						 break;
			case SerializedPropertyType.AnimationCurve	:	value = sp.animationCurveValue;				 break;
			case SerializedPropertyType.ObjectReference	:	value = sp.objectReferenceValue;			 break;
			case SerializedPropertyType.Enum			:	value = sp.enumNames[sp.enumValueIndex].ToString();		 break;
			case SerializedPropertyType.Vector2			:	value = sp.vector2Value;					 break;
			case SerializedPropertyType.Vector3			:	value = sp.vector3Value;					 break;
			case SerializedPropertyType.Rect			:	value = sp.rectValue;						 break;
			case SerializedPropertyType.Bounds			:	value = sp.boundsValue;						 break;
// 			default: Debug.Log("Undefile propertyType - GetPropertyValue " + sp.name);		break;
		}
		return value;
	}

	public static void SetPropertyValue(SerializedProperty sp, object value)
	{
		switch (sp.propertyType)
		{
			case SerializedPropertyType.Integer			:	sp.intValue		= (int)value;			 break;
			case SerializedPropertyType.Boolean			:	sp.boolValue	= (bool)value;			 break;
			case SerializedPropertyType.Float			:	sp.floatValue	= (float)value;			 break;
			case SerializedPropertyType.String			:	sp.stringValue	= (string)value;		 break;
			case SerializedPropertyType.Color			:	sp.colorValue	= (Color)value;			 break;
			case SerializedPropertyType.AnimationCurve	:	sp.animationCurveValue	= (AnimationCurve)value;	break;
			case SerializedPropertyType.ObjectReference	:	sp.objectReferenceValue	= (Object)value;			break;
			case SerializedPropertyType.Enum			:	sp.enumValueIndex = (int)value;			 break;
			case SerializedPropertyType.Vector2			:	sp.vector2Value	= (Vector2)value;		 break;
			case SerializedPropertyType.Vector3			:	sp.vector3Value	= (Vector3)value;		 break;
			case SerializedPropertyType.Rect			:	sp.rectValue	= (Rect)value;			 break;
			case SerializedPropertyType.Bounds			:	sp.boundsValue	= (Bounds)value;		 break;
// 			default: Debug.Log("Undefile propertyType - SetPropertyValue " + sp.name); break;
		}
	}

	// DeepCopy ---------------------------------------------------------------------------------------------------------------------------------------------
	public static bool IsMeshParticleEmitter(ParticleEmitter srcCom)
	{
		if (srcCom == null)
		{
			Debug.LogWarning("arg is null!!!");
			return false;
		}
		return (srcCom.ToString().Contains("MeshParticleEmitter"));
	}

	public static bool IsEllipsoidParticleEmitter(ParticleEmitter srcCom)
	{
		if (srcCom == null)
		{
			Debug.LogWarning("arg is null!!!");
			return false;
		}
		return (srcCom.ToString().Contains("EllipsoidParticleEmitter"));
	}

	public static bool IsValidCopy(Component tarCom, Component srcCom)
	{
		if (tarCom == null || srcCom == null)
		{
			Debug.LogWarning("arg is null!!!");
			return false;
		}

		if (srcCom.ToString().Contains("EllipsoidParticleEmitter") || srcCom.ToString().Contains("MeshParticleEmitter"))
		{
			return srcCom.ToString().Contains("EllipsoidParticleEmitter") == srcCom.ToString().Contains("EllipsoidParticleEmitter");
		}
		return true;
	}

	public static Component AddComponent(GameObject tarObject, Component srcCom)
	{
		if (srcCom.ToString().Contains("EllipsoidParticleEmitter"))
			return tarObject.AddComponent<EllipsoidParticleEmitter>();
		if (srcCom.ToString().Contains("MeshParticleEmitter"))
			return tarObject.AddComponent<MeshParticleEmitter>();
		if (srcCom.ToString().Contains("WorldParticleCollider"))
			return UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(tarObject, "Assets/IGSoft_Tools/CommonLib/GlobalScript/NgSerialized.cs (190,11)", "WorldParticleCollider");
		Component com = tarObject.AddComponent(srcCom.GetType());

		return com;
	}

	public static Component CloneComponent(Component srcCom, GameObject targetGameObj, bool bRemoveSrcCom)
	{
		Component tarCom	= NgSerialized.AddComponent(targetGameObj, srcCom);
		NgSerialized.CopySerialized(srcCom, tarCom);
		if (bRemoveSrcCom)
			Object.DestroyImmediate(srcCom);
		return tarCom;
	}

	public static void CopySerialized(Component srcCom, Component tarCom)
	{
		if (srcCom == null || tarCom == null)
		{
			Debug.LogError("Arg is null !!!");
			return;
		}

		Transform tarTrans = tarCom.transform;
		EditorUtility.CopySerialized(srcCom, tarCom);
		CopyProperty(tarTrans, tarCom, "m_GameObject");
// 		GetPropertis(tarCom);
		return;
	}

	// GetSet Function ========================================================================================================
// 	NgAssembly.GetPropertis(sysSo, "ShapeModule");

	public static bool IsEnableParticleMesh(Component particleCom, bool bCheckParticleSystem)
	{
		if (particleCom is ParticleSystem)
		{
			if (bCheckParticleSystem)
			{
				// slow code...
 				SerializedObject sysSo = new SerializedObject(particleCom as ParticleSystem);
// 				Debug.Log((bool)GetPropertyValue(sysSo, "ShapeModule.enabled"));
// 				Debug.Log((int)GetPropertyValue(sysSo, "ShapeModule.type"));
				if ((bool)GetPropertyValue(sysSo, "ShapeModule.enabled"))
					return ((int)GetPropertyValue(sysSo, "ShapeModule.type") == 6/*Mesh*/);
			} else {
				return false;
			}
		}
		if (particleCom is ParticleEmitter)
		{
			return IsMeshParticleEmitter(particleCom as ParticleEmitter);
// 			return IsPropertyValue(new SerializedObject(particleCom as ParticleEmitter), "m_Mesh");
		}
		if (particleCom is ParticleSystemRenderer)
		{
// 			NgAssembly.LogFieldsPropertis(particleCom);
			if (GetPropertyValue(new SerializedObject(particleCom), "m_RenderMode").ToString() == "Mesh")
				return true;
		}
		return false;
	}

	public static bool IsMesh(Component com, bool bCheckParticleSystem)
	{
		if (IsEnableParticleMesh(com, bCheckParticleSystem))
			return true;
		if (com is MeshFilter)
			return true;
		return false;
	}

	public static Mesh GetMesh(Component com, bool bCheckParticleSystem)
	{
		if (IsEnableParticleMesh(com, bCheckParticleSystem))
			return GetParticleMesh(com);
		if (com is MeshFilter)
			return (com as MeshFilter).sharedMesh;
		return null;
	}

	public static void SetMesh(Component com, Mesh saveMesh, bool bUpdate)
	{
		if (IsEnableParticleMesh(com, true))
			SetParticleMesh(com, saveMesh, bUpdate);
		if (com is MeshFilter)
			(com as MeshFilter).sharedMesh = saveMesh;
	}

	public static Mesh GetParticleMesh(Component particleCom)
	{
		if (particleCom is ParticleSystem)
		{
			SerializedObject sysSo = new SerializedObject(particleCom as ParticleSystem);
			if ((bool)GetPropertyValue(sysSo, "ShapeModule.enabled"))
				if ((int)GetPropertyValue(sysSo, "ShapeModule.type") == 6/*Mesh*/)
					return GetPropertyValue(sysSo, "ShapeModule.m_Mesh") as Mesh;
			return null;
		}
		if (particleCom is ParticleEmitter)
		{
 			return GetPropertyValue(new SerializedObject(particleCom as ParticleEmitter), "m_Mesh") as Mesh;
		}
		if (particleCom is ParticleSystemRenderer)
		{
			return GetPropertyValue(new SerializedObject(particleCom as ParticleSystemRenderer), "m_Mesh") as Mesh;
		}
		return null;
	}

	public static void SetParticleMesh(Component particleCom, Mesh saveMesh, bool bUpdate)
	{
		if (particleCom is ParticleSystem)
		{
			SerializedObject sysSo = new SerializedObject(particleCom as ParticleSystem);
// 			GetPropertis(sysSo, "ShapeModule");
// 			if ((bool)GetPropertyValue(sysSo, "ShapeModule.enabled"))
// 				if ((int)GetPropertyValue(sysSo, "ShapeModule.type") == 6/*Mesh*/)
			Debug.Log(particleCom.name);
					SetPropertyValue(sysSo, "ShapeModule.m_Mesh", saveMesh, bUpdate);
			return;
		}
		if (particleCom is ParticleEmitter)
		{
			SetPropertyValue(new SerializedObject(particleCom as ParticleEmitter), "m_Mesh", saveMesh, bUpdate);
			return;
		}
		if (particleCom is ParticleSystemRenderer)
		{
			SetPropertyValue(new SerializedObject(particleCom as ParticleSystemRenderer), "m_Mesh", saveMesh, bUpdate);
			return;
		}
	}

	public static bool GetSimulationSpaceWorld(Transform srcTrans)
	{
		if (srcTrans.GetComponent<ParticleSystem>() != null)
		{
			SerializedObject sysSo = new SerializedObject(srcTrans.GetComponent<ParticleSystem>());
// 			GetPropertis(sysSo, "ShapeModule");
			return ((bool)GetPropertyValue(sysSo, "moveWithTransform") == false);
		}
		if (srcTrans.GetComponent<ParticleEmitter>() != null)
			return srcTrans.GetComponent<ParticleEmitter>().useWorldSpace;

		return false;
	}

	public static void SetSimulationSpaceWorld(Transform srcTrans, bool bWorld)
	{
		if (srcTrans.GetComponent<ParticleSystem>() != null)
		{
			SerializedObject sysSo = new SerializedObject(srcTrans.GetComponent<ParticleSystem>());
			SetPropertyValue(sysSo, "moveWithTransform", !bWorld, true);
		}
		if (srcTrans.GetComponent<ParticleEmitter>() != null)
			srcTrans.GetComponent<ParticleEmitter>().useWorldSpace = bWorld;
	}

	public static bool GetMeshNormalVelocity(ParticleEmitter srcCom, out float fSetMinValue, out float fSetMaxValue)
	{
		if (srcCom != null)
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
// 			LogPropertis(sysSo, false);
			fSetMinValue = (float)GetPropertyValue(sysSo, "m_MinNormalVelocity");
			fSetMaxValue = (float)GetPropertyValue(sysSo, "m_MaxNormalVelocity");
			return true;
		}
		fSetMinValue = 0;
		fSetMaxValue = 0;
		return false;
	}

	public static bool SetMeshNormalVelocity(ParticleEmitter srcCom, float fSetMinValue, float fSetMaxValue)
	{
		if (srcCom != null && IsMeshParticleEmitter(srcCom))
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
			SetPropertyValue(sysSo, "m_MinNormalVelocity", fSetMinValue, false);
			SetPropertyValue(sysSo, "m_MaxNormalVelocity", fSetMaxValue, true);
			return true;
		}
		return false;
	}

	public static void SetShurikenSpeed(ParticleSystem srcCom, float fShurikenSpeedRate)
	{
		if (srcCom != null)
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
			SetPropertyValue(sysSo, "speed", fShurikenSpeedRate, true);
		}
	}

	public static float GetShurikenSpeed(ParticleSystem srcCom)
	{
		if (srcCom != null)
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
			return (float)GetPropertyValue(sysSo, "speed");
		}
		return 0;
	}

	public static bool GetEllipsoidSize(ParticleEmitter srcCom, out Vector3 ellipsoid, out float minEmitterRange)
	{
		if (srcCom != null && IsEllipsoidParticleEmitter(srcCom))
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
 			LogPropertis(sysSo, false);
			ellipsoid		= (Vector3)GetPropertyValue(sysSo, "m_Ellipsoid");
			minEmitterRange = (float)GetPropertyValue(sysSo, "m_MinEmitterRange");
// 			Debug.Log(ellipsoid);
// 			Debug.Log(minEmitterRange);
			return true;
		}
		ellipsoid		= Vector3.zero;
		minEmitterRange	= 0;
		return false;
	}

	public static bool SetEllipsoidSize(ParticleEmitter srcCom, Vector3 ellipsoid, float minEmitterRange)
	{
		if (srcCom != null && IsEllipsoidParticleEmitter(srcCom))
		{
			SerializedObject sysSo = new SerializedObject(srcCom);
			SetPropertyValue(sysSo, "m_Ellipsoid", ellipsoid, false);
			SetPropertyValue(sysSo, "m_MinEmitterRange", minEmitterRange, true);
// 			Debug.Log(ellipsoid);
// 			Debug.Log(minEmitterRange);
			return true;
		}
		return false;
	}

#endif

}
