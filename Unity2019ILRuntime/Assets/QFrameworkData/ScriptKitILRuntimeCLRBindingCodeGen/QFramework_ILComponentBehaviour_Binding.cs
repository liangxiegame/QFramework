using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class QFramework_ILComponentBehaviour_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(QFramework.ILComponentBehaviour);

            field = type.GetField("Script", flag);
            app.RegisterCLRFieldGetter(field, get_Script_0);
            app.RegisterCLRFieldSetter(field, set_Script_0);
            field = type.GetField("OnDestroyAction", flag);
            app.RegisterCLRFieldGetter(field, get_OnDestroyAction_1);
            app.RegisterCLRFieldSetter(field, set_OnDestroyAction_1);


        }



        static object get_Script_0(ref object o)
        {
            return ((QFramework.ILComponentBehaviour)o).Script;
        }
        static void set_Script_0(ref object o, object v)
        {
            ((QFramework.ILComponentBehaviour)o).Script = (System.Object)v;
        }
        static object get_OnDestroyAction_1(ref object o)
        {
            return ((QFramework.ILComponentBehaviour)o).OnDestroyAction;
        }
        static void set_OnDestroyAction_1(ref object o, object v)
        {
            ((QFramework.ILComponentBehaviour)o).OnDestroyAction = (System.Action)v;
        }


    }
}
