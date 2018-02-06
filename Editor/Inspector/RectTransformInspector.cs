/****************************************************************************
 * Copyright (c) 2017 Karsion
****************************************************************************/
// Date: 2017-08-19
// Time: 18:50
// Author: Karsion

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(RectTransform))]
public class RectTransformInspector : CustomCustomEditor
{
    private const string strButtonLeft = "ButtonLeft";
    private const string strButtonMid = "ButtonMid";
    private const string strButtonRight = "ButtonRight";
    private static GUIStyle styleMove;
    private static GUIStyle stylePivotSetup;
    private float scaleAll = 1;
    private SerializedProperty spAnchoredPosition;
    private SerializedProperty spLocalRotation;
    private SerializedProperty spLocalScale;
    private SerializedProperty spPivot;
    private SerializedProperty spSizeDelta;

    public RectTransformInspector() : base("RectTransformEditor")
    {
    }

    private void OnEnable()
    {
        spAnchoredPosition = serializedObject.FindProperty("m_AnchoredPosition");
        spSizeDelta = serializedObject.FindProperty("m_SizeDelta");
        spLocalRotation = serializedObject.FindProperty("m_LocalRotation");
        spLocalScale = serializedObject.FindProperty("m_LocalScale");
        spPivot = serializedObject.FindProperty("m_Pivot");
        scaleAll = spLocalScale.FindPropertyRelative("x").floatValue;
    }

    private static void DrawArrow(Rect lineRect)
    {
        GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
        if (lineRect.width == 1f)
        {
            GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.y + 1f, 3f, 1f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.y + 2f, 5f, 1f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.yMax - 2f, 3f, 1f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.yMax - 3f, 5f, 1f), EditorGUIUtility.whiteTexture);
        }
        else
        {
            GUI.DrawTexture(new Rect(lineRect.x + 1f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.x + 2f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.xMax - 2f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(lineRect.xMax - 3f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Event e = Event.current;
        if (e != null)
        {
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 2)
                {
                    RectTransform tf = target as RectTransform;
                    Undo.RecordObject(tf.gameObject, "SetActive");
                    tf.gameObject.SetActive(!tf.gameObject.activeSelf);
                    e.Use();
                }
            }

            if (e.type == EventType.KeyDown && e.control)
            {
                int nUnit = e.shift ? 10 : 1;
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                        Undo.RecordObjects(targets, "UpArrow");
                        foreach (Object item in targets)
                        {
                            RectTransform rtf = item as RectTransform;
                            rtf.anchoredPosition += Vector2.up*nUnit;
                        }

                        //spAnchoredPosition.vector2Value += Vector2.up*nUnit;
                        e.Use();
                        break;
                    case KeyCode.DownArrow:
                        Undo.RecordObjects(targets, "DownArrow");
                        foreach (Object item in targets)
                        {
                            RectTransform rtf = item as RectTransform;
                            rtf.anchoredPosition += Vector2.down*nUnit;
                        }

                        //spAnchoredPosition.vector2Value += Vector2.down*nUnit;
                        e.Use();
                        break;
                    case KeyCode.LeftArrow:
                        Undo.RecordObjects(targets, "LeftArrow");
                        foreach (Object item in targets)
                        {
                            RectTransform rtf = item as RectTransform;
                            rtf.anchoredPosition += Vector2.left*nUnit;
                        }

                        //spAnchoredPosition.vector2Value += Vector2.left*nUnit;
                        e.Use();
                        break;
                    case KeyCode.RightArrow:
                        Undo.RecordObjects(targets, "RightArrow");
                        foreach (Object item in targets)
                        {
                            RectTransform rtf = item as RectTransform;
                            rtf.anchoredPosition += Vector2.right*nUnit;
                        }

                        //spAnchoredPosition.vector2Value += Vector2.right*nUnit;
                        e.Use();
                        break;
                }
            }
        }

        const float fButtonWidth2 = 19;
        const float fButtonWidth = 21;
        base.OnInspectorGUI();
        if (stylePivotSetup == null)
        {
            stylePivotSetup = new GUIStyle("PreButton")
                              {
                                  normal = new GUIStyle("CN Box").normal,
                                  active = new GUIStyle("AppToolbar").normal,

                                  //contentOffset = new Vector2(0, 0),
                                  overflow = new RectOffset(),
                                  padding = new RectOffset(0, 0, -1, 0),

                                  //clipping = TextClipping.Clip,
                                  //fontSize = 30,
                                  //fixedHeight = 18,
                                  fixedWidth = fButtonWidth2
                              };

            styleMove = new GUIStyle(stylePivotSetup)
                        {
                            padding = new RectOffset(0, 0, -2, 0)
                        };
        }

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                GUIContent content;
                GUILayout.BeginHorizontal();
                {
                    EditorGUIUtility.labelWidth = 64;
                    content = new GUIContent("Scale …", "Scale all axis");
                    float newScale = EditorGUILayout.FloatField(content, scaleAll);
                    if (!Mathf.Approximately(scaleAll, newScale))
                    {
                        scaleAll = newScale;
                        spLocalScale.vector3Value = Vector3.one*scaleAll;
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    content = new GUIContent("P", "AnchoredPosition 0");
                    if (GUILayout.Button(content, strButtonLeft, GUILayout.Width(fButtonWidth)))
                    {
                        spAnchoredPosition.vector2Value = Vector2.zero;
                    }

                    content = new GUIContent("D", "DeltaSize 0");
                    if (GUILayout.Button(content, strButtonMid, GUILayout.Width(fButtonWidth)))
                    {
                        spSizeDelta.vector2Value = Vector2.zero;
                    }

                    content = new GUIContent("R", "Rotation 0");
                    if (GUILayout.Button(content, strButtonMid, GUILayout.Width(fButtonWidth)))
                    {
                        spLocalRotation.quaternionValue = Quaternion.identity;
                    }

                    content = new GUIContent("S", "Scale 0");
                    if (GUILayout.Button(content, strButtonRight, GUILayout.Width(fButtonWidth)))
                    {
                        spLocalScale.vector3Value = Vector3.one;
                        scaleAll = spLocalScale.FindPropertyRelative("x").floatValue;
                    }

                    content = new GUIContent("Round", "AnchoredPosition DeltaSize round to int");
                    if (GUILayout.Button(content))
                    {
                        Vector2 v2 = spAnchoredPosition.vector2Value;
                        spAnchoredPosition.vector2Value = new Vector2(Mathf.Round(v2.x), Mathf.Round(v2.y));
                        v2 = spSizeDelta.vector2Value;
                        spSizeDelta.vector2Value = new Vector2(Mathf.Round(v2.x), Mathf.Round(v2.y));
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Color c = GUI.color;
                GUI.color = new Color(1f, 1f, 0.5f, 1f);
                content = new GUIContent("C", "Copy component value");
                if (GUILayout.Button(content, strButtonLeft, GUILayout.Width(fButtonWidth)))
                {
                    ComponentUtility.CopyComponent(target as RectTransform);
                }

                GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
                content = new GUIContent("P", "Paste component value");
                if (GUILayout.Button(content, strButtonMid, GUILayout.Width(fButtonWidth)))
                {
                    foreach (Object item in targets)
                    {
                        ComponentUtility.PasteComponentValues(item as RectTransform);
                    }
                }

                GUI.color = c;

                //Rect rect2 = new Rect(0, 0, 1, 10);
                ////Debug.Log(EditorGUIUtility.fieldWidth);
                //GUILayout.BeginArea(new Rect(0f, 0f, EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth));
                //DrawArrow(rect2);
                //GUILayout.EndArea();

                //content = EditorGUIUtility.IconContent("RectTool On", "Fill the parent RectTransform");
                content = new GUIContent("F", "Fill the parent RectTransform");
                if (GUILayout.Button(content, strButtonMid, GUILayout.Width(fButtonWidth)))
                {
                    Undo.RecordObjects(targets, "F");
                    foreach (Object item in targets)
                    {
                        RectTransform rtf = item as RectTransform;
                        rtf.anchorMax = Vector2.one;
                        rtf.anchorMin = Vector2.zero;
                        rtf.offsetMax = Vector2.zero;
                        rtf.offsetMin = Vector2.zero;
                    }
                }

                content = new GUIContent("N", "Change to normal sizeDelta mode");
                if (GUILayout.Button(content, strButtonRight, GUILayout.Width(fButtonWidth)))
                {
                    Undo.RecordObjects(targets, "N");
                    foreach (Object item in targets)
                    {
                        RectTransform rtf = item as RectTransform;
                        Rect rect = rtf.rect;
                        rtf.anchorMax = new Vector2(0.5f, 0.5f);
                        rtf.anchorMin = new Vector2(0.5f, 0.5f);
                        rtf.sizeDelta = rect.size;
                    }
                }

                //content = EditorGUIUtility.IconContent("TextAsset Icon", "Ctrl + ↑ move 1 pixel, and Shift 10 pixel");
                content = new GUIContent("Readme", "Ctrl + ↑ move 1 pixel, and Shift 10 pixel");
                GUILayout.Label(content);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Pivot", "ProfilerPaneSubLabel"); //┌─┐
                GUILayout.BeginHorizontal(); //│┼│
                {
                    //└─┘
                    if (GUILayout.Button("◤", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(0, 1);
                    }

                    if (GUILayout.Button("", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(0.5f, 1);
                    }

                    if (GUILayout.Button("◥", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(1, 1);
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(0, 0.5f);
                    }

                    if (GUILayout.Button("+", styleMove))
                    {
                        spPivot.vector2Value = new Vector2(0.5f, 0.5f);
                    }

                    if (GUILayout.Button("", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(1, 0.5f);
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("◣", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(0, 0);
                    }

                    if (GUILayout.Button("", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(0.5f, 0);
                    }

                    if (GUILayout.Button("◢", stylePivotSetup))
                    {
                        spPivot.vector2Value = new Vector2(1, 0);
                    }
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        TransformInspector.DrawBottomPanel(targets);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        //base.OnSceneGUI();
        Event e = Event.current;
        if (e != null)
        {
            if (e.type == EventType.KeyDown && e.control)
            {
                int nUnit = e.shift ? 10 : 1;
                RectTransform rt;
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                        Undo.RecordObject(target, "Move");
                        rt = target as RectTransform;
                        rt.anchoredPosition += Vector2.up*nUnit;
                        e.Use();
                        break;
                    case KeyCode.DownArrow:
                        Undo.RecordObject(target, "Move");
                        rt = target as RectTransform;
                        rt.anchoredPosition += Vector2.down*nUnit;
                        e.Use();
                        break;
                    case KeyCode.LeftArrow:
                        Undo.RecordObject(target, "Move");
                        rt = target as RectTransform;
                        rt.anchoredPosition += Vector2.left*nUnit;
                        e.Use();
                        break;
                    case KeyCode.RightArrow:
                        Undo.RecordObject(target, "Move");
                        rt = target as RectTransform;
                        rt.anchoredPosition += Vector2.right*nUnit;
                        e.Use();
                        break;
                }
            }
        }
    }
}