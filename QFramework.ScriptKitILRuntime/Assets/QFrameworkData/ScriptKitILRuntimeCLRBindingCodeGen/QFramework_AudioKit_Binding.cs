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
    unsafe class QFramework_AudioKit_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(QFramework.AudioKit);
            args = new Type[]{typeof(System.String), typeof(System.Boolean), typeof(System.Action<QFramework.AudioPlayer>), typeof(System.Int32)};
            method = type.GetMethod("PlaySound", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlaySound_0);

            field = type.GetField("Settings", flag);
            app.RegisterCLRFieldGetter(field, get_Settings_0);


        }


        static StackObject* PlaySound_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @customEventId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<QFramework.AudioPlayer> @callBack = (System.Action<QFramework.AudioPlayer>)typeof(System.Action<QFramework.AudioPlayer>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Boolean @loop = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.String @soundName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = QFramework.AudioKit.PlaySound(@soundName, @loop, @callBack, @customEventId);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_Settings_0(ref object o)
        {
            return QFramework.AudioKit.Settings;
        }


    }
}
