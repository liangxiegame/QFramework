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
#if UNITY_EDITOR
    [ClassAPI("01.FluentAPI.CSharp", "System.Object", 0)]
    [APIDescriptionCN("针对 System.Object 提供的链式扩展，理论上任何对象都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.object can theoretically be used by any Object")]
#endif
    public static class SystemObjectExtension
    {
#if UNITY_EDITOR
        // v1 No.1 
        [MethodAPI]
        [APIDescriptionCN("将自己传到 Action 委托中")]
        [APIDescriptionEN("apply self to the Action delegate")]
        [APIExampleCode(@"
new GameObject()
        .Self(gameObj=>gameObj.name = ""Enemy"")
        .Self(gameObj=>{
            Debug.Log(gameObj.name);
        });"
        )]
#endif
        public static T Self<T>(this T self, Action<T> onDo)
        {
            onDo?.Invoke(self);
            return self;
        }
        
#if UNITY_EDITOR
        // v1 No.1.1
        [MethodAPI]
        [APIDescriptionCN("将自己传到 Func<T,T> 委托中,然后返回自己")]
        [APIDescriptionEN("apply self to the Func<T,T> delegate")]
        [APIExampleCode(@"
new GameObject()
        .Self(gameObj=>gameObj.name = ""Enemy"")
        .Self(gameObj=>{
            Debug.Log(gameObj.name);
        });"
        )]
#endif
        public static T Self<T>(this T self, Func<T,T> onDo)
        {
            return onDo.Invoke(self);
        }


#if UNITY_EDITOR
        // v1 No.2
        [MethodAPI]
        [APIDescriptionCN("判断是否为空")]
        [APIDescriptionEN("Check Is Null,return true or false")]
        [APIExampleCode(@"
var simpleObject = new object();
        
if (simpleObject.IsNull()) // simpleObject == null
{
    // do sth
}")]
#endif
        public static bool IsNull<T>(this T selfObj) where T : class
        {
            return null == selfObj;
        }

#if UNITY_EDITOR
        // v1 No.3
        [MethodAPI]
        [APIDescriptionCN("判断不是为空")]
        [APIDescriptionEN("Check Is Not Null,return true or false")]
        [APIExampleCode(@"
var simpleObject = new object();
        
if (simpleObject.IsNotNull()) // simpleObject != null
{
    // do sth
}")]
#endif
        public static bool IsNotNull<T>(this T selfObj) where T : class
        {
            return null != selfObj;
        }
      
#if UNITY_EDITOR
        // v1 No.36
        [MethodAPI]
        [APIDescriptionCN("转型")]
        [APIDescriptionEN("cast")]
        [APIExampleCode(@"
int a = 10;
Debug.Log(a.As<float>())
// 10
")]
#endif
        public static T As<T>(this object selfObj) where T : class
        {
            return selfObj as T;
        }
    }
}