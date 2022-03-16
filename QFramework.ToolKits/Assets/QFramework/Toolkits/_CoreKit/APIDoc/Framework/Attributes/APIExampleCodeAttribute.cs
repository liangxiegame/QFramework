/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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