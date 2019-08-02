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

namespace QF.Action
{
    using System.Collections;
    using UnityEngine;
    
    public static class IActionExtension
    {
        public static T ExecuteNode<T>(this T selBehaviour, IAction commandNode) where T : MonoBehaviour
        {
            selBehaviour.StartCoroutine(commandNode.Execute());
            return selBehaviour;
        }

        public static void Delay<T>(this T selfBehaviour, float seconds, System.Action delayEvent) where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayAction.Allocate(seconds, delayEvent));
        }
        
        public static IEnumerator Execute(this IAction selfNode)
        {
            if (selfNode.Finished) selfNode.Reset();
            
            while (!selfNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        }
    }
}