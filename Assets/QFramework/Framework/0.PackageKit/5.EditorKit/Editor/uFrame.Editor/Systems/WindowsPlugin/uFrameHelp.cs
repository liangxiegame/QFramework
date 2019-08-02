using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class UFrameHelp : EditorWindow, IDocumentationBuilder, INodeItemEvents
{
    public static Dictionary<string, Texture> ImageCache 
    {
        get { return mImageCache ?? (mImageCache = new Dictionary<string, Texture>()); }
        set { mImageCache = value; }
    }

    public static Dictionary<string, string> ContentCache
    {
        get { return mContentCache ?? (mContentCache = new Dictionary<string, string>()); }
        set { mContentCache = value; }
    }
    public Texture GetImage(string url)
    {
        Texture texture;
        if (ImageCache.TryGetValue(url, out texture))
        {
            return texture;
        }
        ImageCache.Add(url, null);
        DownloadImage(url);
        if (ImageCache.TryGetValue(url, out texture))
        {
            return texture;
        }
        else
        {
            return ElementDesignerStyles.GetSkinTexture("LoadingImage");
        }

        return ElementDesignerStyles.ArrowDownTexture;
    }
    public string GetContent(string url)
    {
        string texture;
        if (ContentCache.TryGetValue(url, out texture))
        {
            return texture;
        }
        ContentCache.Add(url, null);
        DownloadString(url);
        if (ContentCache.TryGetValue(url, out texture))
        {
            return texture;
        }
        return "Loading...";
    }
    public void DownloadImage(string url)
    {
        var ww = new WWW(url);

        EditorApplication.CallbackFunction callbackFunction= null;
        callbackFunction= () =>
        {
            if (ww.isDone)
            {
                EditorApplication.update -= callbackFunction;
                ImageCache[url] = ww.texture;
                Repaint();
            }
        };
        EditorApplication.update += callbackFunction;
        
    }
    public void DownloadString(string url)
    {
        var ww = new WWW(url);
        while (!ww.isDone)
        {

        }
        ContentCache[url] = ww.text;
        Repaint();
    }
    [FormerlySerializedAs("LastPage")]
    [SerializeField]
    public string lastPage;
    private static IDocumentationProvider[] mDocumentationProvider;
    private static List<DocumentationPage> mPages;
    private Stack<DocumentationPage> mPageStack;
    [FormerlySerializedAs("_tocScrollPosition")]
    [SerializeField]
    private Vector2 tocScrollPosition;
    [FormerlySerializedAs("_pageScrollPosition")]
    [SerializeField]
    private Vector2 pageScrollPosition;
    private static Dictionary<string, Texture> mImageCache;
    private static GUIStyle mTitleStyle;
    private static GUIStyle mParagraphStyle;
    private static Dictionary<string, string> mContentCache;
    private static GUIStyle mTutorialActionStyle;


    public static void ShowPage(Type pageType)
    {
        ShowWindow(FindPage(Pages, _ => _.GetType() == pageType));

    }
    public static void ShowPage(string name)
    {
        ShowWindow(FindPage(Pages, _ => _.Name == name));
    }

    public static UFrameHelp Instance;

    [MenuItem("Window/uFrame/Documentation")]
    public static void ShowWindow()
    {
        var window = GetWindow<UFrameHelp>();
        //window.minSize = new Vector2(800,500);
        window.title = "uFrame Help";
        Instance = window;
    //   window.minSize = new Vector2(400, 500);
        window.ShowUtility();
    }

    public static void ShowWindow(Type graphItemType)
    {
        if (graphItemType != null)
        {

            var page = FindPage(Pages, p => p.RelatedNodeType == graphItemType);
            ShowWindow(page);
        }
        else
        {
            ShowWindow();
        }
    }
    public static void ShowWindow(DocumentationPage page)
    {

        var window = GetWindow<UFrameHelp>();
        window.title = "uFrame Help";
        window.minSize = new Vector2(1100, 806);

        if (page != null)
            window.ShowPage(page);
        window.ShowUtility();
    }
    private void ShowPage(DocumentationPage page)
    {
        lastPage = page.Name;
        PageStack.Push(page);
    }

    public static IDocumentationProvider[] DocumentationProvider
    {
        get
        {
            if (mDocumentationProvider != null) return mDocumentationProvider;


            mDocumentationProvider =
                InvertApplication.Container.ResolveAll<IDocumentationProvider>().ToArray();

            return mDocumentationProvider;
        }
        set { mDocumentationProvider = value; }
    }

    public static List<DocumentationPage> Pages
    {
        get
        {
            if (mPages == null)
            {
                mPages = new List<DocumentationPage>();
                foreach (var provider in DocumentationProvider)
                {
                    provider.GetPages(mPages);
                }
            }
            return mPages;
        }
        set { mPages = value; }
    }

    public static DocumentationPage FindPage(IEnumerable<DocumentationPage> inside, Predicate<DocumentationPage> predicate)
    {
        foreach (var page in inside)
        {
            if (predicate(page))
            {
                return page;
            }
            var result = FindPage(page.ChildPages, predicate);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
    public static void DrawTitleBar(string subTitle)
    {
        //GUI.Label();
        ElementDesignerStyles.DoTilebar(subTitle);
    }

    public void OnGUI()
    {
        Instance = this;
        //if (disposer == null)
        //{
        //    disposer = InvertApplication.ListenFor<ICommandEvents>(this);
        //}

        //if (disposer2 == null)
        //{
        //    disposer2 = InvertApplication.ListenFor<INodeItemEvents>(this);
        //}
        GUIHelpers.IsInsepctor = false;
        // DrawTitleBar("uFrame Help");

        if (DocumentationProvider == null)
        {
            EditorGUILayout.HelpBox(string.Format("No Help Found"), MessageType.Info);
            return;
        }
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Sidebar", EditorStyles.toolbarButton))
        {
            EditorPrefs.SetBool("uFrameHelpSidebar", !EditorPrefs.GetBool("uFrameHelpSidebar", true));
        }
        if (PageStack.Count > 0)
        {
            if (GUILayout.Button("Back", EditorStyles.toolbarButton))
            {
                PageStack.Pop();
            }
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Export To Html", EditorStyles.toolbarButton))
        {
            var folder = EditorUtility.SaveFolderPanel("Documentation Output Path", null, null);

            if (folder != null)
            {
                var tocDocs = new HtmlDocsBuilder(Pages, "toc", "Screenshots");
#if DEBUG
                tocDocs.PageLinkHandler = page =>
                {
                    return string.Format("<a href=\"/docs/mvvm/{0}.html\">{1}</a>", page.Name.Replace(" ", ""), page.Name);
                };
#endif
                tocDocs.Output.AppendFormat("<div class='toc'>");
                foreach (var page in Pages.OrderBy(p=>p.Order))
                {
                    tocDocs.OutputTOC(page,tocDocs.Output);
                } 
                tocDocs.Output.Append("</div>");
                File.WriteAllText(Path.Combine(folder, "toc.html"), tocDocs.ToString());

                foreach (var page in AllPages())
                {
                    var docsBuilder = new HtmlDocsBuilder(Pages, "content", "Screenshots");
                    tocDocs.Output.AppendFormat("<div class='content'>");
                    page.GetContent(docsBuilder);
                    tocDocs.Output.Append("</div>");
             

                    File.WriteAllText(Path.Combine(folder, page.Name.Replace(" ", "") + ".html"), docsBuilder.ToString());
                }
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (EditorPrefs.GetBool("uFrameHelpSidebar", true))
        {
            tocScrollPosition = EditorGUILayout.BeginScrollView(tocScrollPosition, GUILayout.Width(260));
            EditorGUI.DrawRect(new Rect(tocScrollPosition.x, tocScrollPosition.y, Screen.width, Screen.height), new Color(0.3f, 0.3f, 0.4f));
            ShowPages(Pages);
            EditorGUILayout.EndScrollView();
        }


        pageScrollPosition = EditorGUILayout.BeginScrollView(pageScrollPosition);
        EditorGUI.DrawRect(new Rect(pageScrollPosition.x, pageScrollPosition.y, Screen.width, Screen.height), new Color(0.8f, 0.8f, 0.8f));


        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15f);
        EditorGUILayout.BeginVertical();
        if (CurrentPage != null)
        {
            GUILayout.BeginVertical(new GUIStyle()
            {
                padding = new RectOffset(0,20,10,50),
                alignment = TextAnchor.MiddleCenter
            });
            CurrentPage.GetContent(this);

            if(CurrentPage.ChildPages.Any()) Title2("Pages:");
            foreach (var pages in CurrentPage.ChildPages.OrderBy(x=>x.Order))
            {
                LinkToPage(pages);
            }

            if (false)
                foreach (var childPage in CurrentPage.ChildPages)
                {
                    childPage.PageContent(this);
                }
            GUILayout.EndVertical();
            
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();



        EditorGUILayout.EndHorizontal();
    }

    public IEnumerable<DocumentationPage> AllPages(DocumentationPage parentPage = null)
    {
        if (parentPage == null)
        {
            foreach (var page in Pages)
            {
                yield return page;
                foreach (var item in AllPages(page))
                {
                    yield return item;
                }
            }
        }
        else
        {
            foreach (var childPage in parentPage.ChildPages)
            {
                yield return childPage;
                foreach (var item in AllPages(childPage))
                {
                    yield return item;
                }
            }
        }

    }
    public static GUIStyle Item4
    {
        get
        {
            if (mItem4
                == null)
                mItem4 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item4"), textColor = Color.white },
                    active = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = Color.white},
                    stretchHeight = true,
                    stretchWidth = true,
                    fontSize = 12,
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleLeft
                }.WithFont("Verdana",12);

            return mItem4;
        }
    }
    private void ShowPages(List<DocumentationPage> pages, int indent = 1)
    {

        EditorGUILayout.BeginVertical();
        foreach (var item in pages.OrderBy(p => p.Name))
        {

            if (item == null)
            {
                GUILayout.Label("Item is null");
                continue;
            }
            if (!item.ShowInNavigation) continue;
            if (item.Name == null)
            {
                GUILayout.Label(string.Format("{0} name is null", item.GetType().Name));
                continue;
            }
            if (item.ChildPages.Count(p => p.ShowInNavigation) == 0)
            {
//                if (GUILayout.Button(item.Name, Item4))
//                {
//                    this.ShowPage(item);
//                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(indent * 15f);
                if (GUILayout.Button(item.Name, Item4))
                {
                    this.ShowPage(item);
                }
                EditorGUILayout.EndHorizontal();

                //if (GUIHelpers.DoTriggerButton(new UFStyle(item.Name, ElementDesignerStyles.Item4)
                //{
                //    TextAnchor = TextAnchor.MiddleLeft
                //}))
                //{

                //}
            }
            else
            {

                var item1 = item;
                if (GUIHelpers.DoToolbarEx(item.Name, null, null, null, () => { ShowPage(item1); },true,Color.white))
                {

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(indent * 15f);
                    //if (GUIHelpers.DoTriggerButton(new UFStyle(item.Name,ElementDesignerStyles.EventButtonStyleSmall,ElementDesignerStyles.TriggerInActiveButtonStyle)))
                    //{
                    //    PageStack.Push(item);
                    //}

                    ShowPages(item.ChildPages, indent + 1);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndVertical();

    }

    public DocumentationPage CurrentPage
    {
        get
        {
            if (PageStack.Count < 1)
            {
                if (lastPage != null)
                {
                    var page = FindPage(Pages, p => p.Name == lastPage);
                    if (page != null)
                    {
                        return page;
                    }
                }
                return Pages.FirstOrDefault();
            }


            return PageStack.Peek();
        }
    }
    public Stack<DocumentationPage> PageStack
    {
        get { return mPageStack ?? (mPageStack = new Stack<DocumentationPage>()); }
        set { mPageStack = value; }
    }

    public void BeginArea(string id)
    {

    }

    public void EndArea()
    {

    }

    public void BeginSection(string id)
    {

    }

    public void EndSection()
    {

    }

    public void PushIndent()
    {

    }

    public void PopIndent()
    {

    }

    public void LinkToNode(IDiagramNodeItem node, string text = null)
    {

    }

    public void NodeImage(GraphNode node)
    {

    }

    public void Paragraph(string text, params object[] args)
    {
        GUILayout.Space(5f);
        text = "    "+text;
        if (args == null || args.Length == 0)
        {
            GUILayout.Label(text, ParagraphStyle);
        }
        else
        {
            GUILayout.Label(string.Format(text, args), ParagraphStyle);
        }


    }

    public string EditableParagraph(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            GUILayout.Label("Type some helpful information here...", ParagraphStyle);
        }

        return GUILayout.TextArea(text ?? string.Empty, EditableParagraphStyle, GUILayout.MinWidth(400), GUILayout.MinHeight(40));
    }

    public void Break()
    {
        this.Paragraph(string.Empty);
    }

    public static GUIStyle TitleStyle
    {
        get
        {
            return mTitleStyle ?? (mTitleStyle = new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = new Color(0.3f, 0.3f, 0.4f) },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true,
                    margin = new RectOffset(0,0,10,10)
                    
                });
        }

    }
    public static GUIStyle TutorialActionStyle
    {
        get
        {
            return mTutorialActionStyle ?? (mTutorialActionStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.red },
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                wordWrap = true
            });
        }

    }
    public static GUIStyle EditableParagraphStyle
    {
        get
        {
            if (mEditableParagraphStyle == null)
            {
                mEditableParagraphStyle = new GUIStyle(EditorStyles.textArea)
                {
                   
                    wordWrap = true,
                    margin = new RectOffset(8, 8, 4, 4)
                }.WithFont("Verdana", 14);

                mEditableParagraphStyle.normal.background = null;
                mEditableParagraphStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            }
               
           return mEditableParagraphStyle;
        }
    }
    public static GUIStyle ParagraphStyle
    {
        get
        {
            return mParagraphStyle ?? (mParagraphStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = new Color(0.2f, 0.2f, 0.2f) },
                wordWrap = true,
                margin = new RectOffset(8,8,4,4)
            }).WithFont("Verdana",14);
        }
    }
    public void Lines(params string[] lines)
    {
        foreach (var item in lines)
        {
            GUILayout.Label(item, ParagraphStyle);
        }
    }

    public void Title(string text, params object[] args)
    {
        GUILayout.Space(10f);
        TitleStyle.WithFont("Verdana", 24);
        GUILayout.Label(text, TitleStyle);
        GUILayout.Space(10f);
    }

    public void Title2(string text, params object[] args)
    {
        GUILayout.Space(8f);
        TitleStyle.WithFont("Verdana", 16);
        GUILayout.Label(text, TitleStyle);
    }

    public void Title3(string text, params object[] args)
    {
        GUILayout.Space(5f);
        TitleStyle.WithFont("Verdana", 14);
        GUILayout.Label(text, TitleStyle);

    }

    public void Note(string text, params object[] args)
    {
//        EditorGUILayout.LabelField(GUIContent.none, new GUIContent(text,ElementDesignerStyles.GetSkinTexture("Icon")), EditorStyles.helpBox );

            EditorGUILayout.LabelField(new GUIContent(text,ElementDesignerStyles.GetSkinTexture("NoteIcon")), NoteStyle);
    //    EditorGUILayout.HelpBox(text, MessageType.Info);
    }


    private GUIStyle mNoteStyle;

    public GUIStyle NoteStyle
    {
        get
        {
            var textColor = new Color(0.2f,0.2f,0.2f);
            return mNoteStyle ?? (mNoteStyle = new GUIStyle()
            {
                wordWrap = true,
                stretchWidth = true,
                alignment = TextAnchor.UpperLeft,
                border = new RectOffset(5,5,5,5),
                margin = new RectOffset(20,55,20,0),
                padding = new RectOffset(13,15,15,15),
            
                imagePosition = ImagePosition.ImageLeft,
                stretchHeight = false,
            }).WithFont("Verdana",12).WithAllStates("InfoBackground",textColor);
        }
    }       
    
    public GUIStyle TroubleShootingStyle
    {
        get
        {
            var textColor = new Color(0.2f,0.2f,0.2f);
            return mNoteStyle ?? (mNoteStyle = new GUIStyle()
            {
                wordWrap = true,
                stretchWidth = true,
                alignment = TextAnchor.UpperLeft,
                border = new RectOffset(5,5,5,5),
                margin = new RectOffset(20,55,20,0),
                padding = new RectOffset(13,15,15,15),
            
                imagePosition = ImagePosition.ImageLeft,
                stretchHeight = false,
            }).WithFont("Verdana", 12).WithAllStates("TroubleShootingBackground", textColor);
        }
    }   
    
    private GUIStyle mImageStyle;

    public GUIStyle ImageStyle
    {
        get
        {
            var textColor = new Color(0.2f,0.2f,0.2f);
            return mImageStyle ?? (mImageStyle = new GUIStyle()
            {
                wordWrap = true,
                stretchWidth = true,
                alignment = TextAnchor.LowerCenter,
                border = new RectOffset(5,5,5,5),
                margin = new RectOffset(0,0,20,0),
                padding = new RectOffset(5,5,5,5),
                imagePosition = ImagePosition.ImageAbove,
                stretchHeight = false,
            }).WithFont("Verdana",12).WithAllStates("",textColor);
        }
    }

    private GUIStyle mLinkStyle;

    public GUIStyle LinkStyle
    {
        get
        {
            var textColor = new Color(0.2f,0.2f,0.2f);
            return mLinkStyle ?? (mLinkStyle = new GUIStyle()
            {
                wordWrap = true,
                stretchWidth = true,
                alignment = TextAnchor.UpperLeft,
                border = new RectOffset(5,5,5,5),
                margin = new RectOffset(20,55,20,0),
                padding = new RectOffset(5,5,5,5),
            
                imagePosition = ImagePosition.ImageLeft,
                stretchHeight = false,
            }).WithFont("Verdana", 12).WithAllStates("InfoBackground", textColor);
        }
    }

   


    public void TemplateLink()
    {
    }

    public void Literal(string text, params object[] args)
    {

    }

    public void Section(string text, params object[] args)
    {

    }

    public void Rows(params Action[] actions)
    {

    }

    public void Columns(params Action[] actions)
    {

    }

    public bool DrawImage(string url, string description, params object[] args)
    {
        var finalUrl = string.Format(url, args);
        var texture = GetImage(finalUrl);

        if (string.IsNullOrEmpty(description)) description = finalUrl;


        

        if (texture != null)
        {
//            var rect = GUILayoutUtility.GetRect(Math.Min(Screen.width, texture.width), Math.Min(Screen.width, texture.width), texture.height, texture.height);
//            rect = new Rect(rect.x, rect.y, Math.Min(Screen.width, texture.width), texture.height);
//            return GUI.Button(rect, texture);
            GUILayout.Label(new GUIContent(description, texture), ImageStyle);
        }
        return false;

    }
    public void YouTubeLink(string id)
    {

        if (this.DrawImage("http://img.youtube.com/vi/{0}/mqdefault.jpg", id))
        {
            Application.OpenURL(string.Format("https://www.youtube.com/watch?v={0}", id));
        }
    }

    public void TemplateExample<TTemplate, TData>(TData data, bool isDesignerFile = true, params string[] members)
        where TTemplate : class, IClassTemplate<TData>, new()
        where TData : class, IDiagramNodeItem
    {

//        var tempProject = CreateInstance<TemporaryProjectRepository>();
//        tempProject.CurrentGraph = data.Node.Graph;
//        tempProject.Graphs = new[] { data.Node.Graph };


//        var template = new TemplateClassGenerator<TData, TTemplate>()
//        {
//            Data = data,
//            IsDesignerFile = isDesignerFile,
//            // If we don't have any make sure its null
//            FilterToMembers = members != null && members.Length > 0 ? members : null
//        };
//        var name = "Example of " + data.Name;
//        if (members != null)
//        {
//            name += string.Join(", ", members);
//        }
//        if (isDesignerFile)
//        {
//            name += ".designer";
//        }
//        name += ".cs";
////        

//        var areaCode = string.Format("{0}_{1}", typeof(TTemplate).Name, typeof(TData).Name);
//        if (ExpandableArea(areaCode, name, false))
//        {
//            var codeFileGenerator = new CodeFileGenerator
//            {
//                Generators = new OutputGenerator[] { template },
//                RemoveComments = true
//            };
//            EditorGUILayout.TextArea(codeFileGenerator.ToString(),GistCodeSnippetStyle);
//        }
        
        //if (GUIHelpers.DoToolbarEx(name, null, null, null, null, false, Color.black))
//        {
//            var codeFileGenerator = new CodeFileGenerator
//            {
//                Generators = new OutputGenerator[] { template },
//                RemoveComments = true
//            };
//            EditorGUILayout.TextArea(codeFileGenerator.ToString());
//        }


    }


    public static GUIStyle ExpandedArea
    {
        get
        {
            return mExpandedArea ?? (mExpandedArea = new GUIStyle
            {
                normal =
                {
                    background = ElementDesignerStyles.GetSkinTexture("CommandExpanded"),
                    textColor = !EditorGUIUtility.isProSkin
                        ? new Color(0.2f, 0.2f, 0.2f)
                        : new Color(0.7f, 0.7f, 0.7f)
                },
                //active = { background = CommandBarClosedStyle.normal.background },
                fixedHeight = 28,
                border = new RectOffset(3, 3, 3, 3),

                stretchHeight = true,

                //padding = new RectOffset(5, 5, 5, 0)
            });}
        set { mExpandedArea = value; }
    }


    public void ShowGist(string id, string filename, string userId = "micahosborne")
    {

        GUILayout.Space(10f);

        if (ExpandableArea("Gist: " + filename,
            "Gist: " + filename))
        {
            var content = GetContent(string.Format("https://gist.githubusercontent.com/{1}/{0}/raw", id, userId));
            EditorGUILayout.TextArea(
                content,
                GistCodeSnippetStyle);
        }

//        GUILayout.Space(10f);
//
//        //https://gist.githubusercontent.com/micahosborne/5e04f3cbbd28094edaf5/raw/
//        if (GUIHelpers.DoToolbarEx("Gist: " + filename, null, null, null, null, false, Color.black, CodeSnippetStyle, CodeSnippetStyle))
//        {
//            EditorGUILayout.TextArea(
//                GetContent(string.Format("https://gist.githubusercontent.com/{1}/{0}/raw", id, userId)),CodeSnippetStyle);
//        }

    }
    /// <summary>
    /// Show a tutorial step, and if it returns true, it is the current step.
    /// </summary>
    /// <param name="step"></param>
    /// <param name="stepContent"></param>
    /// <returns></returns>
    public bool ShowTutorialStep(ITutorialStep step, Action<IDocumentationBuilder> stepContent = null)
    {

        CurrentTutorial.Steps.Add(step);

        if (CurrentTutorial.LastStepCompleted)
        {
            step.IsComplete = step.IsDone();
            CurrentTutorial.LastStepCompleted = step.IsComplete == null;
        }
        else
        {
            step.IsComplete = "Waiting for previous step to complete.";
        }


        if (stepContent != null)
        {
            step.StepContent = stepContent;
        }
        return step.IsComplete == null;

        //if (step.IsComplete == null)
        //{
        //    CurrentTutorial.LastStepCompleted = true;
        //    //TutorialActionStyle.fontSize = 13;
        //    //TutorialActionStyle.normal.textColor = new Color(0.3f, 0.6f, 0.3f);
        //    //GUILayout.Label(string.Format("Step {0}: Complete", CurrentTutorial.Steps.IndexOf(step) + 1),
        //    //    TutorialActionStyle);
        //    return false;
        //}
        //else
        //{
        //    CurrentTutorial.LastStepCompleted = false;
        //}
        //Title(string.Format("Step {0}: {1}", CurrentTutorial.Steps.IndexOf(step) + 1, step.Name));
        //Break();
        //Title2("Step Trouble Shooting");

        //TutorialActionStyle.fontSize = 12;
        //TutorialActionStyle.normal.textColor = Color.red;

        //GUILayout.Label(result, TutorialActionStyle);
        //Break();
        //Break();
        //Title2("Study Material");

        //if (stepContent != null)
        //    stepContent(this);
        //else if (step.StepContent != null)
        //{
        //    step.StepContent(this);
        //}





        return true;

    }
    public InteractiveTutorial CurrentTutorial { get; set; }

    public void BeginTutorial(string name)
    {
        CurrentTutorial = new InteractiveTutorial(name);
    }
    public static GUIStyle EventButtonStyleSmall
    {
        get
        {
            var textColor = Color.white;
            if (mEventButtonStyleSmall == null)
                mEventButtonStyleSmall = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = new Color(0.1f, 0.1f, 0.1f) },

                    stretchHeight = true,

                    fixedHeight = 40,
                    border = new RectOffset(3, 3, 3, 3),
                    padding = new RectOffset(25, 0, 5, 5)
                }.WithFont("Verdana",16);

            return mEventButtonStyleSmall;
        }
    }
    public static GUIStyle Item2
    {
        get
        {
            if (mItem2 == null)
                mItem2 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item2"), textColor = Color.white },
                    stretchHeight = true,
                    stretchWidth = true,
                    fixedHeight = 30f,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 0, 0, 0)
                }.WithFont("Verdana", 12);

            return mItem2;
        }
    }
    public static GUIStyle Item5
    {
        get
        {
            if (mItem5 == null)
                mItem5 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item5"), textColor = new Color(0.8f, 0.8f, 0.8f) },
                    stretchHeight = false,
                    //fontSize = Mathf.RoundToInt(12f),
                    alignment = TextAnchor.MiddleLeft,
                    fixedHeight = 30f,
                    padding = new RectOffset(10, 0, 0, 0)
                }.WithFont("Verdana", 12);

            return mItem5;
        }
    }

    public static GUIStyle Item1
    {
        get
        {
            if (mItem1 == null)
                mItem1 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item1"), textColor = Color.white },
                    stretchHeight = true,
                    stretchWidth = true,
                    fixedHeight = 40f,
//                    fontStyle = FontStyle.Bold,
//                    fontSize = Mathf.RoundToInt(14f),
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 0, 0, 0)
                }.WithFont("Verdana",14);

            return mItem1;
        }
    }

    public bool ShowAllSteps
    {
        get { return EditorPrefs.GetBool("UF_SHOWALLSTEPS", false); }
        set { EditorPrefs.SetBool("UF_SHOWALLSTEPS", value); }
    }


    public GUIStyle ToggleAreaOnStyle
    {
        get { return ExpandableAreaHeaderCollapsed; }
    }


    private GUIStyle mShowAllStepsToggleStyle;

    public GUIStyle ShowAllStepsToggleStyle
    {
        get { return mShowAllStepsToggleStyle ?? (mShowAllStepsToggleStyle = new GUIStyle(ExpandableAreaHeaderCollapsed)
        {
            margin = new RectOffset(15,15,15,15)
        }); }
    }

    public InteractiveTutorial EndTutorial()
    {


        GenericToggle("UF_SHOWALLSTEPS", "Show/Hide all steps of the tutorial", false, ShowAllStepsToggleStyle, ShowAllStepsToggleStyle,
            ElementDesignerStyles.GetSkinTexture("EyeToggleOn"), ElementDesignerStyles.GetSkinTexture("EyeToggleOff"));

        //ShowAllSteps = GUILayout.Toggle(ShowAllSteps, "Show All Steps");
        
        
        
        var index = 1;
        bool lastStepComplete = true;

        GUILayout.BeginVertical(TutorialPageStepsContentStyle);
        foreach (var step in CurrentTutorial.Steps)
        {

            var lbl = string.Format(" Step {1}: {2} {0}", step.IsComplete == null ? "COMPLETE" : string.Empty, index,
                step.Name);

            Texture skinTexture = null;

            if (step.IsComplete == null)
            {
                skinTexture = ElementDesignerStyles.GetSkinTexture("StepCompleteIcon");
            }
            else if (lastStepComplete)
            {
                skinTexture = ElementDesignerStyles.GetSkinTexture("CurrentStepIcon");
            }
            else
            {
                skinTexture = ElementDesignerStyles.GetSkinTexture("StepLockedIcon");
            }


            GUILayout.Label(new GUIContent(lbl,skinTexture), step.IsComplete == null ? TutorialStepCompleteHeaderStyle : lastStepComplete ? TutorialStepCurrentHeaderStyle : TutorialStepLockedHeaderStyle);
//            if (ShowAllSteps)
//                Break();
            //GUIHelpers.DoTriggerButton(
            //    new UFStyle(lbl, step.IsComplete == null ? Item2 : lastStepComplete ? Item1 : Item5, null,
            //    step.IsComplete == null ? ElementDesignerStyles.TriggerActiveButtonStyle : ElementDesignerStyles.TriggerInActiveButtonStyle)
            //    {
            //       Enabled = true
            //    });

            if (step.IsComplete != null && lastStepComplete)
            {
                GUILayout.BeginVertical(TutorialStepContentStyle);
                
                
                Title2("Step Trouble Shooting");
                TutorialActionStyle.fontSize = 12;
                TutorialActionStyle.normal.textColor = Color.red;

                //GUILayout.Label(step.IsComplete, TutorialActionStyle);
                EditorGUILayout.LabelField(new GUIContent(step.IsComplete, ElementDesignerStyles.GetSkinTexture("TroubleShootingIcon")), TroubleShootingStyle);

                if (step.StepContent != null)
                {
                    Break();
                    Break();
                    Title2("Study Material");
                    step.StepContent(this);
                }


                

                CurrentTutorial.LastStepCompleted = step.IsComplete == null;
                GUILayout.EndVertical();

            }
            else
            {

                if (ShowAllSteps)
                {
                    if (step.StepContent != null)
                    {
                        GUILayout.BeginVertical(TutorialStepContentStyle);

                        step.StepContent(this);
                        GUILayout.EndVertical();
                    }
                }
            }
            lastStepComplete = step.IsComplete == null;
            index++; if (ShowAllSteps) Break();

        }
        GUILayout.EndVertical();


        //EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.BeginVertical();

        //EditorGUILayout.EndVertical();
        //EditorGUILayout.BeginVertical();

        //EditorGUILayout.EndVertical();
        //EditorGUILayout.EndHorizontal();

        if (CurrentTutorial.LastStepCompleted)
        {
            Break();
            Break();
            TutorialActionStyle.fontSize = 20;
            TutorialActionStyle.normal.textColor = new Color(0.3f, 0.6f, 0.3f);
            GUILayout.Label("Contratulations, you've completed this tutorial.", TutorialActionStyle);
        }
        return CurrentTutorial;
    }

    public GUIStyle TutorialPageStepsContentStyle
    {
        get
        {
            return mTutorialPageStepsContentStyle ?? (mTutorialPageStepsContentStyle = new GUIStyle(NoteStyle)
            {
                padding = new RectOffset(1,1,1,1),
                margin = new RectOffset(),

            }).WithAllStates("TutorialStepsBackground", new Color(0.2f,0.2f,0.2f));
        }
    }

    [FormerlySerializedAs("_tutorialStepCompleteHeaderStyle")] public GUIStyle tutorialStepCompleteHeaderStyle;
    [FormerlySerializedAs("_tutorialStepCurrentHeaderStyle")] public GUIStyle tutorialStepCurrentHeaderStyle;
    [FormerlySerializedAs("_tutorialStepLockedHeaderStyle")] public GUIStyle tutorialStepLockedHeaderStyle;

    public GUIStyle TutorialStepCurrentHeaderStyle
    {
        get
        {
            return tutorialStepCurrentHeaderStyle ??
                   (tutorialStepCurrentHeaderStyle = new GUIStyle(TutorialStepCompleteHeaderStyle)
                   {
                   }).WithAllStates("StepCurrentTitleBackground", new Color(1f, 1f, 1f));
        }
    }
    public GUIStyle TutorialStepLockedHeaderStyle
    {
        get
        {
            return tutorialStepLockedHeaderStyle ??
                   (tutorialStepLockedHeaderStyle = new GUIStyle(TutorialStepCompleteHeaderStyle)
                   {
                   }).WithAllStates("StepLockedTitleBackground", new Color(1f, 1f, 1f));
        }
    }

    public GUIStyle TutorialStepCompleteHeaderStyle
    {
        get
        {
            return tutorialStepCompleteHeaderStyle ?? (tutorialStepCompleteHeaderStyle = new GUIStyle(NoteStyle)
            {
                padding = new RectOffset(10,3,3,3),
                margin = new RectOffset(),
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                imagePosition = ImagePosition.ImageLeft
            }).WithFont("Verdana",14)
            .WithAllStates("StepCompleteTitleBackground", new Color(1f,1f,1f));
        }
    }   
    
    public GUIStyle TutorialStepContentStyle
    {
        get
        {
            return mTutorialStepContentStyle ?? (mTutorialStepContentStyle = new GUIStyle()
            {
                padding = new RectOffset(14,14,14,14),
                margin = new RectOffset(),
            }).WithAllStates("", new Color(0.2f,0.2f,0.2f));
        }
    }

    public void ImageByUrl(string empty, string description = null)
    {
            
        DrawImage(empty,description);
    }

    public void CodeSnippet(string code)
    {
        GUILayout.TextArea(code,CodeSnippetStyle);
    }

    public static GUIStyle CodeSnippetStyle
    {
        get
        {
            Color textColor = new Color(0.2f, 0.2f, 0.2f);
            return mCodeSnippetStyle ?? (mCodeSnippetStyle = new GUIStyle()
        {
            border = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(20, 55, 20, 0),
            padding = new RectOffset(13, 15, 15, 15),
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true
        }).WithFont("Consolas", 12)
        .WithAllStates("InfoBackground",textColor);
        }
        set { mCodeSnippetStyle = value; }
    }    
    
    public static GUIStyle GistCodeSnippetStyle
    {
        get
        {
            Color textColor = new Color(0.2f, 0.2f, 0.2f);
            return mGistCodeSnippetStyle ?? (mGistCodeSnippetStyle = new GUIStyle()
            {
                border = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(20, 55, 0, 0),
                padding = new RectOffset(13, 15, 15, 15),
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                stretchHeight = false
            }).WithFont("Consolas", 12)
        .WithAllStates("InfoBackground_TopLeftFill",textColor);
        }
        set { mGistCodeSnippetStyle = value; }
    }

    public void ToggleContentByNode<TNode>(string name)
    {
        var page = FindPage(Pages, p => p.RelatedNodeType == typeof(TNode));
        if (page == null) return;
        if (GUIHelpers.DoToolbarEx(name ?? page.Name, null, null, null, null, false, Color.black))
        {
            page.GetContent(this);
        }

    }
    public void ToggleContentByPage<TPage>(string name)
    {
        var page = FindPage(Pages, p => p is TPage);
        if (page == null) return;
        if (GUIHelpers.DoToolbarEx(name ?? page.Name, null, null, null, null, false, Color.black))
        {
            page.GetContent(this);
        }

    }

    public void ContentByNode<TNode>()
    {
        var page = FindPage(Pages, p => p.RelatedNodeType == typeof(TNode));
        page.GetContent(this);
    }
    public void ContentByPage<TPage>()
    {
        var page = FindPage(Pages, p => p is TPage);
        page.GetContent(this);
    }

    public void LinkToPage<TPage>()
    {
        var page = FindPage(Pages, p => p is TPage);
        LinkToPage(page);
    }
    
    
    public void LinkToPage(DocumentationPage page)
    {
        if (GUILayout.Button(new GUIContent("Link: " + page.Name,ElementDesignerStyles.GetSkinTexture("LinkIcon")), LinkStyle))
        {
            ShowPage(page);
        }
    }

    public void AlsoSeePages(params Type[] type)
    {
        Title2("Also See");
        foreach (var t in type)
        {
            var t1 = t;
            var page = FindPage(Pages, p => p.GetType() == t1);
            if (page == null) continue;
            LinkToPage(page);
//            if (GUILayout.Button(page.Name))
//            {
//                ShowPage(page);
//            }
        }
    }

    public void TemplateExample<TTemplate, TData>(TData data, bool designerFile = true, string templateMember = null) where TTemplate : IClassTemplate<TData>
    {

        //var a = Activator.CreateInstance(typeof (TTemplate)) as IClassTemplate<TData>;
        //a.Ctx = new TemplateContext<TData>(typeof(TTemplate))
        //{
        //    DataObject = data as IDiagramNodeItem,
        //    IsDesignerFile = designerFile, 
        //    CurrentDecleration = new CodeTypeDeclaration()

        //};
        //  var context = new TemplateContext<TData>(TemplateType);

        //    context.DataObject = Data;
        //    context.Namespace = Namespace;
        //    context.CurrentDecleration = Decleration;
        //    context.IsDesignerFile = IsDesignerFile;

        //a.Ctx.RenderTemplateMethod(a,templateMember);
        //a.Ctx.Render
    }



    private Action mDisposer;
    private static GUIStyle mEventButtonStyleSmall;
    private Action mDisposer2;
    private GUIStyle mExpandableAreaHeaderExpandedStyle;
    private static GUIStyle mItem2;
    private static GUIStyle mItem5;
    private static GUIStyle mItem1;
    private static GUIStyle mItem4;
    private static GUIStyle mCodeSnippetStyle;
    private static GUIStyle mExpandedArea;
    private GUIStyle mExpandableAreaHeaderCollapsedStyle;
    private GUIStyle mToggleAreaOnStyle;
    private static GUIStyle mGistCodeSnippetStyle;
    private GUIStyle mToggleAreaOffStyle;
    private GUIStyle mTutorialPageStepsContentStyle;
    private GUIStyle mTutorialStepContentStyle;
    private static GUIStyle mEditableParagraphStyle;

    public void OnDestory()
    {
        if (mDisposer != null)
        {
            mDisposer();
        }
        if (mDisposer2 != null)
        {
            mDisposer2();
        }
    }


    public void Deleted(IDiagramNodeItem node)
    {
        this.Repaint();
    }

    public void Hidden(IDiagramNodeItem node)
    {
        this.Repaint();
    }

    public void Renamed(IDiagramNodeItem node, string previousName, string newName)
    {
        this.Repaint();
    }


    public bool ExpandableArea(string areaCode,string text, bool defaultOn =false)
    {
        if (!EditorPrefs.HasKey(areaCode))
        {
            EditorPrefs.SetBool(areaCode, defaultOn);
        }



        EditorPrefs.SetBool(
            areaCode,
            GUILayout.Toggle(EditorPrefs.GetBool(areaCode),
                new GUIContent(text, EditorPrefs.GetBool(areaCode) ? AreaExpandedHandleTexture : AreaCollapsedHandleTexture),
                EditorPrefs.GetBool(areaCode) ? ExpandableAreaHeaderExpanded : ExpandableAreaHeaderCollapsed));
        return EditorPrefs.GetBool(areaCode);
    }

    public bool GenericToggle(string settingCode,string text, bool defaultOn =false, GUIStyle onStyle = null, GUIStyle offStyle =null, Texture onTexture = null, Texture offTexture = null)
    {
        if (!EditorPrefs.HasKey(settingCode))
        {
            EditorPrefs.SetBool(settingCode, defaultOn);
        }

        EditorPrefs.SetBool(
            settingCode,
            GUILayout.Toggle(EditorPrefs.GetBool(settingCode),
                new GUIContent(text, EditorPrefs.GetBool(settingCode) ? onTexture : offTexture),
                EditorPrefs.GetBool(settingCode) ? onStyle : offStyle));
        return EditorPrefs.GetBool(settingCode);
    }

    public Texture2D AreaExpandedHandleTexture
    {
        get { return ElementDesignerStyles.GetSkinTexture("CollapseAreaIcon"); } 
    }

    public Texture2D AreaCollapsedHandleTexture
    {
        get { return ElementDesignerStyles.GetSkinTexture("ExpandAreaIcon"); }
    }

    public GUIStyle ExpandableAreaHeaderCollapsed
    {
        get
        {
            Color textColor = new Color(0.2f,0.2f,0.2f);
            return mExpandableAreaHeaderCollapsedStyle ?? (mExpandableAreaHeaderCollapsedStyle = new GUIStyle()
        {
            wordWrap = true,
            stretchWidth = true,
            alignment = TextAnchor.UpperLeft,
            border = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(20, 55, 10, 0),
            padding = new RectOffset(5, 5, 5, 5),
            imagePosition = ImagePosition.ImageLeft,
            stretchHeight = false,
        }).WithAllStates("InfoBackground",textColor)
        .WithFont("Verdana",12);
        }
        set { mExpandableAreaHeaderCollapsedStyle = value; }
    }   
    
    public GUIStyle ExpandableAreaHeaderExpanded
    {
        get
        {
            Color textColor = new Color(0.2f,0.2f,0.2f);
            return mExpandableAreaHeaderExpandedStyle ?? (mExpandableAreaHeaderExpandedStyle = new GUIStyle()
        {
            wordWrap = true,
            stretchWidth = true,
            alignment = TextAnchor.UpperLeft,
            border = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(20, 55, 10, 0),
            padding = new RectOffset(5, 5, 5, 5),
            imagePosition = ImagePosition.ImageLeft,
            stretchHeight = false,
        }).WithAllStates("InfoBackground_BottomCutFill",textColor)
         .WithFont("Verdana", 12);
        }
        set { mExpandableAreaHeaderExpandedStyle = value; }
    }
}