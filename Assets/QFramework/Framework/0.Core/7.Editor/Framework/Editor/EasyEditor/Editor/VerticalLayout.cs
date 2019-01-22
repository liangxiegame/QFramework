using RSG.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// https://github.com/Real-Serious-Games
namespace UnityEditorUI
{
    /// <summary>
    /// Layout that arranges widgets in a row vertically.
    /// </summary>
    internal class VerticalLayout : AbstractLayout
    {
        public VerticalLayout(ILayout parent) : 
            base(parent) 
        {
        }

        public override void OnGUI()
        {
            if (!enabled)
            {
                return;
            }

            GUILayout.BeginVertical();
            base.OnGUI();
            GUILayout.EndVertical();
        }
    }
}
