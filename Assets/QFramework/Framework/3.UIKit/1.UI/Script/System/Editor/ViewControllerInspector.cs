using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(ViewController), true)]
    public class ViewControllerInspector : Editor
    {
        class LocaleText
        {
            public static string ScriptName
            {
                get { return Language.IsChinese ? "生成脚本名:" : "Script name:"; }
            }

            public static string ScriptsFolder
            {
                get { return Language.IsChinese ? "脚本生成目录:" : "Scripts Generate Folder:"; }
            }

            public static string GeneratePrefab
            {
                get { return Language.IsChinese ? "生成 Prefab" : "Genreate Prefab"; }
            }

            public static string PrefabGenreateFolder
            {
                get { return Language.IsChinese ? "Prefab 生成目录:" : "Prefab Generate Folder:"; }
            }

            public static string Generate
            {
                get { return Language.IsChinese ? " 生成代码" : " Generate Code"; }
            }
        }


        private VerticalLayout mRootLayout;

        private ViewController mCodeGenerateInfo
        {
            get { return target as ViewController; }
        }

        private void OnEnable()
        {
            mRootLayout = new VerticalLayout();

            new ButtonView(LocaleText.Generate,
                    () => { CreateViewControllerCode.DoCreateCodeFromScene((target as ViewController).gameObject); })
                .Height(30)
                .AddTo(mRootLayout);

            if (mCodeGenerateInfo.ScriptsFolder.IsNullOrEmpty())
            {
                var setting = UIKitSettingData.Load();
                mCodeGenerateInfo.ScriptsFolder = "Assets" + setting.DefaultViewControllerScriptDir;
            }

            if (mCodeGenerateInfo.PrefabFolder.IsNullOrEmpty())
            {
                var setting = UIKitSettingData.Load();
                mCodeGenerateInfo.PrefabFolder = "Assets" + setting.DefaultViewControllerPrefabDir;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            GUILayout.BeginVertical("box");

            GUILayout.Label("代码生成部分", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            });

            if (mCodeGenerateInfo.ScriptName.IsNullOrEmpty())
            {
                mCodeGenerateInfo.ScriptName = mCodeGenerateInfo.name;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptName, GUILayout.Width(150));
            mCodeGenerateInfo.ScriptName = GUILayout.TextArea(mCodeGenerateInfo.ScriptName);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptsFolder, GUILayout.Width(150));
            mCodeGenerateInfo.ScriptsFolder = GUILayout.TextArea(mCodeGenerateInfo.ScriptsFolder, GUILayout.Height(30));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mCodeGenerateInfo.GeneratePrefab =
                GUILayout.Toggle(mCodeGenerateInfo.GeneratePrefab, LocaleText.GeneratePrefab);
            GUILayout.EndHorizontal();
            
            if (mCodeGenerateInfo.GeneratePrefab)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.PrefabGenreateFolder, GUILayout.Width(150));
                mCodeGenerateInfo.PrefabFolder =
                    GUILayout.TextArea(mCodeGenerateInfo.PrefabFolder, GUILayout.Height(30));
                GUILayout.EndHorizontal();
            }

            mRootLayout.DrawGUI();

            GUILayout.EndVertical();
        }
    }
}