using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace QFramework.Unity.Editor
{
    public class ScriptingDefineSymbols
    {
        public Dictionary<BuildTargetGroup, string> BuildTargetToDefSymbol
        {
            get { return mBuildTargetToDefSymbol; }
        }

        readonly Dictionary<BuildTargetGroup, string> mBuildTargetToDefSymbol;

        public ScriptingDefineSymbols()
        {
            mBuildTargetToDefSymbol = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(buildTargetGroup => buildTargetGroup != BuildTargetGroup.Unknown)
                .Where(buildTargetGroup => !isBuildTargetObsolete(buildTargetGroup))
                .Distinct()
                .ToDictionary(
                    buildTargetGroup => buildTargetGroup,
                    buildTargetGroup => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                );
        }

        public void AddDefineSymbol(string defineSymbol)
        {
            foreach (var kv in mBuildTargetToDefSymbol)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    kv.Key, kv.Value.Replace(defineSymbol, string.Empty) + "," + defineSymbol
                );
            }
        }

        public void RemoveDefineSymbol(string defineSymbol)
        {
            foreach (var kv in mBuildTargetToDefSymbol)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    kv.Key, kv.Value.Replace(defineSymbol, string.Empty)
                );
            }
        }

        bool isBuildTargetObsolete(BuildTargetGroup buildTargetGroup)
        {
            var fieldInfo = buildTargetGroup.GetType().GetField(buildTargetGroup.ToString());
            return Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));
        }
    }
}