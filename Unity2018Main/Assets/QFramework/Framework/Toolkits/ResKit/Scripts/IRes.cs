/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2019.1 liangxie
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

using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    using System;

    public enum ResState
    {
        Waiting = 0,
        Loading = 1,
        Ready   = 2,
    }

    public static class ResLoadType
    {
        public const short AssetBundle   = 0;
        public const short ABAsset       = 1;
        public const short ABScene       = 2;
        public const short Internal      = 3;
        public const short NetImageRes   = 4;
        public const short LocalImageRes = 5;
    }



    public interface IRes : IRefCounter, IPoolType, IEnumeratorTask
    {
        string AssetName { get; }

        string OwnerBundleName { get; }

        ResState State { get; }

        UnityEngine.Object Asset { get; }

        float Progress { get; }
        Type AssetType { get; set; }

        void RegisteOnResLoadDoneEvent(Action<bool, IRes> listener);
        void UnRegisteOnResLoadDoneEvent(Action<bool, IRes> listener);

        bool UnloadImage(bool flag);

        bool LoadSync();

        void LoadAsync();

        string[] GetDependResList();

        bool IsDependResLoadFinish();

        bool ReleaseRes();
        
    }
}