/****************************************************************************
 * Copyright (c) 2019.1 liangxie
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

using System;
using System.Collections;
using UnityEngine;

namespace QFramework
{
    public class ResKit
    {
        private static IResDatas mResDatas = null;
        
        /// <summary>
        /// 默认
        /// </summary>
        private static Func<IResDatas> mResDataFactory = () =>  new ResDatas();

        public static Func<IResDatas> ResDataFactory
        {
            set { mResDataFactory = value; }
        }

        /// <summary>
        /// 获取自定义的 资源信息
        /// </summary>
        /// <returns></returns>
        public static IResDatas ResDatas
        {
            get
            {
                if (mResDatas == null)
                {
                    mResDatas = mResDataFactory.Invoke();
                }

                return mResDatas;
            }
            set { mResDatas = value; }
        }


        public static bool LoadResFromStreammingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadResFromStreammingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadResFromStreammingAssetsPath", value ? 1 : 0); }
        }
        
        public static void Init()
        {
            ResMgr.Init();
        }

        public static IEnumerator InitAsync()
        {
            yield return ResMgr.InitAsync();
        }

    }
}