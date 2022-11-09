/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public interface ICodeTemplate
    {
        void Write(ICodeTemplateData data);
    }
    
    public interface ICodeTemplate<TData> : ICodeTemplate where TData : ICodeTemplateData
    {
        void Write(TData data);
    }

    public abstract class CodeTemplate<TData> : ICodeTemplate<TData> where TData : class, ICodeTemplateData
    {
        public abstract void Write(TData data);

        void ICodeTemplate.Write(ICodeTemplateData data)
        {
            Write(data as TData);
        }
    }

    public interface ICodeTemplateData
    {
        
    }
}