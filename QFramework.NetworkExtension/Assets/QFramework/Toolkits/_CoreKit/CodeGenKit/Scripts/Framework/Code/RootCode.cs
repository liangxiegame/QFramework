/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{
    public class RootCode : ICodeScope
    {
        private List<ICode> mCodes = new List<ICode>();

        public List<ICode> Codes
        {
            get { return mCodes; }
            set { mCodes = value; }
        }


        public void Gen(ICodeWriter writer)
        {
            foreach (var code in Codes)
            {
                code.Gen(writer);
            }
        }
    }
}