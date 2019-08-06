using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.GraphDesigner;
using Invert.uFrame.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class uFrameMenu : AssetPostprocessor
{

    //[MenuItem("Assets/[u]Frame/New Project", false, 40)]
    //public static void NewUFrameProject()
    //{
    //    var project = CreateAsset<ProjectRepository>();
    //    AssetDatabase.SaveAssets();
    //    Refresh();
    //}

    //public static void NewUFrameProject(string name)
    //{
    //    //var project = CreateAsset<ProjectRepository>("Assets/" + path);
    //    AssetDatabase.CreateFolder("Assets", name);
    //    var project = ScriptableObject.CreateInstance<ProjectRepository>();
    //    AssetDatabase.CreateAsset(project, "Assets/" + name + "/" + name + ".asset");
    //    Selection.activeObject = project;
    //    AssetDatabase.SaveAssets();
    //    Refresh();

    //}

    ///// <summary>
    ////	This makes it easy to create, name and place unique new ScriptableObject asset files.
    ///// </summary>
    //public static T CreateAsset<T>(string assetPath = null, string assetName = null) where T : ScriptableObject
    //{
    //    return CreateAsset(typeof(T),assetPath,assetName) as T;

        
    //}

    //public static ScriptableObject CreateAsset(Type type, string assetPath = null, string assetName = null)
    //{
    //    var asset = ScriptableObject.CreateInstance(type) as ScriptableObject;

    //    string path = assetPath ?? AssetDatabase.GetAssetPath(Selection.activeObject);
    //    if (path == "")
    //    {
    //        path = "Assets";
    //    }
    //    else if (Path.GetExtension(path) != "")
    //    {
    //        path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    //    }

    //    string assetPathAndName = assetName == null ? AssetDatabase.GenerateUniqueAssetPath(path + "/" + type.ToString() + ".asset") : AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetName + ".asset");

    //    AssetDatabase.CreateAsset(asset, assetPathAndName);
    //    AssetDatabase.SaveAssets();
    //    EditorUtility.FocusProjectWindow();
    //    Selection.activeObject = asset;
    //    return asset;
    //}
    //public static void OnPostprocessAllAssets(
    //    String[] importedAssets,
    //    String[] deletedAssets,
    //    String[] movedAssets,
    //    String[] movedFromAssetPaths)
    //{
    //    if (deletedAssets.Length > 0)
    //        Refresh();
    //}



    //private static void Refresh()
    //{
    //    //InvertGraphEditor.Projects = null;
    //    //var projectService = InvertGraphEditor.Container.Resolve<WorkspaceService>();
    //    //projectService.RefreshProjects();
    //}
}