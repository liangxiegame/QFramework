/****************************************************************************
 * Copyright (c) 2018.12 liangxie
 * Copyright (c) 2019.1  vin129
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
    using UnityEngine;
    [RequireComponent(typeof(LuaComponent))]
    public class UILuaPanel : UIPanel, ILuaComponentBind
    {
        void Awake()
        {
            
        }

        protected void InitUI(IUIData uiData = null)
        {
            
        }

        protected void RegisterUIEvent()
        {
            
        }

        void OnEnable()
        {
            CallLuaFunction(LuaMain.FuncName.OnEnable);
        }

        void Start()
        {
            CallLuaFunction(LuaMain.FuncName.Start);
        }

        void Update()
        {
            CallLuaFunction(LuaMain.FuncName.Update);
        }

        void OnDisable()
        {
            CallLuaFunction(LuaMain.FuncName.OnDisable);
        }

        protected override void OnClose()
        {
            throw new System.NotImplementedException();
        }

        private void OnDestroy()
        {
            base.OnDestroy();
            LuaDispose();
        }

        #region LuaComponent 

        public LuaComponent LuaCp;

        public string LuaPath
        {
            get
            {
                if (LuaCp)
                    return LuaCp.LuaPath;
                else
                    return null;
            }

            set
            {
                if (LuaCp)
                {
                    LuaCp.LuaPath = value;
                    // TODO:ReLoad??
                }
            }
        }

        public void BindLuaComponent()
        {
            if (LuaCp == null)
                LuaCp = GetComponent<LuaComponent>();
            if (LuaCp == null)
            {
                Log.E("Not find LuaComponet");
                return;
            }

            if (LuaCp.Initilize(LuaPath))
                CallLuaFunction(LuaMain.FuncName.Awake);
        }

        public void LuaDispose()
        {
            CallLuaFunction(LuaMain.FuncName.OnDestroy);
            if (LuaCp)
                LuaCp.DisposeLuaTable();
        }

        public void CallLuaFunction(string funcName)
        {
            if (LuaCp == null)
                LuaCp = GetComponent<LuaComponent>();
            if (LuaCp == null)
            {
                Log.E("Not find LuaComponet");
                return;
            }
            LuaCp.CallLuaFunction(funcName);
        }


        #endregion
    }

}