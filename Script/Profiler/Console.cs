using UnityEngine;
using System.Collections;

/// <summary>
/// 控制台GUI输出类
/// 包括FPS，内存使用情况，日志GUI输出
/// </summary>
public class Console : QSingleton<Console>
{
    /// <summary>
    /// Update回调
    /// </summary>
    public delegate void OnUpdateCallback();
    /// <summary>
    /// OnGUI回调
    /// </summary>
    public delegate void OnGUICallback();

    public OnUpdateCallback onUpdateCallback = null;
    public OnGUICallback onGUICallback = null;
    /// <summary>
    /// FPS计数器
    /// </summary>
    private FPSCounter fpsCounter = null;
    /// <summary>
    /// 内存监视器
    /// </summary>
    private MemoryDetector memoryDetector = null;
    private bool showGUI = false;

    private Console()
    {
        this.fpsCounter = new FPSCounter(this);
        this.memoryDetector = new MemoryDetector(this);
//        this.showGUI = App.Instance().showLogOnGUI;
        App.Instance().onUpdate += Update;
        App.Instance().onGUI += OnGUI;
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetKeyUp(KeyCode.F1))
            this.showGUI = !this.showGUI;
#elif UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
            this.showGUI = this.showGUI;
#elif UNITY_IOS
        if (Input.GetKeyUp(KeyCode.Home))
            this.showGUI = !this.showGUI;
#endif

        if (this.onUpdateCallback != null)
            this.onUpdateCallback();
    }

    void OnGUI()
    {
        if (!this.showGUI)
            return;

        if (this.onGUICallback != null)
            this.onGUICallback();
    }
}
