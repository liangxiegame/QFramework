using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using UnityEditor;
using UnityEngine;

public class DocumentationWindow : EditorWindow
{
    private List<IDrawer> _generatorDrawers;
    private CodeFileGenerator[] fileGenerators;
    private Vector2 _scrollPosition;
    private DiagramDrawer drawer;

    private List<GraphNode> _screenshots;
    private int _currentScreenshotIndex = 0;
    private bool _capturing = false;
    private bool _exitOnComplete = false;

    internal static void ShowWindow()
    {
        var window = GetWindow<DocumentationWindow>();
        window.title = "Documentation Window";
        window._currentScreenshotIndex = 0;

        // window.minSize = new Vector2(400, 500);
        //window.drawer = window.DiagramDrawer();
        window.ShowPopup();
    }
    internal static void ShowWindowAndGenerate()
    {
        var window = GetWindow<DocumentationWindow>();
        window.title = "Documentation Window";
        window._currentScreenshotIndex = 0;

        // window.minSize = new Vector2(400, 500);
        //window.drawer = window.DiagramDrawer();
        window.ShowPopup();
        var repository = InvertGraphEditor.DesignerWindow.DiagramViewModel.CurrentRepository;
        window._screenshots = repository.AllOf<GraphNode>().ToList();
        window._capturing = true;
        window._exitOnComplete = true;
        window.NextScreenshot();
    }
    public void OnGUI()
    {
        if (!_capturing)
        {
            if (GUILayout.Button("Generate Documentation"))
            {
                var repository = InvertGraphEditor.DesignerWindow.DiagramViewModel.CurrentRepository;
                _screenshots = repository.AllOf<GraphNode>().ToList();
                _capturing = true;
                NextScreenshot();
            }
            return;
        }

        if (drawer == null) return;
        foreach (var item in drawer.Children)
        {
            //if (!(item is DiagramNodeDrawer)) continue;
            //if (item is ScreenshotNodeDrawer) continue;
            item.Draw(InvertGraphEditor.PlatformDrawer, 1f);
        }


        if (Event.current.type == EventType.Repaint)
        {
            Texture2D texture2D = new Texture2D(Mathf.RoundToInt(this.position.width), Mathf.RoundToInt(this.position.height), (TextureFormat)3, false);

            texture2D.ReadPixels(new Rect(0f, 0f, this.position.width, this.position.height), 0, 0);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            Object.DestroyImmediate((Object)texture2D, true);
            var directory = Path.Combine("Documentation", "Screenshots");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(Path2.Combine("Documentation", "Screenshots", _screenshots[_currentScreenshotIndex - 1].Name + ".png"), bytes);
            Debug.Log("Saved image " + _screenshots[_currentScreenshotIndex - 1].Name + ".png");
            NextScreenshot();
        }

        //this.Close();
    }

    private void NextScreenshot()
    {
        if (_currentScreenshotIndex >= _screenshots.Count)
        {
            _capturing = false;
            _currentScreenshotIndex = 0;
            if (_exitOnComplete)
            {
                _exitOnComplete = false;
                this.Close();
                return;
            }
            Repaint();
            return;
        }
        drawer = DiagramDrawer(_screenshots[_currentScreenshotIndex]);

        _currentScreenshotIndex++;
        if (drawer == null)
        {
            NextScreenshot();
            return;
        }
        Repaint();
    }

    private DiagramDrawer DiagramDrawer(GraphNode node)
    {
        var window = InvertGraphEditor.DesignerWindow as ElementsDesigner;

        var diagramViewModel = new DiagramViewModel(node.Graph);
        diagramViewModel.NavigateTo(node.Identifier);


    
        var drawer = new DiagramDrawer(diagramViewModel);
        drawer.Refresh(InvertGraphEditor.PlatformDrawer);

        var screenshotVM = diagramViewModel.AllViewModels.OfType<DiagramNodeViewModel>().FirstOrDefault(p => p.GraphItemObject.Identifier == node.Identifier);


        if (screenshotVM == null)
            return null;
        this.position = new Rect(this.position.x, this.position.y, screenshotVM.Bounds.width + 20f, screenshotVM.Bounds.height + 20f);
        var position = screenshotVM.Position - new Vector2(10f,10f);
        Debug.Log(diagramViewModel.GraphData.CurrentFilter.Name + " " + position.x + ": " + position.y);
        foreach (var item in drawer.Children.OrderBy(p => p.ZOrder))
        {
           
            
            //item.Refresh(InvertGraphEditor.PlatformDrawer, new Vector2(item.Bounds.x - screenshotVM.Bounds.x, item.Bounds.y - screenshotVM.Bounds.y));
            if (item == null) continue;
            if (item.ViewModelObject != null)
            {
                item.IsSelected = false;
                item.ViewModelObject.ShowHelp = true;
            }
            

            item.Bounds = new Rect(item.Bounds.x - position.x, item.Bounds.y - position.y,
                item.Bounds.width, item.Bounds.height);

            foreach (var child in item.Children)
            {
                if (child == null) continue;
                child.Bounds = new Rect(child.Bounds.x - position.x, child.Bounds.y - position.y,
                    child.Bounds.width, child.Bounds.height);
                if (child.ViewModelObject != null)
                {
                    var cb = child.ViewModelObject.ConnectorBounds;

                    child.ViewModelObject.ConnectorBounds = new Rect(cb.x - position.x, cb.y - position.y,
                        cb.width, cb.height);
                }

                //child.Refresh(InvertGraphEditor.PlatformDrawer, new Vector2(item.Bounds.x - screenshotVM.Bounds.x, item.Bounds.y - screenshotVM.Bounds.y));
            }
        }
        return drawer;
    }
}