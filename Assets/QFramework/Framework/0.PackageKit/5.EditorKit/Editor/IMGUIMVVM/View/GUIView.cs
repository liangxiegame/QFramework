/****************************************************************************
 * Copyright (c) 2017 liangxie
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
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GUIView : IOnGUIView
    {
        #region transform

        //private Matrix4x4 mTransform = Matrix4x4.identity;

        //public IOnGUIView Scale(float scale)
        //{
        //    mTransform = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
        //    return this;
        //}
        
        #endregion
        
        #region GUILayoutOptions

        protected float mWidth = 0.0f;
        public float Width
        {
            get { return mWidth; }
            set
            {
                mWidth = value;
                mLayoutOptions.Add(GUILayout.Width(mWidth));
            }
        }
        
        protected float mHeight = 0.0f;
        
        public float Height
        {
            get { return mHeight; }
            set
            {
                mHeight = value;
                mLayoutOptions.Add(GUILayout.Height(mHeight));
            }
        }

        protected List<GUILayoutOption> mLayoutOptions = new List<GUILayoutOption>();
        #endregion
        
        private readonly List<IOnGUIView> mChildren = new List<IOnGUIView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public virtual void AddChild(IOnGUIView editorView)
        {
            mChildren.Add(editorView);
        }

        public virtual void RemoveChild(IOnGUIView editorView)
        {
            mChildren.Remove(editorView);
        }

        public List<IOnGUIView> Children
        {
            get { return mChildren; }
        }

        public virtual void OnGUI()
        {
            if (Visible)
            {
//                var cachedMatrix = GUI.matrix;
//                GUI.matrix = mTransform;
                mChildren.ForEach(childView => childView.OnGUI());
//                GUI.matrix = cachedMatrix;
            }
        }

        public IOnGUIView End()
        {
            throw new System.NotImplementedException();
        }
    }
}