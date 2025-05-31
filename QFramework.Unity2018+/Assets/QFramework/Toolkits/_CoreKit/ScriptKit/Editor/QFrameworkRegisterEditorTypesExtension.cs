/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using QFramework;
using MoonSharp.Interpreter;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class QFrameworkRegisterEditorTypesExtension
    {
        public static Script RegisterQFrameworkEditorTypes(this Script self)
        {
            // IO
            UserData.RegisterType(typeof(File));
            self.Globals["File"] = typeof(File);
            
            UserData.RegisterType<Application>();
            self.Globals["Application"] = typeof(Application);

            UserData.RegisterType<EditorApplication>();
            self.Globals["EditorApplication"] = typeof(EditorApplication);

            UserData.RegisterType<GUILayout>();
            self.Globals["GUILayout"] = typeof(GUILayout);

            UserData.RegisterType<EditorGUILayout>();
            self.Globals["EditorGUILayout"] = typeof(EditorGUILayout);

            UserData.RegisterType<GUILayoutOption>();
            UserData.RegisterType<GUIUtility>();
            self.Globals["GUIUtility"] = typeof(GUIUtility);

            UserData.RegisterType<GUI>();
            self.Globals["GUI"] = typeof(GUI);

            UserData.RegisterType<GUIStyle>();
            self.Globals["GUIStyleBox"] = new GUIStyle("box");

            UserData.RegisterType<FluentGUIStyle>();
            self.Globals["FluentGUIStyle"] = typeof(FluentGUIStyle);

            UserData.RegisterType<AssetDatabase>();
            self.Globals["AssetDatabase"] = typeof(AssetDatabase);

            UserData.RegisterType<EditorHttpResponse>();
            UserData.RegisterType<WWWForm>();
            self.Globals["CreateForm"] = (Func<WWWForm>)CreateForm;
            self.Globals["Post"] = (Action<String,WWWForm,DynValue>)Post;
            
            UserData.RegisterType<JSONObject>();
            self.Globals["ParseToJSONObject"] = (Func<string, JSONObject>)ParseToJSONObject;

            UserData.RegisterType<Vector2>();
            self.Globals["Vector2Zero"] = Vector2.zero;

            UserData.RegisterType<BindableProperty<string>>();
            UserData.RegisterType(typeof(User));
            self.Globals["User"] = typeof(User);

            UserData.RegisterType<MoonSharpEditorWindow>();
            self.Globals["MoonSharpEditorWindow"] = typeof(MoonSharpEditorWindow);
            
            return self;
        }

        private static JSONObject ParseToJSONObject(string arg)
        {
            return new JSONObject(arg);
        }

        static WWWForm CreateForm()
        {
            return new WWWForm();
        }

        static void Post(string url,WWWForm form,DynValue onResponse)
        {
            EditorHttp.Post(url,form,response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    onResponse.Function.Call(response.Text);
                }
            });
        }
        
    }
}
#endif