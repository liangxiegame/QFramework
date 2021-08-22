using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;


public class CoroutineAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return null;
        }
    }

    public override Type[] BaseCLRTypes
    {
        get
        {
            //跨域继承只能有1个Adapter，因此应该尽量避免一个类同时实现多个外部接口，对于coroutine来说是IEnumerator<object>,IEnumerator和IDisposable，
            //ILRuntime虽然支持，但是一定要小心这种用法，使用不当很容易造成不可预期的问题
            //日常开发如果需要实现多个DLL外部接口，请在Unity这边先做一个基类实现那些个接口，然后继承那个基类
            return new Type[] { typeof(IEnumerator<object>), typeof(IEnumerator), typeof(IDisposable) };
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }
    //Coroutine生成的类实现了IEnumerator<System.Object>, IEnumerator, IDisposable,所以都要实现，这个可以通过reflector之类的IL反编译软件得知
    internal class Adaptor : IEnumerator<System.Object>, IEnumerator, IDisposable, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        IMethod mCurrentMethod;
        bool mCurrentMethodGot;
        public object Current
        {
            get
            {
                if (!mCurrentMethodGot)
                {
                    mCurrentMethod = instance.Type.GetMethod("get_Current", 0);
                    if (mCurrentMethod == null)
                    {
                        //这里写System.Collections.IEnumerator.get_Current而不是直接get_Current是因为coroutine生成的类是显式实现这个接口的，通过Reflector等反编译软件可得知
                        //为了兼容其他只实现了单一Current属性的，所以上面先直接取了get_Current
                        mCurrentMethod = instance.Type.GetMethod("System.Collections.IEnumerator.get_Current", 0);
                    }
                    mCurrentMethodGot = true;
                }

                if (mCurrentMethod != null)
                {
                    var res = appdomain.Invoke(mCurrentMethod, instance, null);
                    return res;
                }
                else
                {
                    return null;
                }
            }
        }

        IMethod mDisposeMethod;
        bool mDisposeMethodGot;
        public void Dispose()
        {
            if (!mDisposeMethodGot)
            {
                mDisposeMethod = instance.Type.GetMethod("Dispose", 0);
                if (mDisposeMethod == null)
                {
                    mDisposeMethod = instance.Type.GetMethod("System.IDisposable.Dispose", 0);
                }
                mDisposeMethodGot = true;
            }

            if (mDisposeMethod != null)
            {
                appdomain.Invoke(mDisposeMethod, instance, null);
            }
        }

        IMethod mMoveNextMethod;
        bool mMoveNextMethodGot;
        public bool MoveNext()
        {
            if (!mMoveNextMethodGot)
            {
                mMoveNextMethod = instance.Type.GetMethod("MoveNext", 0);
                mMoveNextMethodGot = true;
            }

            if (mMoveNextMethod != null)
            {
                return (bool)appdomain.Invoke(mMoveNextMethod, instance, null);
            }
            else
            {
                return false;
            }
        }

        IMethod mResetMethod;
        bool mResetMethodGot;
        public void Reset()
        {
            if (!mResetMethodGot)
            {
                mResetMethod = instance.Type.GetMethod("Reset", 0);
                mResetMethodGot = true;
            }

            if (mResetMethod != null)
            {
                appdomain.Invoke(mResetMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}
