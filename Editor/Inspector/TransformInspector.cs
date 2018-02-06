/****************************************************************************
 * Copyright (c) 2017 Karsion
****************************************************************************/
// Date: 2017-12-23
// Time: 15:16
// Author: Karsion

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class TransformInspector : Editor
{
    public static Vector3 localPositionCopy = Vector3.zero;
    public static Quaternion localRotationCopy = Quaternion.identity;
    public static Vector3 loacalScaleCopy = Vector3.one;
    public static Vector3 positionCopy = Vector3.zero;
    public static Quaternion rotationCopy = Quaternion.identity;

    private static BottomPanelContents s_BottomPanelContents;

    private static Contents s_Contents;

    private float fScale = 1;
    private bool isPAll;

    //private SerializedProperty m_Position;
    //private SerializedProperty m_Rotation;
    private bool isPP;
    private bool isPR;
    private bool isPS;
    private SerializedProperty m_LocalPosition;
    private SerializedProperty m_LocalRotation;
    private SerializedProperty m_LocalScale;

    private void OnEnable()
    {
        m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
        m_LocalRotation = serializedObject.FindProperty("m_LocalRotation");
        m_LocalScale = serializedObject.FindProperty("m_LocalScale");

        //m_Position = serializedObject.FindProperty("m_Position");
        //m_Rotation = serializedObject.FindProperty("m_Rotation");

        fScale = m_LocalScale.FindPropertyRelative("x").floatValue;
        isPP = PlayerPrefs.GetInt("TransformInspectorPP", 1) == 1;
        isPR = PlayerPrefs.GetInt("TransformInspectorPR", 1) == 1;
        isPS = PlayerPrefs.GetInt("TransformInspectorPS", 1) == 1;
        isPAll = isPP && isPR && isPS;
    }

    private static Vector3 Round(Vector3 v3Value, int nDecimalPoint = 0)
    {
        int nScale = 1;
        for (int i = 0; i < nDecimalPoint; i++)
        {
            nScale *= 10;
        }

        v3Value *= nScale;
        v3Value.x = Mathf.RoundToInt(v3Value.x);
        v3Value.y = Mathf.RoundToInt(v3Value.y);
        v3Value.z = Mathf.RoundToInt(v3Value.z);
        return v3Value/nScale;
    }

    /// <summary>
    ///     Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (s_Contents == null)
        {
            s_Contents = new Contents();
        }

        EditorGUIUtility.labelWidth = 15f;
        serializedObject.Update();

        Event e = Event.current;
        if (e != null)
        {
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 2)
                {
                    Transform tf = target as Transform;
                    Undo.RecordObject(tf.gameObject, "SetActive");
                    tf.gameObject.SetActive(!tf.gameObject.activeSelf);
                }
            }
        }

        DrawPosition();
        DrawRotation();
        DrawScale();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUIUtility.labelWidth = 37f;
            float newScale = EditorGUILayout.FloatField("Scale", fScale);
            if (!Mathf.Approximately(fScale, newScale))
            {
                fScale = newScale;
                m_LocalScale.vector3Value = Vector3.one*fScale;
            }

            EditorGUILayout.LabelField("Round", GUILayout.Width(42f));
            if (GUILayout.Button(".", "MiniButtonLeft"))
            {
                m_LocalPosition.vector3Value = Round(m_LocalPosition.vector3Value);
                m_LocalScale.vector3Value = Round(m_LocalScale.vector3Value);
            }

            if (GUILayout.Button(".0", "MiniButtonMid"))
            {
                m_LocalPosition.vector3Value = Round(m_LocalPosition.vector3Value, 1);
                m_LocalScale.vector3Value = Round(m_LocalScale.vector3Value, 1);
            }

            if (GUILayout.Button(".00", "MiniButtonRight"))
            {
                m_LocalPosition.vector3Value = Round(m_LocalPosition.vector3Value, 2);
                m_LocalScale.vector3Value = Round(m_LocalScale.vector3Value, 2);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Copy
        EditorGUILayout.BeginHorizontal();
        {
            Color c = GUI.color;
            GUI.color = new Color(1f, 1f, 0.5f, 1f);
            if (GUILayout.Button("Copy", "ButtonLeft"))
            {
                localPositionCopy = m_LocalPosition.vector3Value;
                localRotationCopy = m_LocalRotation.quaternionValue;
                loacalScaleCopy = m_LocalScale.vector3Value;
                Transform t = target as Transform;
                ;
                positionCopy = t.position;
                rotationCopy = t.rotation;
            }

            GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
            if (GUILayout.Button("PLocal", "ButtonMid"))
            {
                Undo.RecordObjects(targets, "Paste Local");
                if (isPP)
                {
                    m_LocalPosition.vector3Value = localPositionCopy;
                }

                if (isPR)
                {
                    m_LocalRotation.quaternionValue = localRotationCopy;
                }

                if (isPS)
                {
                    m_LocalScale.vector3Value = loacalScaleCopy;
                }
            }

            if (GUILayout.Button("PWorld", "ButtonRight"))
            {
                Undo.RecordObjects(targets, "Paste World");
                if (isPP)
                {
                    foreach (Object o in targets)
                    {
                        Transform t = (Transform)o;
                        t.position = positionCopy;
                    }
                }

                if (isPR)
                {
                    foreach (Transform t in targets)
                    {
                        t.rotation = rotationCopy;
                    }
                }

                if (isPS)
                {
                    m_LocalScale.vector3Value = loacalScaleCopy;
                }
            }

            //GUI.color = new Color(1f, 0.75f, 0.5f, 1f);
            GetValueP(ref isPP, "P", "TransformInspectorPP");
            GetValueP(ref isPR, "R", "TransformInspectorPR");
            GetValueP(ref isPS, "S", "TransformInspectorPS");
            bool value = GUILayout.Toggle(isPAll, "All");
            if (value != isPAll)
            {
                isPAll = value;
                isPP = isPR = isPS = isPAll;
                PlayerPrefs.SetInt("TransformInspectorPP", isPAll ? 1 : 0);
                PlayerPrefs.SetInt("TransformInspectorPR", isPAll ? 1 : 0);
                PlayerPrefs.SetInt("TransformInspectorPS", isPAll ? 1 : 0);
                PlayerPrefs.Save();
            }

            GUI.color = c;
        }
        EditorGUILayout.EndHorizontal();

        DrawBottomPanel(targets);

        Transform transform = target as Transform;
        Vector3 position = transform.position;
        if (Mathf.Abs(position.x) > 100000f || Mathf.Abs(position.y) > 100000f || Mathf.Abs(position.z) > 100000f)
        {
            EditorGUILayout.HelpBox(s_Contents.floatingPointWarning, MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void GetValueP(ref bool isP, string strName, string strKey)
    {
        bool value = GUILayout.Toggle(isP, strName);
        if (value != isP)
        {
            isP = value;
            isPAll = isPP && isPR && isPS;
            PlayerPrefs.SetInt(strKey, isP ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void DrawPosition()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button(s_Contents.positionContent, GUILayout.Width(18), GUILayout.Height(17));

            EditorGUILayout.PropertyField(m_LocalPosition.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(m_LocalPosition.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(m_LocalPosition.FindPropertyRelative("z"));
            if (reset)
            {
                m_LocalPosition.vector3Value = Vector3.zero;
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawScale()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button(s_Contents.scaleContent, GUILayout.Width(18), GUILayout.Height(17));
            EditorGUILayout.PropertyField(m_LocalScale.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(m_LocalScale.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(m_LocalScale.FindPropertyRelative("z"));
            if (reset)
            {
                fScale = 1;
                m_LocalScale.vector3Value = Vector3.one;
            }
        }
        GUILayout.EndHorizontal();
    }

    internal static void DrawBottomPanel(Object[] targets)
    {
        if (s_BottomPanelContents == null)
        {
            s_BottomPanelContents = new BottomPanelContents();
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(s_BottomPanelContents.calc))
            {
                ProcessStartInfo Info = new ProcessStartInfo
                                        {
                                            FileName = "calc.exe "
                                        };
                Process.Start(Info);
            }

            if (GUILayout.Button(s_BottomPanelContents.findRef))
            {
                AutoReferencer.FindReferences(targets);
            }

            if (GUILayout.Button(s_BottomPanelContents.calledByEditor, "ButtonLeft"))
            {
                AutoReferencer.CalledByEditor(targets);
            }

            if (GUILayout.Button(s_BottomPanelContents.calledByEditorc, "ButtonRight"))
            {
                TextEditor te = new TextEditor();
                te.text =
                    @"
#if UNITY_EDITOR
    private void CalledByEditor()
    {
        
    }
#endif
";
                te.OnFocus();
                te.Copy();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private class BottomPanelContents
    {
        public readonly GUIContent calc = new GUIContent("Calc", "Run the system Calc");
        public readonly GUIContent calledByEditor = new GUIContent("CalledByEditor()", "Find and call a Function \"CalledByEditor()\" using reflection");
        public readonly GUIContent calledByEditorc = new GUIContent("c", "Copy \"CalledByEditor()\" code");
        public readonly GUIContent findRef = new GUIContent("Find Ref", "Auto find references by Property Name");
    }

    private class Contents
    {
        public readonly string floatingPointWarning = LocalizationDatabase.GetLocalizedString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");
        public readonly GUIContent positionContent = new GUIContent(LocalizationDatabase.GetLocalizedString("P"), LocalizationDatabase.GetLocalizedString("The local position of this Game Object relative to the parent. Click the button to 0."));

        public readonly GUIContent rotationContent = new GUIContent(LocalizationDatabase.GetLocalizedString("R"), LocalizationDatabase.GetLocalizedString("The local rotation of this Game Object relative to the parent. Click the button to 0."));

        public readonly GUIContent scaleContent = new GUIContent(LocalizationDatabase.GetLocalizedString("S"), LocalizationDatabase.GetLocalizedString("The local scaling of this Game Object relative to the parent. Click the button to 1."));
    }

    #region Rotation is ugly as hell... since there is no native support for quaternion property drawing
    [Flags]
    private enum Axes
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7
    }

    private static Axes CheckDifference(Transform t, Vector3 original)
    {
        Vector3 next = t.localEulerAngles;

        Axes axes = Axes.None;

        if (Differs(next.x, original.x))
        {
            axes |= Axes.X;
        }
        if (Differs(next.y, original.y))
        {
            axes |= Axes.Y;
        }
        if (Differs(next.z, original.z))
        {
            axes |= Axes.Z;
        }

        return axes;
    }

    private Axes CheckDifference(SerializedProperty property)
    {
        Axes axes = Axes.None;

        if (property.hasMultipleDifferentValues)
        {
            Vector3 original = property.quaternionValue.eulerAngles;

            foreach (Object obj in serializedObject.targetObjects)
            {
                axes |= CheckDifference(obj as Transform, original);
                if (axes == Axes.All)
                {
                    break;
                }
            }
        }
        return axes;
    }

    /// <summary>
    ///     Draw an editable float field.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="hidden">Whether to replace the value with a dash</param>
    /// <param name="greyedOut">Whether the value should be greyed out or not</param>
    /// <param name="name"></param>
    /// <param name="opt"></param>
    private static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
    {
        float newValue = value;
        GUI.changed = false;

        if (!hidden)
        {
            if (greyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
                GUI.color = Color.white;
            }
            else
            {
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
            }
        }
        else if (greyedOut)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            GUI.color = Color.white;
        }
        else
        {
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
        }

        if (GUI.changed && Differs(newValue, value))
        {
            value = newValue;
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Because Mathf.Approximately is too sensitive.
    /// </summary>
    private static bool Differs(float a, float b)
    {
        return Mathf.Abs(a - b) > 0.0001f;
    }

    private static float WrapAngle(float angle)
    {
        while (angle > 180f)
        {
            angle -= 360f;
        }
        while (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }

    private void DrawRotation()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button(s_Contents.rotationContent, GUILayout.Width(18), GUILayout.Height(17));

            Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;
            visible.x = WrapAngle(visible.x);
            visible.y = WrapAngle(visible.y);
            visible.z = WrapAngle(visible.z);
            Axes changed = CheckDifference(m_LocalRotation);
            Axes altered = Axes.None;

            GUILayoutOption opt = GUILayout.MinWidth(30f);

            if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, false, opt))
            {
                altered |= Axes.X;
            }
            if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, false, opt))
            {
                altered |= Axes.Y;
            }
            if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt))
            {
                altered |= Axes.Z;
            }

            if (reset)
            {
                m_LocalRotation.quaternionValue = Quaternion.identity;
            }
            else if (altered != Axes.None)
            {
                Undo.RecordObjects(serializedObject.targetObjects, "Change Rotation");
                foreach (Object obj in serializedObject.targetObjects)
                {
                    Transform t = obj as Transform;
                    Vector3 v = t.localEulerAngles;

                    if ((altered & Axes.X) != 0)
                    {
                        v.x = visible.x;
                    }
                    if ((altered & Axes.Y) != 0)
                    {
                        v.y = visible.y;
                    }
                    if ((altered & Axes.Z) != 0)
                    {
                        v.z = visible.z;
                    }

                    t.localEulerAngles = v;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
    #endregion Rotation is ugly as hell... since there is no native support for quaternion property drawing
}