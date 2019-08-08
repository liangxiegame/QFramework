using System;
using System.Linq;
using Invert.Common;
using Invert.Common.UI;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class MarketWindow : EditorWindow
    {
        public string _AssetPath;
        //public UBSharedBehaviour _Context;
        protected IGrouping<string, GraphTypeInfo>[] _triggerGroups;
        protected Action<GraphTypeInfo> _OnAdd;
        private MarketInfo jsonInfo;


        //[MenuItem("Window/Invert Market")]
        internal static void ShowWindow()
        {
            var window = GetWindow<MarketWindow>();
            window.title = "Invert Market";

            //window.jsonInfo = MarketPlace.CallJson("GetMarketInfo");
            window.Show();
        }

        public void OnGUI()
        {
            GUIHelpers.IsInsepctor = false;
            //if (string.IsNullOrEmpty(MarketPlace.Token) || jsonInfo == null || string.IsNullOrEmpty(MarketPlace.Cookies))
            //{
            DoLoginScreen();
            if (GUILayout.Button("Logout"))
            {
                MarketPlace.Token = null;
                MarketPlace.Cookies = null;
                loginResponse = null;
                jsonInfo = null;
            }
            //}
            //else
            //{
            //    if (jsonInfo == null)
            //    {
            //        jsonInfo = MarketPlace.GetMarketInfo();
            //    }
            //    if (jsonInfo == null)
            //    {
            //        return;
            //    }
            //    DoMarketScreen();
            //}


        }

        public string username = string.Empty;
        public string password = string.Empty;
        public Response loginResponse = null;

        private void DoLoginScreen()
        {
            if (loginResponse != null && !loginResponse.Success)
            {
                EditorGUILayout.HelpBox(loginResponse.Message, MessageType.Error);
            }
            username = EditorGUILayout.TextField("Username:", username);
            password = EditorGUILayout.TextField("Password:", password);
            if (GUILayout.Button("Login"))
            {
                loginResponse = MarketPlace.Login(username, password);
                jsonInfo = MarketPlace.GetMarketInfo();
            }
        }

        private void DoMarketScreen()
        {
            foreach (var item in jsonInfo.MarketItems)
            {

                if (
                    GUIHelpers.DoTriggerButton(new UFStyle(string.Format("{0} {1:C}", item.Name, item.Price),
                        ElementDesignerStyles.EventButtonStyleSmall)
                    {
                        FullWidth = false
                    }))
                {

                }
            }
            if (GUILayout.Button("Logout"))
            {
                MarketPlace.Token = null;
                MarketPlace.Cookies = null;
                loginResponse = null;
                jsonInfo = null;
            }
        }
    }
}