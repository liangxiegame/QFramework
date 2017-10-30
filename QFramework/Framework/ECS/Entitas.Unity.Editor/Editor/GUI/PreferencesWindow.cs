using System;
using System.Linq;
using Entitas.Utils;
using QFramework;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Entitas.Unity.Editor
{
    public class PreferencesWindow : EditorWindow
    {
        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences()
        {
//            EntitasEditorLayout.ShowWindow<PreferencesWindow>("Entitas " + CheckForUpdates.GetLocalVersion());
            EntitasEditorLayout.ShowWindow<PreferencesWindow>("Entitas QFramework");
        }

        Texture2D mHeaderTexture;
        Properties mProperties;
        IEntitasPreferencesDrawer[] mPreferencesDrawers;
        Vector2 mScrollViewPosition;

        Exception mConfigException;

        void OnEnable()
        {
            mHeaderTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");
            mPreferencesDrawers = AppDomain.CurrentDomain
                .GetInstancesOf<IEntitasPreferencesDrawer>()
                .OrderBy(drawer => drawer.Priority)
                .ToArray();

            try
            {
                mProperties = Preferences.HasProperties()
                    ? Preferences.LoadProperties()
                    : new Properties();
                foreach (var drawer in mPreferencesDrawers)
                {
                    drawer.Initialize(mProperties);
                }
                Preferences.SaveProperties(mProperties);
            }
            catch (Exception ex)
            {
                mConfigException = ex;
            }
        }

        void OnGUI()
        {
            DrawToolbar();
            mScrollViewPosition = EditorGUILayout.BeginScrollView(mScrollViewPosition);
            {
                DrawHeader();
                DrawPreferencesDrawers();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                Preferences.SaveProperties(mProperties);
            }
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                if (GUILayout.Button("Check for Updates", EditorStyles.toolbarButton))
                {
                    CheckForUpdates.DisplayUpdates();
                }
                if (GUILayout.Button("Chat", EditorStyles.toolbarButton))
                {
                    EntitasFeedback.EntitasChat();
                }
                if (GUILayout.Button("Docs", EditorStyles.toolbarButton))
                {
                    EntitasFeedback.EntitasDocs();
                }
                if (GUILayout.Button("Wiki", EditorStyles.toolbarButton))
                {
                    EntitasFeedback.EntitasWiki();
                }
                //if (GUILayout.Button("Donate", EditorStyles.toolbarButton)) {
                //    EntitasFeedback.Donate();
                //}
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawHeader()
        {
            var rect = EntitasEditorLayout.DrawTexture(mHeaderTexture);
            if (rect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0)
            {
                //Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/blob/develop/README.md");
            }
        }

        void DrawPreferencesDrawers()
        {
            if (mConfigException == null)
            {
                for (int i = 0; i < mPreferencesDrawers.Length; i++)
                {
                    try
                    {
                        mPreferencesDrawers[i].Draw(mProperties);
                    }
                    catch (Exception ex)
                    {
                        DrawException(ex);
                    }

                    if (i < mPreferencesDrawers.Length - 1)
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
            else
            {
                DrawException(mConfigException);
            }
        }

        void DrawException(Exception exception)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.normal.textColor = Color.red;

            EditorGUILayout.LabelField(exception.Message, style);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure Entitas.properties is set up correctly.");
        }
    }
}