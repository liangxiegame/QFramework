/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("FluentAPI.CSharp","System.Object")]
    [APIDescriptionCN("针对 System.Object 提供的链式扩展，理论上任何对象都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.object can theoretically be used by any Object")]
    [APIExampleCode(@"
if (someObj.IsNull())
{
    // do sth    
}

if (someObj.IsNotNull())
{
    // do sth
}

new GameObject()
    .Self(self=>self.name = ""Hello World"")
    .Self(self=>{
        newGameObject = self;
    });
"
    )]
#endif
    public static class SystemObjectExtension
    {
        [MethodAPI]
        [APIDescriptionCN("判断是否为空")]
        [APIDescriptionEN("Check Is Null,return true or false")]
        [APIExampleCode(@"
        var simpleObject = new object();
        
        if (simpleObject.IsNull()) // 等价于 simpleObject == null
        {
            // do sth
        }
        ")]
        public static bool IsNull<T>(this T selfObj) where T : class
        {
            return null == selfObj;
        }

        /// <summary>
        /// 功能：判断不是为空
        /// 示例：
        /// <code>
        /// var simpleObject = new object();
        ///
        /// if (simpleObject.IsNotNull()) // 等价于 simpleObject != null
        /// {
        ///    // do sth
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj">判断对象（this)</param>
        /// <typeparam name="T">对象的类型（可不填）</typeparam>
        /// <returns>是否不为空</returns>
        public static bool IsNotNull<T>(this T selfObj) where T : class
        {
            return null != selfObj;
        }
        
        public static T Self<T>(this T self, Action<T> onDo)
        {
            onDo?.Invoke(self);
            return self;
        }
    }
}