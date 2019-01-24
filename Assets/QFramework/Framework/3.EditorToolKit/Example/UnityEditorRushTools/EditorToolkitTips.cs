// /****************************************************************************
//  * Copyright (c) 2017 liangxie
//  * Copyright (c) 2017 Karsion
//  * 
//  * http://liangxiegame.com
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
// Date: 2018-02-07
// Time: 14:19
// Author: Karsion

using UnityEngine;

namespace QFramework.Example
{
    public class EditorToolkitTips : MonoBehaviour
    {
        [Header("In the Hierarchy panel, the middle"), Space(-10)]
        [Header("mouse button is the equivalent of"), Space(-10)]
        [Header("the toggle GameObject active."), Space(10)]
         
        [Header("In the Project panel, the middle"), Space(-10)]
        [Header("mouse button is equivalent to the"), Space(-10)]
        [Header("\"Show In Explorer\" cmd"), Space(10)]
        public Transform tfSelf;
    }
}