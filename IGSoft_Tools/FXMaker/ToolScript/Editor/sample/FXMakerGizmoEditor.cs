// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
/*
[CustomEditor(typeof(GameObject))]

public class FXMakerGizmoEditor : Editor
{
	protected	GameObject	m_SelectedObj;
	protected	string		m_TempPreviewTextureDir	= "Assets/ToolResources/TempPreviewTexture";

    void OnEnable()
    {
		m_SelectedObj = target as GameObject;
    }

    void OnDisable()
    {
    }

	public void OnDrawGizmo()
	{
		Debug.Log("OnDrawGizmo");
		Handles.color = Color.green;
		Handles.DrawLine(Vector3.zero, new Vector3(2,2,2));
	}

	public override void OnInspectorGUI()
	{
		Rect			rect;
		int				nAddHeight		= 25;

		DrawDefaultInspector();
	}

	public override bool HasPreviewGUI()
	{
		return true;
	}

	public override void OnPreviewGUI(Rect rect, GUIStyle background)
	{
		string filename	= NgAsset.GetPrefabThumbFilename(m_SelectedObj);
		Texture tex		= NgAsset.GetAssetPreview(m_SelectedObj);
		Texture sshot	= (Texture)AssetDatabase.LoadAssetAtPath(NgFile.CombinePath(m_TempPreviewTextureDir, filename), typeof(Texture));
		tex	= NgAsset.GetAssetPreview(m_SelectedObj);
		if (sshot != null && tex != null)
		{
			Rect leftRect	= rect;
			Rect rightRect	= rect;

			leftRect.width	= leftRect.width/2;
			rightRect.width	= rightRect.width/2;
			rightRect.x		= rightRect.x + rightRect.width;
			GUI.DrawTexture(leftRect, tex);
			GUI.DrawTexture(rightRect, sshot);
		} else {
			if (tex != null)
				GUI.DrawTexture(rect, tex);
			if (sshot != null)
				GUI.DrawTexture(rect, sshot);
		}
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------

	// Event Function -------------------------------------------------------------------

}
*/
/*







		HandleUtility.AddDefaultControl(m_SelectedGameObject.GetInstanceID());
// 		Handles.DrawLine(m_SelectedGameObject.transform.position, Vector3.zero);
// 		selTrans.rotation = Handles.RotationHandle(selTrans.rotation, Vector3.zero);

// 		selTrans.localScale = Handles.ScaleHandle(selTrans.localScale, selTrans.position, selTrans.rotation, 5.0f);
target.rot = Handles.RotationHandle (target.rot, Vector3.zero);

		GameObject target = selTrans.gameObject;

		Handles.DrawWireArc(target.transform.position, target.transform.up, -target.transform.right, 180, shieldArea);
		shieldArea = Handles.ScaleValueHandle(shieldArea, target.transform.position + target.transform.forward*shieldArea, target.transform.rotation, 1, Handles.ConeCap, 1);



*/