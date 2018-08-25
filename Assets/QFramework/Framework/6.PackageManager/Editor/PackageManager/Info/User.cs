/****************************************************************************
 * Copyright (c) 2018.8 liangxie
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

using UnityEditor;

namespace QFramework
{
    public class User
    {
        private static string mUsername;
        private static string mPassword;

        public static string Username
        {
            get
            {
                if (mUsername.IsNullOrEmpty())
                {
                    mUsername = EditorPrefs.GetString("username",string.Empty);
                }

                return mUsername;
            }
            set { mUsername = value; }
        }

        public static string Password
        {
            get
            {
                if (mPassword.IsNullOrEmpty())
                {
                    mPassword = EditorPrefs.GetString("password", string.Empty);
                }

                return mPassword;
            }

            set { mPassword = value; }
        }


        public static void Save()
        {
            EditorPrefs.SetString("username",mUsername);
            EditorPrefs.SetString("password",mPassword);
        }
        
    }
}