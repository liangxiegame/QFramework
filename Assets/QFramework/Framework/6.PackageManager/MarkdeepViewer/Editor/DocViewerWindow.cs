using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Timers;
using System.Reflection;
using QFramework;
using Timer = System.Timers.Timer;

public class DocViewerWindow : EditorWindow, IHasCustomMenu
{
    [SerializeField] ScriptableObject webView;
    [SerializeField] ScriptableObject scriptObject;

    static Type webViewType;
    static MethodInfo loadURLMethod;
    private static MethodInfo loadFileMethod;
    static MethodInfo showMethod;
    static MethodInfo hideMethod;
    static MethodInfo focusMethod;
    static MethodInfo setSizePositionMethod;
    static MethodInfo setHostViewMethod;
    static MethodInfo reloadMethod;
    static MethodInfo defineScriptObjectMethod;
    static MethodInfo backMethod;
    static MethodInfo forwardMethod;
    private static MethodInfo executeJavascriptMethod;

    static FieldInfo parentField;

    public string urlText;

    protected bool m_SyncingFocus;
    protected bool m_HasDelayedRefresh;
    protected Timer m_PostLoadTimer;

    protected string pathToMarkdeep;
    protected string pathToStylesheet;

    object previousParent = null;

    static public DocViewerWindow Load(string url)
    {
        InitFieldMapping();

        DocViewerWindow window = DocViewerWindow.GetWindow<DocViewerWindow>();
        window.urlText = url;
        window.Init();

        window.ShowUtility();

        return window;
    }

    static void InitFieldMapping()
    {
        if (webViewType == null)
        {
            //Get WebView type
            webViewType = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.WebView");

            parentField = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);

            loadURLMethod = webViewType.GetMethod("LoadURL");
            loadFileMethod = webViewType.GetMethod("LoadFile");

            focusMethod = webViewType.GetMethod("SetFocus");
            showMethod = webViewType.GetMethod("Show");
            hideMethod = webViewType.GetMethod("Hide");
            setHostViewMethod = webViewType.GetMethod("SetHostView");
            reloadMethod = webViewType.GetMethod("Reload");
            executeJavascriptMethod = webViewType.GetMethod("ExecuteJavascript");
            defineScriptObjectMethod = webViewType.GetMethod("DefineScriptObject");
            backMethod = webViewType.GetMethod("Back");
            forwardMethod = webViewType.GetMethod("Forward");

            setSizePositionMethod = webViewType.GetMethod("SetSizeAndPosition");
        }
    }

    void Init()
    {
        //Init web view
        InitWebView();
    }

    public virtual void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
    }

    private void OnEnable()
    {
        InitFieldMapping();
#if SINGLE_FILE
        //single file version copy it's own embeded file data into file in the Library folder it will link to.
        CopyFiles();
#else
        string[] markdeepGUID = AssetDatabase.FindAssets("markdeep.min");
        if (markdeepGUID.Length == 0)
        {
            Debug.LogError("Couldn't find markdeep local copy. Markdown to markdeep auto conversion won't work.");
        }
        else
        {
            pathToMarkdeep = AssetDatabase.GUIDToAssetPath(markdeepGUID[0]).Replace("Assets", Application.dataPath);
        }

        markdeepGUID = AssetDatabase.FindAssets(EditorGUIUtility.isProSkin ? "dark_style" : "light_style");
        if (markdeepGUID.Length == 0)
        {
            Debug.LogError("Couldn't find CSS style local copy. Default style will be used, display may be hard to read.");
        }
        else
        {
            pathToStylesheet = AssetDatabase.GUIDToAssetPath(markdeepGUID[0]).Replace("Assets", Application.dataPath);
        }
#endif
    }

#if SINGLE_FILE
    void CopyFiles()
    {
        string lightpath = Application.dataPath + "/../Library/docmd/light_style.css";
        string darkpath = Application.dataPath + "/../Library/docmd/dark_style.css";

        pathToMarkdeep = Application.dataPath + "/../Library/docmd/markdeep.min.jscript";
        pathToStylesheet = Application.dataPath + "/../Library/docmd/"+
                           (EditorGUIUtility.isProSkin ? "dark_style" : "light_style") + ".css";

        if (!System.IO.Directory.Exists(Application.dataPath + "/../Library/docmd"))
            System.IO.Directory.CreateDirectory(Application.dataPath + "/../Library/docmd");

        if (!System.IO.File.Exists(pathToMarkdeep))
        {
            System.IO.File.WriteAllText(pathToMarkdeep, MARKDEEP_CONTENT);
        }

        if (!System.IO.File.Exists(lightpath))
        {
            System.IO.File.WriteAllText(lightpath, LIGHT_STYLE_CONTENT);
        }

        if (!System.IO.File.Exists(darkpath))
        {
            System.IO.File.WriteAllText(darkpath, DARK_STYLE_CONTENT);
        }
    }
#endif

    bool OnLocationChanged(string url)
    {
        //bool md = System.IO.Path.GetExtension(url).ToLower() == ".md";
        bool md = url.Contains(".md");

        ExecuteJS("window.styleCheckWaitForProcess = false;");

        // All the javascript is unreadable here cause multiple instruction packed into single lines, but easier that way.
        //TODO : maybe make that nicer to read, would help debug
        if (md)
        {
            ExecuteJS("var lnk = document.createElement(\"meta\"); lnk.setAttribute(\"charset\",\"utf-8\");document.head.appendChild(lnk);");
            ExecuteJS(
                "window.styleCheckWaitForProcess = true; window.alreadyProcessedMarkdeep = false; var containScript = false; var scripts = document.getElementsByTagName(\"script\"); for (var i = 0; i < scripts.length; ++i) { if( scripts[i].src.includes(\"markdeep.min.js\") ){containScript = true;} }; if(!containScript){document.body.innerHTML = document.body.children[0].innerText; var v = document.createElement(\'script\'); v.src=\"" +
                pathToMarkdeep +
                "\"; document.body.appendChild(v);}");
        }


        //check if there is a stylesheet linked to the document, if there is none, this is either a pure markdown file, a "default style" markdeep, or "basic HTML" file.
        //it then assign the default style (choosen depending on the skin of the editor) to it.
        ExecuteJS("function addStyle(){var needStyle = true; var a = document.head.getElementsByTagName(\"link\"); for(var i = 0, len = a.length; i < len; i++) { if(a[i].rel == \"stylesheet\") needStyle = false; }; if(needStyle) { var lnk = document.createElement(\"link\"); lnk.rel = \"stylesheet\"; lnk.href = \"" + pathToStylesheet + "?\"; document.head.appendChild(lnk); }}");

        ExecuteJS("function checkMarkdeepProcessed() { if(window.styleCheckWaitForProcess && !window.alreadyProcessedMarkdeep) { setTimeout(checkMarkdeepProcessed, 50); return; } addStyle();}; checkMarkdeepProcessed();");

        return true;
    }

    public void OnInitScripting()
    {
        Type webScriptObjectType = System.Type.GetType("UnityEditor.Web.WebScriptObject, UnityEditor");

        this.scriptObject = ScriptableObject.CreateInstance(webScriptObjectType);
        this.scriptObject.hideFlags = HideFlags.HideAndDontSave;

        PropertyInfo info = webScriptObjectType.GetProperty("webView");

        info.SetValue(this.scriptObject, this.webView, null);

        defineScriptObjectMethod.Invoke(webView, new object[] { "window.unityScriptObject", (ScriptableObject)this.scriptObject });
    }

    protected virtual void InitWebView()
    {
        if (this.webView == null)
        {
            previousParent = parentField.GetValue(this);
            this.webView = CreateInstance(webViewType);
            webViewType.GetMethod("InitWebView").Invoke(webView,
                new object[]
                {
                    previousParent, 0, (int) EditorGUIUtility.singleLineHeight * 3, (int) position.width,
                    (int) position.height, false
                });
            this.webView.hideFlags = HideFlags.HideAndDontSave;
            this.SetFocus(true);
        }

        webViewType.GetMethod("SetDelegateObject").Invoke(webView, new object[] {this});
        this.LoadUri();
        this.SetFocus(true);
    }

    void ExecuteJS(string js)
    {
        executeJavascriptMethod.Invoke(webView, new object[] {js});
    }

    public void LoadUri()
    {
        if (this.urlText.StartsWith("http"))
        {
            loadURLMethod.Invoke(webView, new object[] {urlText});
            this.m_PostLoadTimer = new Timer(30.0);
            this.m_PostLoadTimer.Elapsed += new ElapsedEventHandler(this.RaisePostLoadCondition);
            this.m_PostLoadTimer.Enabled = true;
        }
        else if (this.urlText.StartsWith("file"))
        {
            loadFileMethod.Invoke(webView, new object[] {this.urlText});
        }
        else
        {
            string path =
                Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")),
                    this.urlText);
            loadFileMethod.Invoke(webView, new object[] {path});
        }
    }

    private void RaisePostLoadCondition(object obj, ElapsedEventArgs args)
    {
        this.m_PostLoadTimer.Stop();
        this.m_PostLoadTimer = null;
        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update,
            new EditorApplication.CallbackFunction(this.DoPostLoadTask));
    }

    private void OnWebViewDirty()
    {
        this.Repaint();
    }

    public void Reload()
    {
        if (this.webView != null)
        {
            reloadMethod.Invoke(webView, null);
        }
    }

    public void OnBatchMode()
    {
        if (this.urlText != null)
        {
            if (this.webView == null)
            {
                this.InitWebView();
            }
        }
    }

    void OnGUI()
    {
        Rect buttonsRect = new Rect(0, 0, 64, (int)EditorGUIUtility.singleLineHeight * 2);
        Rect rect = new Rect(0, (int)EditorGUIUtility.singleLineHeight * 3, (int)position.width, (int)position.height - (int)EditorGUIUtility.singleLineHeight * 2);

        if (GUI.Button(buttonsRect, "Back"))
        {
            backMethod.Invoke(webView, new object[] { });
            SetFocus(true);
        }

        buttonsRect.x += buttonsRect.width;
        if (GUI.Button(buttonsRect, "Forward"))
        {
            forwardMethod.Invoke(webView, new object[] { });
            SetFocus(true);
        }

        buttonsRect.x += buttonsRect.width;
        if (GUI.Button(buttonsRect, "Reload"))
        {
            Reload();
            SetFocus(true);
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (this.m_HasDelayedRefresh)
            {
                this.Refresh();
                this.m_HasDelayedRefresh = false;
            }
        }

        if (this.urlText != null)
        {
            if (webView == null)
            {
                this.InitWebView();
            }

            if (Event.current.type == EventType.Repaint)
            {
                object parent = parentField.GetValue(this);
                setHostViewMethod.Invoke(webView, new object[] {parent});
                setSizePositionMethod.Invoke(webView,
                    new object[] {(int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height});
            }
        }
    }

    void OnDestroy()
    {
        DestroyImmediate(webView);
    }

    public void OnBecameInvisible()
    {
        if (this.webView != null)
        {
            setHostViewMethod.Invoke(webView, new object[] {null});
        }
    }

    public void Refresh()
    {
        if (this.webView != null)
        {
            hideMethod.Invoke(webView, null);
            showMethod.Invoke(webView, null);
        }
    }

    public void ToggleMaximize()
    {
        base.maximized = !base.maximized;
        this.Refresh();
        this.SetFocus(true);
    }

    public void OnFocus()
    {
        this.SetFocus(true);
    }

    public void OnLostFocus()
    {
        this.SetFocus(false);
    }

    public void OnOpenExternalLink(string url)
    {

    }

    void OnBeginLoading(string url)
    {

    }

    void OnFinishLoading(string url)
    {

    }

    public void OnLoadError(string url)
    {
        Debug.Log("load error : " + url);
    }

    private void DoPostLoadTask()
    {
        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update,
            new EditorApplication.CallbackFunction(this.DoPostLoadTask));
        Repaint();
    }

    private void SetFocus(bool value)
    {
        if (!this.m_SyncingFocus)
        {
            this.m_SyncingFocus = true;
            if (this.webView != null)
            {
                if (value)
                {
                    setHostViewMethod.Invoke(webView, new object[] {parentField.GetValue((this))});
                    showMethod.Invoke(webView, null);
                }

                focusMethod.Invoke(webView, new object[] {value});
            }

            this.m_SyncingFocus = false;
        }
    }
}