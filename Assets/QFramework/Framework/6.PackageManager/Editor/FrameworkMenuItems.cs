/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 ****************************************************************************/

using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class FrameworkMenuItems
    {
        public const string Preferences = "QFramework/Preferences... %e";

        public const string CheckForUpdates = "QFramework/Check for Updates...";

        public const string Feedback = "QFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int CheckForUpdates = 10;

        public const int Feedback = 11;
    }
}
