/****************************************************************************
 * Copyright (c) 2020.1 liangxie
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
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public interface ISettingFromUnity
    {
        bool SimulationMode { get; set; }
        string PathPrefix { get; }

        // 外部目录  
        string PersistentDataPath { get; }

        // 内部目录
        string StreamingAssetsPath { get; }

        // 外部资源目录
        string PersistentDataPath4Res { get; }

        // 外部头像缓存目录
        string PersistentDataPath4Photo { get; }

        // 资源路径，优先返回外存资源路径
        string GetResPathInPersistentOrStream(string relativePath);

        // 上一级目录
        string GetParentDir(string dir, int floor = 1);

        void GetFileInFolder(string dirName, string fileName, List<string> outResult);
        
        ResDatas BuildEditorDataTable();

        string AssetPath2Name(string assetPath);
        
        string GetPlatformName();
        string[] GetAssetPathsFromAssetBundleAndAssetName(string abRAssetName, string assetName);
        Object LoadAssetAtPath(string assetPath, Type assetType);
        T LoadAssetAtPath<T>(string assetPath) where T : Object;
        string PasswordField(string value, GUIStyle style, GUILayoutOption[] options);
        string TextField(string value, GUIStyle style, GUILayoutOption[] options);
        string TextArea(string value, GUIStyle style, GUILayoutOption[] options);
        void AddABInfo2ResDatas(IResDatas assetBundleConfigFile, string[] abNames=null);
    }
}