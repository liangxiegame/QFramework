
using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IEnumerableAdapter:CrossBindingAdaptor
{
public override Type BaseCLRType
{
    get
    {
        return typeof(System.Collections.IEnumerable);//这是你想继承的那个类
    }
}
public override Type AdaptorType
{
    get
    {
        return typeof(Adaptor);//这是实际的适配器类
    }
}
public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
{
    return new Adaptor(appdomain, instance);//创建一个新的实例
}
//实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
public class Adaptor : System.Collections.IEnumerable, CrossBindingAdaptorType
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
    public ILTypeInstance ILInstance { get { return instance; } }
bool m_bGetEnumeratorGot = false;
IMethod m_GetEnumerator = null;
public  System.Collections.IEnumerator GetEnumerator ()
{
   if(!m_bGetEnumeratorGot)
   {
       m_GetEnumerator = instance.Type.GetMethod("GetEnumerator",0);
       m_bGetEnumeratorGot = true;
   }
          if(m_GetEnumerator != null)
       {
           return(System.Collections.IEnumerator) appdomain.Invoke(m_GetEnumerator, instance,null);
        }
       else
       {
           return null;
       } 
}
}
}