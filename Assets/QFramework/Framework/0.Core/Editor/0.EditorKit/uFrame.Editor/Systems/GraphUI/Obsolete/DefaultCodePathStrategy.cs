using System.IO;

namespace QFramework.GraphDesigner
{
    public class DefaultCodePathStrategy : ICodePathStrategy
    {
        public IGraphData Data { get; set; }

        public string AssetPath { get; set; }

        public virtual string BehavioursPath
        {
            get { return Path.Combine(AssetPath, "Behaviours"); }
        }

        public virtual string ScenesPath
        {
            get { return Path.Combine(AssetPath, "Scenes"); }
        }

        public string GetDesignerFilePath(string postFix)
        {
            return Path.Combine("_DesignerFiles", Data.Name + postFix + ".designer.cs");
        }

        public string GetEditableFilePath(IGraphItem item, string name = null)
        {
            return (name ?? Data.Name) + ".cs";
        }

        //public virtual string GetEditableViewFilename(ViewData nameAsView)
        //{
        //    return Path.Combine("Views", nameAsView.NameAsView + ".cs");
        //}

        //public virtual string GetEditableViewComponentFilename(ViewComponentData name)
        //{
        //    return Path.Combine("ViewComponents", name.Name + ".cs");
        //}

        //public virtual string GetEditableSceneManagerFilename(SceneManagerData nameAsSceneManager)
        //{
        //    return Path.Combine("SceneManagers", nameAsSceneManager.NameAsSceneManager + ".cs");
        //}

        //public virtual string GetEditableSceneManagerSettingsFilename(SceneManagerData nameAsSettings)
        //{
        //    return Path.Combine("SceneManagers", nameAsSettings.NameAsSettings + ".cs");
        //}

        //public virtual string GetEditableControllerFilename(ElementData controllerName)
        //{
        //    return Path.Combine("Controllers", controllerName.NameAsController + ".cs");
        //}

        //public virtual string GetEditableViewModelFilename(ElementData nameAsViewModel)
        //{
        //    return Path.Combine("ViewModels", nameAsViewModel.NameAsViewModel + ".cs");
        //}

        //public virtual string GetEnumsFilename(EnumData name)
        //{
        //    return GetDesignerFilePath(string.Empty);
        //}

        //public virtual void MoveTo(GeneratorSettings settings, ICodePathStrategy strategy, string name, ElementsDesigner designerWindow)
        //{
        //    var sourceFiles = uFrameEditor.GetAllFileGenerators(settings).ToArray();
        //    strategy.Data = Data;
        //    strategy.AssetPath = AssetPath;
        //    var targetFiles = uFrameEditor.GetAllFileGenerators(settings).ToArray();

        //    if (sourceFiles.Length == targetFiles.Length)
        //    {
        //        // Attempt to move every file
        //        ProcessMove(strategy, name, sourceFiles, targetFiles);
        //    }
        //    else
        //    {
        //        // Attempt to move non designer files
        //        // var designerFiles = sourceFiles.Where(p => p.Filename.EndsWith("designer.cs"));
        //        sourceFiles = sourceFiles.Where(p => !p.SystemPath.EndsWith("designer.cs")).ToArray();
        //        targetFiles = targetFiles.Where(p => !p.SystemPath.EndsWith("designer.cs")).ToArray();
        //        if (sourceFiles.Length == targetFiles.Length)
        //        {
        //            ProcessMove(strategy, name, sourceFiles, targetFiles);
        //            //// Remove all designer files
        //            //foreach (var designerFile in designerFiles)
        //            //{
        //            //    File.Delete(System.IO.Path.Combine(AssetPath, designerFile.Filename));
        //            //}
        //            //var saveCommand = uFrameEditor.Container.Resolve<IToolbarCommand>("SaveCommand");
        //            //saveCommand.Execute();
        //        }
        //    }

        //}


        //protected virtual void ProcessMove(ICodePathStrategy strategy, string name, CodeFileGenerator[] sourceFiles,
        //    CodeFileGenerator[] targetFiles)
        //{
        //    for (int index = 0; index < sourceFiles.Length; index++)
        //    {
        //        var sourceFile = sourceFiles[index];
        //        var targetFile = targetFiles[index];

        //        var sourceFileInfo = new FileInfo(System.IO.Path.Combine(AssetPath, sourceFile.SystemPath));
        //        var targetFileInfo = new FileInfo(System.IO.Path.Combine(AssetPath, targetFile.SystemPath));
        //        if (sourceFileInfo.FullName == targetFileInfo.FullName) continue;
        //        if (!sourceFileInfo.Exists) continue;
        //        EnsurePath(sourceFileInfo);
        //        if (targetFileInfo.Exists) continue;
        //        EnsurePath(targetFileInfo);

        //        var sourceAsset = "Assets" + sourceFileInfo.FullName.Replace("\\", "/").Replace(Application.dataPath, "").Replace("\\", "/");
        //        var targetAsset = "Assets" + targetFileInfo.FullName.Replace("\\", "/").Replace(Application.dataPath, "").Replace("\\", "/");
        //        uFrameEditor.Log(string.Format("Moving file {0} to {1}", sourceAsset, targetAsset));
        //        AssetDatabase.MoveAsset(sourceAsset, targetAsset);
        //    }

        //    Data.Settings.CodePathStrategyName = name;
        //    Data.CodePathStrategy = null;
        //    EditorUtility.SetDirty(Data as UnityEngine.Object);
        //    AssetDatabase.SaveAssets();
        //    EditorApplication.SaveAssets();
        //    AssetDatabase.Refresh();
        //    ////Clean up old directories
        //    //foreach (var sourceFile in sourceFiles)
        //    //{
        //    //    var sourceFileInfo = new FileInfo(System.IO.Path.Combine(AssetPath, sourceFile.Filename));
        //    //    if (sourceFileInfo.Directory != null)
        //    //    {
        //    //        if (!sourceFileInfo.Directory.Exists) continue;

        //    //        var directories = sourceFileInfo.Directory.GetDirectories("*", SearchOption.AllDirectories);
        //    //        foreach (var directory in directories)
        //    //        {
        //    //            if (directory.GetFiles("*").Count(x => x.Extension != ".meta" && x.Extension != "meta") == 0)
        //    //            {
        //    //                directory.Delete(true);
        //    //                Debug.Log("Removed Directory " + directory.FullName);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //AssetDatabase.Refresh();
        //}

        //protected void EnsurePath(FileInfo fileInfo)
        //{

        //    // Get the path to the directory
        //    var directory = System.IO.Path.GetDirectoryName(fileInfo.FullName);
        //    // Create it if it doesn't exist
        //    if (directory != null && !Directory.Exists(directory))
        //    {

        //        Directory.CreateDirectory(directory);
        //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        //    }
        //}
    }
}