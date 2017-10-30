/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

#if !NONE_LUA_SUPPORT
namespace QFramework
{
    using UnityEngine;
#if SLUA_SUPPORT
    using SLua;

    [CustomLuaClassAttribute]
#endif

    public class QLuaUIBehaviour : QUIBehaviour
    {
        protected LuaTable mLuaScript;

        public Transform Transform
        {
            get { return transform; }
        }
        
        public void SetBehaviour(LuaTable luaScript)
        {
            mLuaScript = luaScript;

            mLuaScript["this"] = this;
            mLuaScript["transform"] = transform;
            mLuaScript["gameObject"] = gameObject;

            CallMethod("Init", mLuaScript);
        }

        public object CallMethod(string function, params object[] args)
        {
            if (mLuaScript == null || mLuaScript[function] == null || !(mLuaScript[function] is LuaFunction))
                return null;

            LuaFunction func = (LuaFunction) mLuaScript[function];

            if (func == null) return null;
            try
            {
                if (args != null)
                {
                    return func.call(args);
                }
                func.call();
                return null;
            }
            catch (System.Exception e)
            {
                Log.w(FormatException(e), gameObject);
            }
            return null;
        }


        public void CloseSelf()
        {
            base.CloseSelf();
        }
        
//        protected void Update()
//        {
//            if (null != mLuaScript)
//            {
//                CallMethod("Update", mLuaScript);
//            }
//        }

//public class LuaBehaviour : MonoBehaviour
//{
//	[System.NonSerialized]
//    public bool usingUpdate = false;
//    [System.NonSerialized]
//    public bool usingFixedUpdate = false;
//    
//    protected LuaTable table;
//
//    //保存的lua 数据存取
//    public LuaTable data{get;set;} 
//
//    protected Lua env
//    {
//        get 
//        {
//            return API.env;
//        }
//    }
//

        protected override void OnClose()
        {
            base.OnClose();

            CallMethod("OnClose", mLuaScript);

            if (mLuaScript != null)
            {
                mLuaScript.Dispose();
            }

        }

//    //加载脚本文件
//    public void DoFile(string fn, System.Action complete=null)
//    {
//        StartCoroutine(DoMeFile(fn, complete));        
//    }
//
//
//    IEnumerator DoMeFile(string fn, System.Action complete=null)
//    {
//        yield return new WaitForEndOfFrame();
//        while(env==null || !env.isReady)
//        {
//            yield return new WaitForEndOfFrame();
//        }      
//
//        try
//        {            
//            object chunk = env.DoFile(fn);            
//
//            if (chunk != null && (chunk is LuaTable))
//            {
//                setBehaviour((LuaTable)chunk);
//            }
//
//        }
//        catch (System.Exception e)
//        {       
//            Debug.LogError(FormatException(e), gameObject);
//        }
//
//        if(complete!=null)
//        complete();
//    }
//
//    //获取绑定的lua脚本
//    public LuaTable GetChunk()
//    {       
//        return table;
//    }
//
//    //设置lua脚本可直接使用变量
//    public void SetEnv(string key, object val, bool isGlobal)
//    {
//        if (isGlobal)
//        {
//            env[key] = val;
//        }
//        else
//        {
//            if (table != null)
//            {
//                table[key] = val;
//            }
//        }
//    }
//
//    //延迟执行
//    public void LuaInvoke(float delaytime,LuaFunction func,params object[] args)
//    {
//        StartCoroutine(doInvoke(delaytime, func, args));
//    }
//    private IEnumerator doInvoke(float delaytime, LuaFunction func, params object[] args)
//    {
//        yield return new  WaitForSeconds(delaytime);
//        if (args != null)
//        {
//            func.call(args);
//        }
//        else
//        {
//            func.call(); 
//        }
//    }
//
//    //协程
//    public void RunCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
//    {
//        StartCoroutine(doCoroutine(ins, func, args));
//    }
//    public void CancelCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
//    {
//        StopCoroutine(doCoroutine(ins, func, args));      
//    }
//    private IEnumerator doCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
//    {
//        yield return ins;
//        if (args != null)
//        {
//            func.call(args);
//        }
//        else
//        {
//            func.call();
//        }
//    }
//

//
//    public object CallMethod(string function)
//    {
//        return CallMethod(function, null);
//    }
//
        public static string FormatException(System.Exception e)
        {
            string source = (string.IsNullOrEmpty(e.Source))
                ? "<no source>"
                : e.Source.Substring(0, e.Source.Length - 2);
            return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
        }

//    //挂接回调调用函数：一般用于jni或者invoke等操作
//    public void MeMessage(object arg)
//    {
//        Messenger.Broadcast<object>(this.name + "MeMessage", arg);
//    }
//
//    //挂接回调调用函数：一般用于jin或者invoke等操作
//    public void MeMessageAll(object arg)
//    {
//        Messenger.Broadcast<object>("MeMessageAll", arg);
//    }
//}
    }
}
#endif