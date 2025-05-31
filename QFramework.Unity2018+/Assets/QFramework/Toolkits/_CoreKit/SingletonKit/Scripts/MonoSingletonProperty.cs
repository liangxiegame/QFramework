/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.165
    [ClassAPI("03.SingletonKit", "MonoSingletonProperty<T>", 2, "MonoSingletonProperty<T>")]
    [APIDescriptionCN("通过属性实现的 MonoSingleton，不占用父类的位置")]
    [APIDescriptionEN("MonoSingleton, implemented through property, does not occupy the location of the parent class")]
    [APIExampleCode(@"
public class GameManager : MonoBehaviour,ISingleton
{
    public static GameManager Instance
    {
        get { return MonoSingletonProperty<GameManager>.Instance; }
    }
		
    public void Dispose()
    {
    	MonoSingletonProperty<GameManager>.Dispose();
    }
		
    public void OnSingletonInit()
    {
    	Debug.Log(name + "":"" + ""OnSingletonInit"");
    }
    
    private void Awake()
    {
        Debug.Log(name + "":"" + ""Awake"");
    }
    
    private void Start()
    {
        Debug.Log(name + "":"" + ""Start"");
    }
    
    protected void OnDestroy()
    {
        Debug.Log(name + "":"" + ""OnDestroy"");
    }
}
var gameManager = GameManager.Instance;
// GameManager:OnSingletonInit
// GameManager:Awake
// GameManager:Start
// ---------------------
// GameManager:OnDestroy
")]
#endif
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = SingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            if (SingletonCreator.IsUnitTestMode)
            {
                UnityEngine.Object.DestroyImmediate(mInstance.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(mInstance.gameObject);
            }

            mInstance = null;
        }
    }
}