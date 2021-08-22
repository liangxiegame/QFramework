using UnityEngine;

namespace BDFramework.Editor.Tools
{
   static public class EditorGUIHelper
    {
        
       readonly static public  GUIStyle TitleStyle =new GUIStyle()
        {
            fontSize = 25,
            normal = new GUIStyleState()
            {
                textColor = Color.red
            }
        };
       
       
       readonly static public  GUIStyle OnGUITitleStyle =new GUIStyle()
       {
           fontSize = 25,
           normal = new GUIStyleState()
           {
               textColor = Color.red
           }
       };
        
    }
}