namespace QF.GraphDesigner
{
    public interface ICodePathStrategy
    {
        /// <summary>
        /// The root path to the diagram file
        /// </summary>
        string AssetPath { get; set; }

        /// <summary>
        /// Where behaviours are stored
        /// </summary>
        string BehavioursPath { get; }

        /// <summary>
        /// Where scenes are stored
        /// </summary>
        string ScenesPath { get; }

        IGraphData Data { get; set; }


        string GetDesignerFilePath(string postFix);
        string GetEditableFilePath(IGraphItem item,string name = null);

        //string GetEditableViewFilename(ViewData nameAsView);
        //string GetEditableViewComponentFilename(ViewComponentData name);
        //string GetEditableSceneManagerFilename(SceneManagerData nameAsSceneManager);
        //string GetEditableSceneManagerSettingsFilename(SceneManagerData nameAsSettings);
        //string GetEditableControllerFilename(ElementData controllerName);
        //string GetEditableViewModelFilename(ElementData nameAsViewModel);
        //string GetEnumsFilename(EnumData name);

        //void MoveTo(GeneratorSettings settings, ICodePathStrategy strategy, string name, ElementsDesigner designerWindow);
    }
}