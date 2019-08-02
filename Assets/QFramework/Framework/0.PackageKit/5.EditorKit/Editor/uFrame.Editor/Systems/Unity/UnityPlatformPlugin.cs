using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using QF;
using Invert.Windows;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QF.GraphDesigner.Unity
{
    [InitializeOnLoad]
    public class UnityPlatformPlugin : DiagramPlugin, IAssetDeleted, IWorkspaceChanged, IChangeDatabase
    {
        public override decimal LoadPriority
        {
            get { return -95; }
        }

        public override bool Required
        {
            get { return true; }
        }

        static UnityPlatformPlugin()
        {
            InvertApplication.CachedAssembly(typeof (UnityPlatformPlugin).Assembly);
            InvertApplication.CachedAssembly(typeof(Vector3).Assembly);
            InvertApplication.CachedTypeAssembly(typeof(Vector3).Assembly);
            InvertGraphEditor.Prefs = new UnityPlatformPreferences();
            InvertApplication.Logger = new UnityPlatform();
            InvertGraphEditor.Platform = new UnityPlatform();
            InvertGraphEditor.PlatformDrawer = new UnityDrawer();
        }
        public override bool Enabled { get { return true; } set { } }
        public override void Initialize(QFrameworkContainer container)
        {
           

            EditorUtility.ClearProgressBar();
            // TODO 2.0: Obviously fix undo
            //Undo.undoRedoPerformed = delegate
            //{
            //    var ps = container.Resolve<WorkspaceService>();
           
            //    ps.RefreshProjects();
            //    InvertGraphEditor.DesignerWindow.RefreshContent();
            //};
            container.RegisterInstance<IPlatformDrawer>(InvertGraphEditor.PlatformDrawer);
            container.RegisterInstance<IStyleProvider>(new UnityStyleProvider());
#if DOCS
            container.RegisterToolbarCommand<GenerateDocsCommand>();
            container.RegisterToolbarCommand<DocsModeCommand>();
#endif
           // container.RegisterInstance<IToolbarCommand>(new Test(), "Test");


            //container.RegisterInstance<IAssetManager>(new UnityAssetManager());

            // Command Drawers
            container.RegisterInstance<ToolbarUI>(new UnityToolbar()
            {
                
            });
            container.Register<ContextMenuUI, UnityContextMenu>();

            container.RegisterInstance<IGraphEditorSettings>(new UFrameSettings());
            // Where the generated code files are placed
            container.Register<ICodePathStrategy, DefaultCodePathStrategy>("Default");
            container.RegisterInstance<IWindowManager>(new UnityWindowManager());

        }

        public override void Loaded(QFrameworkContainer container)
        {

        }




        public void AssetDeleted(string filename)
        {
            // TODO 2.0 This is no longer valid
        }

        public void WorkspaceChanged(Workspace workspace)
        {
            if (InvertGraphEditor.DesignerWindow != null)
            {
                InvertGraphEditor.DesignerWindow.ProjectChanged(workspace);
            }
        }

        public void ChangeDatabase(IGraphConfiguration configuration)
        {
            
        }
    }


    

}
