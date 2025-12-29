using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class LogModule : ConsoleModule
    {
        struct ConsoleMessage
        {
            public readonly string message;
            public readonly string stackTrace;
            public readonly LogType type;

            public ConsoleMessage(string message, string stackTrace, LogType type)
            {
                this.message = message;
                this.stackTrace = stackTrace;
                this.type = type;
            }
        }

        public override string Title { get; set; } = "Log";
        GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        GUIContent scrollToBottomLabel = new GUIContent("ScrollToBottom", "Scroll bar always at bottom");

        Vector2 mScrollPos;
        private bool mScrollToBottom = true;
        private readonly List<ConsoleMessage> mEntries = new List<ConsoleMessage>();
        private bool mCollapse;

        public override void OnInit()
        {
            Application.logMessageReceived += HandleLog;
        }

        private readonly FluentGUIStyle mLabelStyle = FluentGUIStyle.Label()
            .NormalTextColor(Color.white)
            .FontSize(10);

        private readonly FluentGUIStyle mButtonStyle = FluentGUIStyle.Button()
            .FontSize(10);

        public override void DrawGUI()
        {
            if (mScrollToBottom)
            {
                GUILayout.BeginScrollView(Vector2.up * mEntries.Count * 100.0f);
            }
            else
            {
                mScrollPos = GUILayout.BeginScrollView(mScrollPos);
            }

            for (var i = 0; i < mEntries.Count; i++)
            {
                var entry =
                    mEntries
                        [i]; // If this message is the same as the last one and the collapse feature is chosen, skip it 
                if (mCollapse && i > 0 && entry.message == mEntries[i - 1].message)
                {
                    continue;
                }

                switch (entry.type)
                {
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.contentColor = Color.red;
                        break;
                    case LogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    default:
                        GUI.contentColor = Color.white;
                        break;
                }

                GUILayout.BeginHorizontal("box",GUILayout.ExpandWidth(true));

                if (entry.type == LogType.Exception)
                {
                    GUILayout.Label(entry.message + " || " + entry.stackTrace, mLabelStyle);
                }
                else
                {
                    GUILayout.Label(entry.message, mLabelStyle);
                }
                
                
                if (GUILayout.Button("Copy", mButtonStyle,GUILayout.Width(40)))
                {
                    GUIUtility.systemCopyBuffer = entry.message + "\n" + entry.stackTrace;
                }

                GUILayout.EndHorizontal();
            }

            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            // Clear button
            if (GUILayout.Button(clearLabel))
            {
                mEntries.Clear();
            }

            // Collapse toggle
            mCollapse = GUILayout.Toggle(mCollapse, collapseLabel, GUILayout.ExpandWidth(false));
            mScrollToBottom =
                GUILayout.Toggle(mScrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
            mEntries.Add(entry);
        }

        public override void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}