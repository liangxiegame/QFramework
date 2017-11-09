/****************************************************************************
 * Copyright (c) 2017 liangxie
 *
 * http://unitylist.com/r/axc/input-system
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
    using System;
    using UnityEngine;

    [QMonoSingletonPath("[Framework]/InputSystem")]
    public class InputSystem : QMonoSingleton<InputSystem>
    {
        public Action OnKeyWDown = delegate { };
        public Action OnKeyW = delegate { };
        public Action OnKeyWUp = delegate { };
        
        public Action OnKeyADown = delegate { };
        public Action OnKeyA = delegate { };
        public Action OnKeyAUp = delegate { };
        
        public Action OnKeySDown = delegate { };
        public Action OnKeyS = delegate { };
        public Action OnKeySUp = delegate { };
        
        public Action OnKeyDDown = delegate { };
        public Action OnKeyD = delegate { };
        public Action OnKeyDUp = delegate { };
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnKeyWDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.W))
            {
                OnKeyW.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                OnKeyWUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                OnKeyADown.InvokeGracefully(true);
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                OnKeyA.InvokeGracefully(false);
            }
            
            if (Input.GetKeyUp(KeyCode.A))
            {
                OnKeyAUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                OnKeySDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnKeyS.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                OnKeySUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                OnKeyDDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.D))
            {
                OnKeyD.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                OnKeyDUp.InvokeGracefully(false);
            }
        }
    }
}