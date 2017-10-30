using System.Linq;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class EntitasPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override int Priority
        {
            get { return 0; }
        }

        public override string Title
        {
            get { return "Entitas"; }
        }

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum ARCMode
        {
            Safe,
            FastAndUnsafe
        }

        ScriptingDefineSymbols mScriptingDefineSymbols;
        ARCMode mScriptCallOptimization;

        public override void Initialize(Properties properties)
        {
            mScriptingDefineSymbols = new ScriptingDefineSymbols();
            mScriptCallOptimization = mScriptingDefineSymbols.BuildTargetToDefSymbol.Values
                .All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                ? ARCMode.FastAndUnsafe
                : ARCMode.Safe;
        }

        protected override void DrawContent(Properties properties)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Automatic Entity Reference Counting");
                var buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                if (mScriptCallOptimization == ARCMode.Safe)
                {
                    buttonStyle.normal = buttonStyle.active;
                }
                if (GUILayout.Button("Safe", buttonStyle))
                {
                    mScriptCallOptimization = ARCMode.Safe;
                    mScriptingDefineSymbols.RemoveDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }

                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight);
                if (mScriptCallOptimization == ARCMode.FastAndUnsafe)
                {
                    buttonStyle.normal = buttonStyle.active;
                }
                if (GUILayout.Button("Fast And Unsafe", buttonStyle))
                {
                    mScriptCallOptimization = ARCMode.FastAndUnsafe;
                    mScriptingDefineSymbols.AddDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}