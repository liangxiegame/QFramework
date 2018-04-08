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

        public const int documentation = 12;

        public const int feedback_report_a_bug          = 20;
        public const int feedback_request_a_feature     = 21;
        public const int feedback_join_the_entitas_chat = 22;
        public const int feedback_entitas_wiki          = 23;
        public const int feedback_donate                = 24;
    }


    public static class EntitasMenuItems {
       
        public const string documentation                    = "Tools/Entitas/Documentation...";

        public const string feedback_report_a_bug            = "Tools/Entitas/Feedback/Report a bug...";
        public const string feedback_request_a_feature       = "Tools/Entitas/Feedback/Request a feature...";
        public const string feedback_join_the_entitas_chat   = "Tools/Entitas/Feedback/Join the Entitas chat...";
        public const string feedback_entitas_wiki            = "Tools/Entitas/Feedback/Entitas Wiki...";
        public const string feedback_donate                  = "Tools/Entitas/Feedback/Donate...";
    }

    public static class EntitasFeedback {

        [MenuItem(EntitasMenuItems.documentation, false, FrameworkMenuItemsPriorities.documentation)]
        public static void EntitasDocs() {
            Application.OpenURL("http://sschmid.github.io/Entitas-CSharp/");
        }

        [MenuItem(EntitasMenuItems.feedback_report_a_bug, false, FrameworkMenuItemsPriorities.feedback_report_a_bug)]
        public static void ReportBug() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem(EntitasMenuItems.feedback_request_a_feature, false, FrameworkMenuItemsPriorities.feedback_request_a_feature)]
        public static void RequestFeature() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem(EntitasMenuItems.feedback_join_the_entitas_chat, false, FrameworkMenuItemsPriorities.feedback_join_the_entitas_chat)]
        public static void EntitasChat() {
            Application.OpenURL("https://gitter.im/sschmid/Entitas-CSharp");
        }

        [MenuItem(EntitasMenuItems.feedback_entitas_wiki, false, FrameworkMenuItemsPriorities.feedback_entitas_wiki)]
        public static void EntitasWiki() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/wiki");
        }

        [MenuItem(EntitasMenuItems.feedback_donate, false, FrameworkMenuItemsPriorities.feedback_donate)]
        public static void Donate() {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
