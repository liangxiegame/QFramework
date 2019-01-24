/****************************************************************************
 * Copyright (c) 2017 ~ 2018.7 Karsion
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

using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{

    /*-------------------------------------------------------------------------------------------------------------------------------------------
        To define a hot-key, use the following special characters: 
            % (Ctrl on Windows, Cmd on OS X)
            # (Shift)
            & (Alt)
            _ (no key modifiers). 
        For example, to define the hot-key Shift-Alt-g use "#&g". To define the hot-key g and no key modifiers pressed use "_g".
        Some special keys are supported, for example "#LEFT" would map to Shift-left. The keys supported like this are: 
            LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.
        eg. [MenuItem("GameObject/UI/img &g")]
    -------------------------------------------------------------------------------------------------------------------------------------------*/

    internal static class AssetsMenuItem
    {
        [MenuItem("GameObject/Copy/名称")]
        private static void CopyName()
        {
            TextEditor te = new TextEditor();
            te.text = Selection.activeObject.name;
            te.OnFocus();
            te.Copy();

            Debug.Log("Name: " + te.text);
        }

        //[MenuItem("Assets/Copy/Copy FilePath")]
        //private static void CopyFilePath()
        //{
        //    TextEditor editor = new TextEditor();
        //    editor.text = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    editor.SelectAll();
        //    editor.Copy();

        //    Debug.Log("FilePath: " + editor.text);
        //}

        #region Copy Hierarchy Node Path
        [MenuItem("GameObject/Copy/节点路径 ..]")]//, false, 0
        static void CopyNodePathFunc()
        {
            string nodePath = "";
            GetNodePath(Selection.activeGameObject.transform, ref nodePath);

            Debug.Log("...] " + nodePath);

            //复制到剪贴板
            TextEditor editor = new TextEditor();
            editor.text = nodePath;
            editor.SelectAll();
            editor.Copy();
        }

        [MenuItem("GameObject/Copy/节点区域路径 [...]")]
        static void CopyNodePath()
        {
            string nodePath = "";

            //获取最后一个obj
            if (Selection.gameObjects.Length >= 2)
            {
                string nodePathMin = "";
                string nodePathMax = "";
                string nodeMinName = "";

                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    string path = "";
                    GetNodePath(Selection.gameObjects[i].transform, ref path);

                    if (i == 0) { nodePathMin = path; nodeMinName = Selection.gameObjects[i].name; }

                    if (path.Length > nodePathMax.Length)
                    {
                        nodePathMax = path;
                    }
                    if (path.Length < nodePathMin.Length)
                    {
                        nodePathMin = path;
                        nodeMinName = Selection.gameObjects[i].name;
                    }
                }

                int index =  nodePathMax.IndexOf(nodeMinName);
                nodePath = nodePathMax.Substring(index, nodePathMax.Length - index);
            }
            else
            {
                nodePath = Selection.activeGameObject.name;
            }

            Debug.Log("[...] " + nodePath);

            //复制到剪贴板
            TextEditor editor = new TextEditor();
            editor.text = nodePath;
            editor.SelectAll();
            editor.Copy();
        }

        static void GetNodePath(Transform trans, ref string path)
        {
            if (path == "")
            {
                path = trans.name;
            }
            else
            {
                path = trans.name + "/" + path;
            }

            if (trans.parent != null)
            {
                GetNodePath(trans.parent, ref path);
            }
        }
        #endregion


    }
}