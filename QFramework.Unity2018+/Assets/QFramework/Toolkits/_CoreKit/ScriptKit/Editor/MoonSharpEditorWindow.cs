/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using MoonSharp.Interpreter;
using UnityEditor;

namespace QFramework
{
    public class MoonSharpEditorWindow : EditorWindow
    {
        public static MoonSharpEditorWindow CreateWithTable(DynValue table)
        {
            var window = CreateInstance<MoonSharpEditorWindow>();
            window.mTable = table;
            return window;
        }

        private DynValue mTable;

        private void OnGUI()
        {
            (mTable.Table["OnGUI"] as DynValue)?.Function.Call();
        }
    }
}

#endif