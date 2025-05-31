/****************************************************************************
 * Copyright (c) 2021.3 liangxie
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
using UnityEngine.Events;
using UnityEngine.UI;

namespace QFramework
{
    public static class InputFieldEnterSubmitExtensions
    {
        public static void OnEnterSubmit(this InputField inputField, UnityAction action)
        {
            var trigger = inputField.GetComponent<InputFieldEnterSubmitTrigger>();

            if (!trigger)
            {
                trigger = inputField.gameObject.AddComponent<InputFieldEnterSubmitTrigger>();
            }
            
            trigger.OnEnterSubmit.AddListener(action);
        }
    }
    
    [RequireComponent(typeof(InputField))]
    public class InputFieldEnterSubmitTrigger : MonoBehaviour
    {
        private InputField mInputField;

        public readonly UnityEvent OnEnterSubmit = new UnityEvent();

        private void OnDestroy()
        {
            OnEnterSubmit.RemoveAllListeners();
        }

        private void Awake()
        {
            mInputField = GetComponent<InputField>();
        }

        void OnEnable()
        {
            mInputField.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;

            mInputField.onValidateInput += CheckForEnter;
        }


        void OnDisable()
        {
            mInputField.onValidateInput -= CheckForEnter;
        }

        // 回车解决方案
        private char CheckForEnter(string text, int charIndex, char addedChar)
        {
            if (addedChar == '\n')
            {
                OnEnterSubmit.Invoke();
                return '\0';
            }

            return addedChar;
        }
    }
}