using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;

namespace QFramework
{
    public class ConsoleWindow : MonoBehaviour
    {
        /// <summary>
        /// Update回调
        /// </summary>
        internal delegate void OnUpdateCallback();

        /// <summary>
        /// OnGUI回调
        /// </summary>
        internal delegate void OnGUICallback();

        internal OnUpdateCallback onUpdateCallback = null;

        internal OnGUICallback onGUICallback = null;
        
        
        internal static ConsoleWindow Default = null;

        internal bool ShowGUI
        {
            get => showGUI;
            set => showGUI = value;
        }
        private bool showGUI = true;

#if UNITY_IOS
        bool                 mTouching = false;
#endif

        const int margin = 20;

        Rect windowRect = new Rect(margin + 960 * 0.5f, margin, 960 * 0.5f - (2 * margin),
            540 - (2 * margin));

        public bool OpenInAwake = false;
        
        
        void Awake()
        {
            ConsoleKit.Modules.ForEach(m => m.OnInit());
            this.showGUI = OpenInAwake;
            DontDestroyOnLoad(this);

            mIndex = ConsoleKit.GetDefaultIndex();
            Default = this;
        }

        void OnDestroy()
        {
            ConsoleKit.Modules.ForEach(m => m.OnDestroy());
            // ConsoleKit.RemoveAllModules();
            Default = null;
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            if (Input.GetKeyUp(KeyCode.F1))
                this.showGUI = !this.showGUI;
#elif UNITY_ANDROID
            if (Input.GetKeyUp (KeyCode.Escape))
                this.showGUI = !this.showGUI;
#elif UNITY_IOS
            if (!mTouching && Input.touchCount == 4) {
                mTouching = true;
                this.showGUI = !this.showGUI;
            } else if (Input.touchCount == 0) {
                mTouching = false;
            }
#endif

            if (this.onUpdateCallback != null)
                this.onUpdateCallback();
        }

        private int mIndex = 0;
        

        void OnGUI()
        {
            if (!showGUI)
                return;

            if (onGUICallback != null)
                onGUICallback();

            var cachedMatrix = GUI.matrix;
            IMGUIHelper.SetDesignResolution(960, 540);
            windowRect = GUILayout.Window(int.MaxValue / 2, windowRect, DrawConsoleWindow, "控制台");
            GUI.matrix = cachedMatrix;
        }


        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        void DrawConsoleWindow(int windowID)
        {
            mIndex = GUILayout.Toolbar(mIndex, ConsoleKit.Modules.Select(m => m.Title).ToArray());
            ConsoleKit.Modules[mIndex].DrawGUI();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        internal void ShowModule(int index)
        {
            mIndex = index;
        }
    }
}