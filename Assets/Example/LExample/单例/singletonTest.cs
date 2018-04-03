using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public interface ISing
{
   void Run();
}

[QMonoSingletonPath("Test/SingletonTest")]
public class singletonTest : QMonoSingleton<singletonTest>, ISing
{

    public void Run()
    {
        this.LogInfo("单例");
    }

}

public class singletonTest2 : QSingleton<singletonTest2>,ISing
{
    private singletonTest2()
    {

    }

    public void Run()
    {
        Debug.Log("单例2");
    }
}

[QMonoSingletonPath("Test/SingletonTest3")]
public class singletonTest3 :MonoBehaviour,ISingleton,ISing
{
    private static singletonTest3 mInstance;

    public void OnSingletonInit()
    {

    }

    public void Run()
    {
        Debug.Log("单例3");
    }

    public static singletonTest3 Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = QMonoSingletonProperty<singletonTest3>.Instance;
            }
            return mInstance;
        }
    }
}