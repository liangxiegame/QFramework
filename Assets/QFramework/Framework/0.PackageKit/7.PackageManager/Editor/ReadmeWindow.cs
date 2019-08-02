/****************************************************************************
 * Copyright (c) 2018.7 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QF
{
    using UnityEditor;
    using UnityEngine;

    public class ReadmeWindow : EditorWindow
    {
        private Readme mReadme;

        private Vector2 mScrollPos = Vector2.zero;

        private PackageVersion mPackageVersion;
        
        public static void Init(Readme readme,PackageVersion packageVersion)
        {

            var readmeWin = (ReadmeWindow) GetWindow(typeof(ReadmeWindow), true, packageVersion.Name, true);
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
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                GUILayout.Label("version: " + item.version, GUILayout.Width(130));
                GUILayout.Label("author: " + item.author);
                GUILayout.Label("date: " + item.date);

                GUILayout.EndHorizontal();
                GUILayout.Label(item.content);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            });

            GUILayout.EndScrollView();
        }
    }
}