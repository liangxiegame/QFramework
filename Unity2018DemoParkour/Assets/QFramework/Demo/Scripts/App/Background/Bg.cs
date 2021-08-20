/****************************************************************************
 * Copyright (c) 2018.3 ~ 2021.8 liangxie
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

using UnityEngine;

namespace QFramework.PlatformRunner
{
    public class Bg
    {
        public static void Check(Transform backA, Transform backB, Transform foreA, Transform foreB,float width)
        {
            if (backA.GetPosition().x <= -width)
            {
                backA.PositionX(backB.GetPosition().x + width);
            }
            
            if (backB.GetPosition().x <= -width)
            {
                backB.PositionX(backA.GetPosition().x + width);
            }
            
            if (foreA.GetPosition().x <= -width)
            {
                foreA.PositionX(foreB.GetPosition().x + width);
            }
            
            if (foreB.GetPosition().x <= -width)
            {
                foreB.PositionX(foreA.GetPosition().x + width);
            }
        }
    }
}