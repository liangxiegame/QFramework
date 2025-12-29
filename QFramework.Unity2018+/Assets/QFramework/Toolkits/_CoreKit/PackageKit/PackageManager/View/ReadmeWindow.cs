#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class ReadmeWindow : EditorWindow
    {
        private Readme mReadme;

        private Vector2 mScrollPos = Vector2.zero;

        private PackageVersion mPackageVersion;


        public static void Init(Readme readme, PackageVersion packageVersion)
        {
            var readmeWin = (ReadmeWindow)GetWindow(typeof(ReadmeWindow), true, packageVersion.Name, true);
            readmeWin.mReadme = readme;
            readmeWin.mPackageVersion = packageVersion;
            readmeWin.position = new Rect(Screen.width / 2, Screen.height / 2, 600, 300);
            readmeWin.Show();
        }

        public void OnGUI()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(580), GUILayout.Height(300));

            GUILayout.Label("类型:" + mPackageVersion.Type);

            mReadme.items.ForEach(item =>
            {
                EasyIMGUI
                    .Custom()
                    .OnGUI(() =>
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("version: " + item.version, GUILayout.Width(130));
                        GUILayout.Label("author: " + item.author);
                        GUILayout.Label("date: " + item.date);

                        if (item.author == User.Username.Value || User.Username.Value == "liangxie")
                        {
                            if (GUILayout.Button("删除"))
                            {
//                            RenderEndCommandExecuter.PushCommand(() =>
// //                            {
//                                 new PackageManagerServer().DeletePackage(item.PackageId,
//                                     () => { mReadme.items.Remove(item); });
//                            });
                            }
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Label(item.content);
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();
                    }).DrawGUI();
            });

            GUILayout.EndScrollView();
        }
    }
}
#endif