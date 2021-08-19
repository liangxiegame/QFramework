using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    partial class CLRBindings
    {

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector4> s_UnityEngine_Vector4_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        static partial void OnInitialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_GameObject_Binding.Register(app);
            QFramework_ILComponentBehaviour_Binding.Register(app);
            System_Activator_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            QFramework_UIKit_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            UnityEngine_SceneManagement_Scene_Binding.Register(app);
            System_String_Binding.Register(app);
            QFramework_ResKit_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            UnityEngine_SpriteRenderer_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UniRx_Observable_Binding.Register(app);
            UniRx_ObservableExtensions_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            QFramework_UIRoot_Binding.Register(app);
            DG_Tweening_ShortcutExtensions_Binding.Register(app);
            UnityEngine_Random_Binding.Register(app);
            QFramework_ResLoader_Binding.Register(app);
            UniRx_DisposableExtensions_Binding.Register(app);
            QFramework_Log_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            UnityEngine_ColorUtility_Binding.Register(app);
            UnityEngine_Transform_Array2_Binding.Register(app);
            UnityEngine_PlayerPrefs_Binding.Register(app);
            System_Int32_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            QFramework_GameObjectExtension_Binding.Register(app);
            DG_Tweening_DOTweenModuleUI_Binding.Register(app);
            DG_Tweening_TweenSettingsExtensions_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            QFramework_AudioKit_Binding.Register(app);
            QFramework_AudioKitSettings_Binding.Register(app);
            QFramework_Property_1_Boolean_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Reflection_MethodBase_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            QFramework_DictionaryPool_2_Type_ILTypeInstance_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Array_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector4));
            s_UnityEngine_Vector4_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector4>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        static partial void OnShutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector2_Binding_Binder = null;
            s_UnityEngine_Vector3_Binding_Binder = null;
            s_UnityEngine_Vector4_Binding_Binder = null;
            s_UnityEngine_Quaternion_Binding_Binder = null;
        }
    }
}
