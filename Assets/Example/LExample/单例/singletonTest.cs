/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

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