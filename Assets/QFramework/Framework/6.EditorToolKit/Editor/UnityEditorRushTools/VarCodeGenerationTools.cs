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
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    internal static class VarCodeGenerationTools
    {
        //private static string[] key = {"tf", "rt"};
        //private static string[] type = {"tf", "rt"};

        private static string GetVarCodeAuto(Object obj)
        {
            StringBuilder sb = new StringBuilder();
            string strName = obj.name;
            sb.Append("public ");
            if (obj.name.StartsWith("tf", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("Transform");
            }
            else if (obj.name.StartsWith("rt", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("RectTransform");
            }
            else if (obj.name.StartsWith("go", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("GameObject");
            }
            else if (obj.name.StartsWith("txt", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("Text");
            }
            else if (obj.name.StartsWith("img", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("Image");
            }
            else if (obj.name.StartsWith("ps", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("ParticleSystem");
            }
            else if (obj.name.StartsWith("col", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("Collider");
            }
            else
            {
                sb.Append("Object");
            }

            sb.Append(" ").Append(strName).Append(";");
            return sb.ToString();
        }

        private static void GetVarCode(string strType)
        {
            TextEditor te = new TextEditor();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            for (int i = Selection.gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = Selection.gameObjects[i];
                sb.Append("public ").Append(strType).Append(" ").Append(gameObject.name).AppendLine(";");
            }

            te.text = sb.ToString();
            te.OnFocus();
            te.Copy();
        }

        [MenuItem("GameObject/Get Var Code Auto %F12", false, 0)]
        private static void GetAuto()
        {
            TextEditor te = new TextEditor();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            for (int i = Selection.gameObjects.Length - 1; i >= 0; i--)
            {
                sb.AppendLine(GetVarCodeAuto(Selection.gameObjects[i]));
            }

            te.text = sb.ToString();
            te.OnFocus();
            te.Copy();
        }

        [MenuItem("GameObject/Get Var Code By Type/GameObject", false, 0)]
        private static void GetGameObject()
        {
            GetVarCode("GameObject");
        }

        [MenuItem("GameObject/Get Var Code By Type/Transform")]
        private static void GetTransform()
        {
            GetVarCode("Transform");
        }

        [MenuItem("GameObject/Get Var Code By Type/RectTransform")]
        private static void GetRectTransform()
        {
            GetVarCode("RectTransform");
        }

        [MenuItem("GameObject/Get Var Code By Type/Image")]
        private static void GetImage()
        {
            GetVarCode("Image");
        }

        [MenuItem("GameObject/Get Var Code By Type/Text")]
        private static void GetText()
        {
            GetVarCode("Text");
        }

        [MenuItem("GameObject/Get Var Code By Type/Other(Object)")]
        private static void GetOther()
        {
            GetVarCode("Object");
        }
    }
}