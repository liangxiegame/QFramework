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

namespace QFramework
{
    using UnityEditor;
    using UnityEngine;
    
    public class ReadmeWindow :EditorWindow
    {
        private string mReadme;
        
        private Vector2 mScrollPos = Vector2.zero;

        private GUIStyle mTitleStyle;
        
        public static void Init(string readmeContent)
        {
            ReadmeWindow readmeWin = (ReadmeWindow)GetWindow (typeof(ReadmeWindow), true,"PTPlugin Reame",true);
            readmeWin.mReadme = readmeContent;
            readmeWin.position = new Rect (Screen.width / 2, Screen.height / 2, 500, 300);
            readmeWin.Show ();
        }
        
        void OnEnable()
        {
            mTitleStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                alignment = TextAnchor.LowerLeft,
                normal = {textColor = Color.white}
            };
        }

        public void OnGUI()
        {
            mScrollPos = GUILayout.BeginScrollView (mScrollPos, true,true ,GUILayout.Width(480), GUILayout.Height(300));

//            for(int i= mReadme.items.Count-1;i>=0;i--)
//            {
//                ReadmeItem item = mReadme.items [i];
                GUILayout.BeginHorizontal (EditorStyles.helpBox);
                GUILayout.BeginVertical ();
                GUILayout.BeginHorizontal ();
				 
//                GUILayout.Label ("version: "+item.version,mTitleStyle,GUILayout.Width(130));
//                GUILayout.Label ("date: "+item.date,mTitleStyle,GUILayout.Width(130));
//                GUILayout.Label ("author: "+item.author);

            GUILayout.Label ("version: v0.0.8",mTitleStyle,GUILayout.Width(130));
            GUILayout.Label ("date: ",mTitleStyle,GUILayout.Width(130));
            GUILayout.Label ("author: ");
            
                GUILayout.EndHorizontal ();
//                GUILayout.Label (item.content);
                GUILayout.Label (mReadme);
                GUILayout.EndVertical ();


                GUILayout.EndHorizontal ();
//            }

            GUILayout.EndScrollView ();

        }
    }
}