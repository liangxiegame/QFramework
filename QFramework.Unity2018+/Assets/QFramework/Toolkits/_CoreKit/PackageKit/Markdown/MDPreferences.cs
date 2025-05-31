/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class MDPreferences
    {
        private static readonly string KeyJIRA = "MG/MDV/JIRA";
        private static readonly string KeyPipedTables = "MG/MDV/PIPED";
        private static readonly string KeyPipedTablesRequireHeaderSeparator = "MG/MDV/PIPED/USEHSEP";
        private static readonly string KeyHTML = "MG/MDV/HTML";
        private static readonly string KeyDarkSkin = "MG/MDV/DarkSkin";

        private static string mJIRA = string.Empty;
        private static bool mPipedTables = true;
        private static bool mPipedTablesRequireHeaderSeparator = true;
        private static bool mStripHTML = true;
        private static bool mPrefsLoaded = false;
        private static bool mDarkSkin = EditorGUIUtility.isProSkin;

        public static string JIRA
        {
            get
            {
                LoadPrefs();
                return mJIRA;
            }
        }

        public static bool StripHTML
        {
            get
            {
                LoadPrefs();
                return mStripHTML;
            }
        }

        public static bool DarkSkin
        {
            get
            {
                LoadPrefs();
                return mDarkSkin;
            }
        }

        public static bool PipedTables
        {
            get
            {
                LoadPrefs();
                return mPipedTables;
            }
        }

        public static bool PipedTablesRequireRequireHeaderSeparator
        {
            get
            {
                LoadPrefs();
                return mPipedTablesRequireHeaderSeparator;
            }
        }

        private static void LoadPrefs()
        {
            if (!mPrefsLoaded)
            {
                mJIRA = EditorPrefs.GetString(KeyJIRA, "");
                mStripHTML = EditorPrefs.GetBool(KeyHTML, true);
                mPipedTables = EditorPrefs.GetBool(KeyPipedTables, true);
                mPipedTablesRequireHeaderSeparator = EditorPrefs.GetBool(KeyPipedTablesRequireHeaderSeparator, true);
                mDarkSkin = EditorPrefs.GetBool(KeyDarkSkin, EditorGUIUtility.isProSkin);
                mPrefsLoaded = true;
            }
        }

#if UNITY_2019_1_OR_NEWER
        public class MarkownSettings : SettingsProvider
        {
            public MarkownSettings( string path, SettingsScope scopes = SettingsScope.User, IEnumerable<string> keywords
 = null )
                : base( path, scopes, keywords )
            {
            }

            public override void OnGUI( string searchContext )
            {
                DrawPreferences();
            }
        }

        [SettingsProvider]
        static SettingsProvider MarkdownPreferences()
        {
            return new MarkownSettings( "Preferences/Markdown" );
        }
#else
        [PreferenceItem("Markdown")]
#endif
        private static void DrawPreferences()
        {
            LoadPrefs();

            EditorGUI.BeginChangeCheck();

            mJIRA = EditorGUILayout.TextField("JIRA URL", mJIRA);
            mStripHTML = EditorGUILayout.Toggle("Strip HTML", mStripHTML);
            mDarkSkin = EditorGUILayout.Toggle("Dark Skin", mDarkSkin);

            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                EditorPrefs.SetString(KeyJIRA, mJIRA);
                EditorPrefs.SetBool(KeyHTML, mStripHTML);
            }
        }
    }
}
#endif