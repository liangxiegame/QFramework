// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * 
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
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

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework.Example
{
    public class AttributeExample : MonoBehaviour
    {
#if UNITY_EDITOR

        /// <summary>
        /// 使用按钮做初始化操作，省去多次手动拉引用
        /// </summary>
        [Button("Init", showIfRunTime = ShowIfRunTime.Editing)]
        public int ButtonAtt1;

        private void Init()
        {
            Undo.RecordObject(this, "Init");
            TfSelf = transform;
            Colliders = GetComponentsInChildren<Collider>();
            EditorUtility.SetDirty(this);
            Debug.Log("Init Invoked");
        }

        /// <summary>
        /// 使用按钮做一些其他事情
        /// </summary>
        [Button("LogRunSpeed", "LookAtMe", showIfRunTime = ShowIfRunTime.Playing)]
        public int ButtonAtt2;

        private void LogRunSpeed()
        {
            Debug.Log("Button1 Invoked");
            Debug.Log("RunSpeed: " + RunSpeed);
        }

        private void LookAtMe()
        {
            Camera camera = Camera.main;
            if (camera)
            {
                camera.transform.LookAt(transform);
            }

            Debug.Log("LookAtMe Invoked");
        }
#endif

        public bool IsRun;

        /// <summary>
        /// 根据IsRun变量的值在检视面板上绘制RunSpeed变量
        /// </summary>
        [ShowIf("@IsRun")]
        public float RunSpeed;

        public bool IsJump;

        [ShowIf("@IsJump")] public float JumpHeight;

        [ShowIfAnd("@IsJump", "@IsRun")] public bool IsStop;

        public Collider[] Colliders;
        public Transform  TfSelf;

        /// <summary>
        /// 直接给函数显示一个Button
        /// 目前只能显示在顶部，没有做自定义sort功能
        /// </summary>
        [ButtonEx]
        private void CallInit()
        {
#if UNITY_EDITOR
            Init();
#endif
        }
    }
}