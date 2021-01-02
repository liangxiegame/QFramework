using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Xml;
using UnityEditor;

namespace UI.Xml
{
    public static class XmlLayoutEditorUtilities
    {
        public static void XmlFileUpdated(string xmlFile)
        {
            var canvasObjects = GameObject.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvasObjects)
            {                                
                var xmlLayoutElements = canvas.GetComponentsInChildren<XmlLayout>(true);
                
                var elementsUsingThisFile = xmlLayoutElements.Where(xl => xl.AutomaticallyReloadXmlFileIfItChanges && xl.XmlFile != null)
                                                             .Where(xl => AssetDatabase.GetAssetPath(xl.XmlFile).Contains(xmlFile))
                                                             .ToList();

                foreach (var xmlLayoutElement in elementsUsingThisFile)
                {                    
                    RebuildXmlLayout(xmlLayoutElement, () => xmlLayoutElement.ReloadXmlFile(), xmlFile);
                    //xmlLayoutElement.ReloadXmlFile();
                }

                var elementsUsingThisDefaultsFile = xmlLayoutElements.Where(xl => xl.DefaultsFiles != null && xl.DefaultsFiles.Any(df => AssetDatabase.GetAssetPath(df) == xmlFile))
                                                                     .ToList();

                xmlFile = xmlFile.Replace(".xml", "");

                var elementsIncludingThisFile = xmlLayoutElements.Where(xl => xl.IncludedFiles != null && xl.IncludedFiles.Any(f => xmlFile.EndsWith(f)))
                                                                 .ToList();

                var elementsToRebuild = new List<XmlLayout>();
                elementsToRebuild.AddRange(elementsUsingThisDefaultsFile);
                elementsToRebuild.AddRange(elementsIncludingThisFile);
                elementsToRebuild = elementsToRebuild.Distinct().ToList();

                foreach (var xmlLayoutElement in elementsToRebuild)
                {
                    RebuildXmlLayout(xmlLayoutElement, () => xmlLayoutElement.RebuildLayout(true), xmlFile + ".xml");
                    //xmlLayoutElement.RebuildLayout(true);
                }
            }
        }

        private static void RebuildXmlLayout(XmlLayout xmlLayout, Action rebuildAction, string xmlFile)
        {            
            var isActive = xmlLayout.gameObject.activeSelf;

            //var isActiveInHierarchy = xmlLayout.gameObject.activeInHierarchy; 
                // I'm not sure it would be wise to activate potentially multiple parent gameobjects
                // If changes are made to an Xml file and they need to be applied to an XmlLayout which is embedded in an inactive parent object,
                // then the best option for now is to either: 
                //  a) Set XmlLayout.ForceRebuildOnAwake to true (which will force a rebuild when the XmlLayout is actived)
                // or b) Manually activate the XmlLayout, and click 'Force Rebuild Now' in the editor

            Debug.Log("[XmlLayout] Rebuilding XmlLayout '" + xmlLayout.name + "' as the Xml file '" + xmlFile.Substring(xmlFile.LastIndexOf('/') + 1) + "' has been changed.");
            //EditorGUIUtility.PingObject(xmlLayout.gameObject);

            // The layout code works best if the gameobject is active
            if (!isActive)
            {
                xmlLayout.gameObject.SetActive(true);

                EditorApplication.delayCall += () =>
                {                    
                    rebuildAction();                    

                    EditorApplication.delayCall += () =>
                    {                        
                        xmlLayout.gameObject.SetActive(false);
                    };
                };
            } else 
            {
                rebuildAction();
            }                        
        }
    }
}
