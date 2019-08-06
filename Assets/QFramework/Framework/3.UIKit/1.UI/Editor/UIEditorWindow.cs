using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace QFramework
{
    public class UIEditorWindow : EditorWindow
    {
        [MenuItem("QFramework/CreateUIRoot", priority = 600)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(UIEditorWindow));
        }


        void OnEnable()
        {
            GameObject uiManager = GameObject.Find("UIRoot");

            if (uiManager)
            {
                uiManager.GetComponent<UIManager>();
            }
        }

        void OnGUI()
        {
            titleContent.text = "CreateUIRoot";

            EditorGUILayout.BeginVertical();

            UIManagerGUI();

            EditorGUILayout.EndVertical();
        }

        bool isFoldUImanager = false;
        public Vector2 m_referenceResolution = new Vector2(960, 640);
        public CanvasScaler.ScreenMatchMode m_MatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        public bool m_isOnlyUICamera = false;
        public bool m_isVertical = false;

        void UIManagerGUI()
        {
            EditorGUI.indentLevel = 0;
            isFoldUImanager = EditorGUILayout.Foldout(isFoldUImanager, "UIRoot:");
            if (isFoldUImanager)
            {
                EditorGUI.indentLevel = 1;
                m_referenceResolution = EditorGUILayout.Vector2Field("参考分辨率", m_referenceResolution);
                m_isOnlyUICamera = EditorGUILayout.Toggle("只有一个UI摄像机", m_isOnlyUICamera);
                m_isVertical = EditorGUILayout.Toggle("是否竖屏", m_isVertical);

                if (GUILayout.Button("创建UIRoot"))
                {
                    UICreateService.CreatUIManager(m_referenceResolution, m_MatchMode, m_isOnlyUICamera, m_isVertical);
                }

            }
        }
    }
}