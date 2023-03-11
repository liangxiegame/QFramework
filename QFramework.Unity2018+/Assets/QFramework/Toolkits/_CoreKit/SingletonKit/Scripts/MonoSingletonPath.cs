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
    // v1 No.167
    [ClassAPI("03.SingletonKit", "MonoSingletonPath", 4, "MonoSingletonPathAttribute")]
    [APIDescriptionCN("修改 MonoSingleton 或者 MonoSingletonProperty 的 gameObject 名字和路径")]
    [APIDescriptionEN("Modify the gameObject name and path of the MonoSingleton or MonoSingletonProperty")]
    [APIExampleCode(@"
[MonoSingletonPath(""[MyGame]/GameManager"")]
public class GameManager : MonoSingleton<GameManager>
{
 
}
 
var gameManager = GameManager.Instance;
// ------ Hierarchy ------
// DontDestroyOnLoad
// [MyGame]
//     GameManager
")]
#endif
    [AttributeUsage(AttributeTargets.Class)] //这个特性只能标记在Class上
    public class MonoSingletonPathAttribute : Attribute
    {
        public MonoSingletonPathAttribute(string pathInHierarchy)
        {
            PathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy { get; private set; }
    }
}