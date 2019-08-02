using System.Collections.Generic;
using Invert.Common;
using UnityEditor;
using UnityEngine;

namespace Invert.Windows
{

    public class WindowSelectionPanel : Area<IEnumerable<IWindowFactory>>
    {
        private GUIStyle _buttonStyle;
        private Texture2D _buttonBackground;

        public override void Draw(IEnumerable<IWindowFactory> data)
        {
            foreach (var windowConfig in data)
            {
                GUILayout.Button(windowConfig.LauncherTitle, ButtonStyle);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Button("UFrame Designer", ButtonStyle);
            GUILayout.Button("Settings", ButtonStyle);
            GUILayout.Button("Documentation", ButtonStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Button("UFrame Designer", ButtonStyle);
            GUILayout.Button("Settings", ButtonStyle);
            GUILayout.Button("Documentation", ButtonStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Button("UFrame Designer", ButtonStyle);
            GUILayout.Button("Settings", ButtonStyle);
            GUILayout.Button("Documentation", ButtonStyle);
            GUILayout.EndHorizontal();

        }

        public GUIStyle ButtonStyle
        {
            get
            {
                return _buttonStyle ?? (_buttonStyle = new GUIStyle()
                {
                    fixedWidth = 200,
                    fixedHeight = 200,
                    fontSize = 18,
                    normal =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    focused =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    active =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    hover =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    onHover =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    onFocused =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    onNormal =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),
                        textColor = Color.white
                    },
                    onActive =
                    {
                        background = ElementDesignerStyles.GetSkinTexture("WindowButton"),  
                        textColor = Color.white
                    },
                    alignment = TextAnchor.MiddleCenter
                    
                });
                
            }
            set { _buttonStyle = value; }
        }

    }


}