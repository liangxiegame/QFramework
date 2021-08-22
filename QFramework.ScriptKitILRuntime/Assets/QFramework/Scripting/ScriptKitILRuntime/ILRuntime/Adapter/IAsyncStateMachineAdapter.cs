using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IAsyncStateMachineAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(System.Runtime.CompilerServices.IAsyncStateMachine); //这是你想继承的那个类
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor); //这是实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance); //创建一个新的实例
    }

//实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : System.Runtime.CompilerServices.IAsyncStateMachine, CrossBindingAdaptorType
    {
        ILTypeInstance instance;

        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        //缓存这个数组来避免调用时的GC Alloc
        object[] param1 = new object[1];

        public Adaptor()
        {
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance
        {
            get { return instance; }
        }

        bool m_bMoveNextGot = false;
        IMethod m_MoveNext = null;

        public void MoveNext()
        {
            if (!m_bMoveNextGot)
            {
                m_MoveNext = instance.Type.GetMethod("MoveNext", 0);
                m_bMoveNextGot = true;
            }

            if (m_MoveNext != null)
            {
                appdomain.Invoke(m_MoveNext, instance, null);
            }
            else
            {
            }
        }

        bool m_bSetStateMachineGot = false;
        IMethod m_SetStateMachine = null;

        public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine arg0)
        {
            if (!m_bSetStateMachineGot)
            {
                m_SetStateMachine = instance.Type.GetMethod("SetStateMachine", 1);
                m_bSetStateMachineGot = true;
            }

            if (m_SetStateMachine != null)
            {
                appdomain.Invoke(m_SetStateMachine, instance, arg0);
            }
            else
            {
            }
        }
    }
}