/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public interface ICodeWriter : IDisposable
    {
        /// <summary>
        /// 缩进数量
        /// </summary>
        int IndentCount { get; set; }


        void WriteFormatLine(string format, params object[] args);
        void WriteLine(string code = null);
    }
}