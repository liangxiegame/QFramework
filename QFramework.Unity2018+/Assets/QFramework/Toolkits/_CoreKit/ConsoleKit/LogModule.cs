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

        Vector2 scrollPos;
        bool scrollToBottom = true;
        List<ConsoleMessage> entries = new List<ConsoleMessage>();
        bool collapse;

        public override void OnInit()
        {
            Application.logMessageReceived += HandleLog;
        }

        private FluentGUIStyle mLabelStyle = new FluentGUIStyle(() => new GUIStyle
        {
            fontSize = 10,
            normal =
            {
                textColor = Color.white
            }
        });

        public override void DrawGUI()
        {
            if (scrollToBottom)
            {
                GUILayout.BeginScrollView(Vector2.up * entries.Count * 100.0f);
            }
            else
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);
            }

            // Go through each logged entry
            for (int i = 0; i < entries.Count; i++)
            {
                ConsoleMessage entry =
                    entries
                        [i]; // If this message is the same as the last one and the collapse feature is chosen, skip it 
                if (collapse && i > 0 && entry.message == entries[i - 1].message)
                {
                    continue;
                }

                // Change the text colour according to the log type
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


                if (entry.type == LogType.Exception)
                {
                    GUILayout.Label(entry.message + " || " + entry.stackTrace, mLabelStyle);
                }
                else
                {
                    GUILayout.Label(entry.message, mLabelStyle);
                }
            }

            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            // Clear button
            if (GUILayout.Button(clearLabel))
            {
                entries.Clear();
            }

            // Collapse toggle
            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
            scrollToBottom =
                GUILayout.Toggle(scrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
            entries.Add(entry);
        }

        public override void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}