// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-28 15:19
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

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    /*
-------------------------------------------------------------------------------------------------------------------------------------------

    To define a hot-key, use the following special characters: 

        % (Ctrl on Windows, Cmd on OS X)
        # (Shift)
        & (Alt)
        _ (no key modifiers). 

    For example, to define the hot-key Shift-Alt-g use "#&g". To define the hot-key g and no key modifiers pressed use "_g".

    Some special keys are supported, for example "#LEFT" would map to Shift-left. The keys supported like this are: 

        LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.
    
    eg. [MenuItem("GameObject/UI/img &g")]

-------------------------------------------------------------------------------------------------------------------------------------------
*/

    /// <summary>
    ///     创建 Text、Image 的时候默认不选中 raycastTarget 等
    /// </summary>
    public class OverrideCreateUIMenuItem
    {
        /// <summary>
        ///     第一次创建UI元素时。没有canvas、EventSystem全部要生成，Canvas作为父节点
        ///     之后再空的位置上建UI元素会自己主动加入到Canvas下
        ///     在非UI树下的GameObject上新建UI元素也会 自己主动加入到Canvas下（默认在UI树下）
        ///     加入到指定的UI元素下
        /// </summary>
        [MenuItem("GameObject/UI/img (No Raycast Target)")]
        private static void CreatImages()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject canvasObj = SecurityCheck();
            GameObject img = Image();

            if (!activeTransform) // 在根文件夹创建的， 自己主动移动到 Canvas下
            {
                img.transform.SetParent(canvasObj.transform, false);
                img.layer = canvasObj.layer;
            }
            else
            {
                if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    img.transform.SetParent(canvasObj.transform, false);
                    img.layer = canvasObj.layer;
                }
                else
                {
                    img.transform.SetParent(activeTransform, false);
                    img.layer = activeTransform.gameObject.layer;
                }
            }
        }

        private static GameObject Image()
        {
            GameObject go = new GameObject("img", typeof(Image));
            go.GetComponent<Image>().raycastTarget = false;
            Selection.activeGameObject = go;
            return go;
        }

        [MenuItem("GameObject/UI/txt (No Raycast Target)")]
        private static void CreatTexts()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject canvasObj = SecurityCheck();
            GameObject txt = Text();
            if (!activeTransform) // 在根文件夹创建的。 自己主动移动到 Canvas下
            {
                txt.transform.SetParent(canvasObj.transform, false);
                txt.gameObject.layer = canvasObj.layer;
            }
            else
            {
                if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    txt.transform.SetParent(canvasObj.transform, false);
                    txt.gameObject.layer = canvasObj.layer;
                }
                else
                {
                    txt.transform.SetParent(activeTransform, false);
                    txt.gameObject.layer = activeTransform.gameObject.layer;
                }
            }
        }

        private static GameObject Text()
        {
            GameObject go = new GameObject("txt", typeof(Text));
            Text text = go.GetComponent<Text>();
            text.raycastTarget = false;
            text.supportRichText = false;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            //text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Arts/Fonts/zh_cn.TTF"); // 默认字体
            Selection.activeGameObject = go;
            return go;
        }

        // 假设第一次创建UI元素 可能没有 Canvas、EventSystem对象！
        private static GameObject SecurityCheck()
        {
            Canvas cv = Object.FindObjectOfType<Canvas>();
            GameObject canvas = !cv ? new GameObject("Canvas", typeof(Canvas)) : cv.gameObject;
            if (!Object.FindObjectOfType<EventSystem>())
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GameObject("EventSystem", typeof(EventSystem));
            }

            canvas.layer = LayerMask.NameToLayer("UI");
            return canvas;
        }
    }
}