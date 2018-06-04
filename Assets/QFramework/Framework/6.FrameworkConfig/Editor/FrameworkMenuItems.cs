/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 ****************************************************************************/

using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class FrameworkMenuItems
    {
        public const string Preferences = "QFramework/Preferences... #%e";
        
        public const string CheckForUpdates = "QFramework/Check for Updates...";

        public const string Feedback = "QFramework/Feedback";
    }
    
    public static class FrameworkMenuItemsPriorities {

        public const int Preferences = 1;

        public const int CheckForUpdates = 10;

        public const int Feedback = 11;

        public const int feedback_donate                = 24;
    }

    public static class EntitasMenuItems {
        public const string feedback_donate                  = "Tools/Entitas/Feedback/Donate...";
    }

    public static class EntitasFeedback {
        [MenuItem(EntitasMenuItems.feedback_donate, false, FrameworkMenuItemsPriorities.feedback_donate)]
        public static void Donate() {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
