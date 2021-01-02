using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.Reflection;

namespace UI.Xml
{    
    public class XmlPluginProcessor
    {
        static Dictionary<string, string> plugins = new Dictionary<string, string>()
        {
            {"UI.Pagination.PagedRect", "PAGEDRECT_PRESENT"},
            {"UI.Dates.DatePicker", "DATEPICKER_PRESENT"}
        };

        static List<Assembly> assemblies = new List<Assembly>();


        [DidReloadScripts(1)]
        public static void ProcessInstalledPlugins()
        {
            ManageSymbolDefinitions();
        }

        static void LoadAssemblies()
        {            
            assemblies = XmlLayoutUtilities.GetAssemblyNames()
                                           .Select(an => Assembly.Load(an))
                                           .ToList();
        }

        static void ManageSymbolDefinitions()
        {
            LoadAssemblies();

            List<string> platformsUpdated = new List<string>();

            foreach (BuildTargetGroup platform in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (IsObsolete(platform) || platform == BuildTargetGroup.Unknown) continue;

                var existingSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
                var symbols = existingSymbolsString.Replace(" ", "").Split(';').ToList();

                foreach (var plugin in plugins)
                {
                    symbols = ManageSymbolDefinition(symbols, plugin.Key, plugin.Value);
                }                                                

                var newSymbolsString = String.Join(";", symbols.Distinct().ToArray());

                if(existingSymbolsString != newSymbolsString)
                {
                    platformsUpdated.Add(platform.ToString());                    
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newSymbolsString);                    
                }
            }

            if (platformsUpdated.Any())
            {
                Debug.LogFormat("[XmlLayout] Updating symbols for platforms {0}...", String.Join(", ", platformsUpdated.ToArray()));
            }
        }

        static List<string> ManageSymbolDefinition(List<string> symbols, string className, string symbol)
        {
            var type = assemblies.Select(a => a.GetTypes().Where(t => t.FullName.Equals(className)).FirstOrDefault())
                                 .FirstOrDefault(t => t != null);
            
            if (type != null)
            {
                if (!symbols.Contains(symbol)) symbols.Add(symbol);
            }
            else
            {
                if (symbols.Contains(symbol)) symbols.Remove(symbol);
            }

            return symbols;
        }

        // http://stackoverflow.com/questions/29832536/check-if-enum-is-obsolete
        static bool IsObsolete(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (ObsoleteAttribute[])
                fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return (attributes != null && attributes.Length > 0);
        }
    }
}