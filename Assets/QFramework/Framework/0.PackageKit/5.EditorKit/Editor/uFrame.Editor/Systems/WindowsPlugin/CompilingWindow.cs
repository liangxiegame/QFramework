//using UnityEditor;
//using UnityEngine;

//public class CompilingWindow : EditorWindow
//{
//    public string _CompleteCallback;
//    public string _CompleteCallbackArg;
//    public string _CompleteCallbackArg2;
//    private bool _done = false;
//    private float progress = 0;

//    //    private float secs = 100.0f;
//    private int startVal = 0;

//    public static void Init(string completeCallback, string completeCallBackArg,string completeCallBackArg2)
//    {
//        var window = GetWindow<CompilingWindow>();
//        window.startVal = (int)EditorApplication.timeSinceStartup;
//        //window.position = new Rect(-100f,-100f,50f,50f);
//        //window.maxSize = new Vector2(25f, 25f);
//        window._CompleteCallback = completeCallback;
//        window._CompleteCallbackArg = completeCallBackArg;
//        window._CompleteCallbackArg2 = completeCallBackArg2;
//        window._done = false;
//        window.Show();
//    }

//    public static void Init()
//    {
//        var window = GetWindow<CompilingWindow>();
//        window.startVal = (int)EditorApplication.timeSinceStartup;
//        //window.position = new Rect(-100f,-100f,50f,50f);\
//        window._done = false;
//        //window.maxSize = new Vector2(25f, 25f);
//        window.Show();
//    }

//    protected void OnGUI()
//    {
//        progress = (float)EditorApplication.timeSinceStartup - startVal;

//        if (progress < 5)
//        {
//            EditorUtility.DisplayProgressBar("Waiting For Compile", "", progress);
//            return;
//        }

//        if (EditorApplication.isCompiling)
//        {
//            EditorUtility.DisplayProgressBar("Waiting For Compile", "", progress);
//            startVal = (int)EditorApplication.timeSinceStartup;
//        }
//        else
//        {
            
//            EditorUtility.ClearProgressBar();

//            if (!_done && !string.IsNullOrEmpty(_CompleteCallback))
//            {
//                _done = true;
//                typeof(EditorExtensions).GetMethod(_CompleteCallback)
//                .Invoke(_CompleteCallback, new object[] { _CompleteCallbackArg,_CompleteCallbackArg2 });
//            }
//        }
//    }

//    protected void OnInspectorUpdate()
//    {
//        if (_done)
//        {
//            this.Close();
//        }
//        else
//        {
//            Repaint();
//        }
//    }
//}