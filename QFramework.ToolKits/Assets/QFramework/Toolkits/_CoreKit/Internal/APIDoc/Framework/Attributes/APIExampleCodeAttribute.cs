/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class APIExampleCodeAttribute : Attribute
    {
        public string Code { get; private set; }

        public APIExampleCodeAttribute(string code)
        {
            Code = code;
        }
    }
}