// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-28 15:53
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace QFramework
{
    public class SelectionHelper
    {
        [InitializeOnLoadMethod]
        private static void Start()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;
        }

        //在Hierarchy面板按鼠标中键相当于开关GameObject
        private static void HierarchyWindowItemOnGui(int instanceID, Rect selectionRect)
        {
            Event e = Event.current;
            if ((e.type == EventType.KeyDown && e.keyCode == KeyCode.Space) || (e.type == EventType.MouseDown && e.button == 2))
            {
                if (e.alt)
                {
                    Selection.gameObjects.ForEach(go => Debug.Log(go.transform.GetPath()));
                }
                else
                {
                    Undo.RecordObjects(Selection.gameObjects, "Active");
                    Selection.gameObjects.ForEach(go =>
                    {
                        go.SetActive(!go.activeSelf);
                    });
                }

                e.Use();
            }
        }

        //在Project面板按鼠标中键相当于Show In Explorer
        //In the Project panel, the middle mouse button is equivalent to "Show In Explorer".
        private static void ProjectWindowItemOnGui(string guid, Rect selectionRect)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown
                && e.button == 2
                && selectionRect.Contains(e.mousePosition))
            {
                string strPath = AssetDatabase.GUIDToAssetPath(guid);
                if (e.alt)
                {
                    Debug.Log(strPath);
                    Event.current.Use();
                    return;
                }

                if (Path.GetExtension(strPath) == string.Empty) //文件夹
                {
                    Process.Start(Path.GetFullPath(strPath));
                }
                else //文件
                {
                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(strPath));
                }

                e.Use();
            }
        }
    }
}