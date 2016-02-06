// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class ShowNcAnimationCurvePopupEditorWindow : EditorWindow
{
	protected	NcCurveAnimation	m_SelectedObj;
	protected	static Vector2		m_StartPos	= new Vector2();


	// ---------------------------------------------------------------------
//	[MenuItem("Custom/Create Curve For Object")]
	public static EditorWindow Init()
	{
		EditorWindow window = GetWindow(typeof(ShowNcAnimationCurvePopupEditorWindow));

		window.minSize	= new Vector2(300, 500);
		window.Show();
		return window;
	}

    void OnEnable()
    {
		Debug.Log("OnEnable");
   }

    void OnDisable()
    {
		Debug.Log("OnDisable");
    }

	void OnGUI()
	{
		Debug.Log("OnGUI");
	}

	// ----------------------------------------------------------------------------------
	Rect GetCurveRect(int line)
	{
		int		nLineWidth	= 100;
		int		nLineHeight	= 100;

		return new Rect(0, line * nLineHeight, nLineWidth, nLineHeight);
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------

}
