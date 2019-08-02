/****************************************************************************
 * Copyright (c) 2018.8 ~ 10 liangxie
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

#if UNITY_EDITOR
using UnityEditor;
#endif
using QF.Extensions;
using UnityEngine;

using UniRx;

namespace QF
{
    public static class User
    {
        public static StringReactiveProperty Username = new StringReactiveProperty(LoadString("username"));
        public static StringReactiveProperty Password = new StringReactiveProperty(LoadString("password"));
        public static StringReactiveProperty Token = new StringReactiveProperty(LoadString("token"));

        public static bool Test = false;

        public static bool Logined
        {
            get
            {
                return Token.Value.IsNotNullAndEmpty() &&
                       Username.Value.IsNotNullAndEmpty() &&
                       Password.Value.IsNotNullAndEmpty();
            }
        }
        
        
        public static void Save()
        {
            Username.SaveString("username");
            Password.SaveString("password");
            Token.SaveString("token");
        }

        public static void Clear()
        {
            Username.Value = string.Empty;
            Password.Value = string.Empty;
            Token.Value = string.Empty;
            Save();
        }

        public static void SaveString(this StringReactiveProperty selfProperty, string key)
        {
#if UNITY_EDITOR
            EditorPrefs.SetString(key, selfProperty.Value);
#else
            PlayerPrefs.SetString(key, selfProperty.Value);
#endif
        }


        public static string LoadString(string key)
        {

#if UNITY_EDITOR
            return EditorPrefs.GetString(key, string.Empty);
#else
            return PlayerPrefs.GetString(key, string.Empty);
#endif
        }
    }
}