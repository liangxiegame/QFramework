using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using QF;
using Invert.Windows;
using Invert.Windows.Unity;
using UnityEditor;
using UnityEngine;

public class WindowsPlugin : DiagramPlugin, IWindowsEvents {
    public override bool Required
    {
        get { return true; }
    }
    private static List<IWindowFactory> _laucherWindows;
    public static List<IWindowDrawer> _windowDrawers; 
    public override void Initialize(QFrameworkContainer container)
    {
        
    }

    public static Rect GetPosition(EditorWindow window)
    {
        var vector2 = new Vector2((Screen.currentResolution.width - window.position.width) / 2, (Screen.currentResolution.height - window.position.height) / 2);
        return new Rect(vector2.x, vector2.y, (float)window.position.width, (float)window.position.height);
    }

    public override void Loaded(QFrameworkContainer container)
    {
        base.Loaded(container);
        LaucherWindows = container.ResolveAll<IWindowFactory>().Where(c => c.ShowInLauncher).ToList();
    }

    public static List<IWindowFactory> LaucherWindows 
    {
        get { return _laucherWindows ?? (_laucherWindows = new List<IWindowFactory>()); }
        set { _laucherWindows = value; }
    }

    public static List<IWindowDrawer> WindowDrawers
    {
        get { return _windowDrawers ?? ( _windowDrawers = new List<IWindowDrawer>()); }
        set { _windowDrawers = value; }
    }

    public void ShowWindow(string factoryId, string title, IWindow viewModel, Vector2 position, Vector2 size)
    {
        var window = GetWindowFor(factoryId, viewModel);
        window.title = title;
        window.ShowAsDropDown(new Rect(position.x, position.y, 1f, 1f), size);
        window.maxSize = size;
        window.minSize = size;
        window.Focus();
        window.Repaint();
    }

    public void ShowWindowNormal(string factoryId, string title, Vector2 position, Vector2 size)
    {
        var window = GetWindowFor(factoryId);
        window.title = title;
        window.Show();
        window.minSize = size;
        window.Focus();
        window.Repaint();
    }

    public void WindowRequestCloseWithArea(Area drawer)
    {
        WindowDrawers.Where(d => d.Drawers.Contains(drawer)).ToList().ForEach(d =>
        {
            (d as EditorWindow).Close();
        });
    }

    public void WindowRefresh(Area drawer)
    {
        WindowDrawers.Where(d => d.Drawers.Contains(drawer)).ToList().ForEach(d =>
        {
            (d as EditorWindow).Repaint();
        });
    }

    public void WindowRequestCloseWithViewModel(IWindow windowViewModel)
    {
        WindowDrawers.Where(d => d.ViewModel == windowViewModel).ToList().ForEach(d =>
        {
            (d as EditorWindow).Close();
        });
    }

    public void ShowWindowPopup<T>(string title, Action<T> configure, Vector2 position, Vector2 size)
    {

        var factory =
            InvertApplication.Container
                .ResolveAll<IWindowFactory>()
                .FirstOrDefault(f => f.ViewModelType == typeof (T));
        if (factory == null) return;
        var window = GetWindowFor(factory);
        configure((T)window.ViewModel);
        window.title = title;
        window.ShowAsDropDown(new Rect(position.x, position.y, 1f, 1f), size);
        window.minSize = size;
        window.Focus();
        window.Repaint();
    }


    public static SmartWindow GetWindowFor(IWindowFactory factory, IWindow viewModel = null,
        bool createNewIfMultipleAllowed = true)
    {
        SmartWindow drawer = null;

        if (factory.Multiple && createNewIfMultipleAllowed)
        {

        }
        else if (factory.Multiple && !createNewIfMultipleAllowed)
        {
            drawer = GetByFactoryId(factory.Identifier).FirstOrDefault() as SmartWindow;
        }
        else if (!factory.Multiple)
        {
            drawer = GetByFactoryId(factory.Identifier).FirstOrDefault() as SmartWindow;
        }

        if (drawer == null)
        {
            drawer = ScriptableObject.CreateInstance<SmartWindow>();
            drawer.WindowFactoryId = factory.Identifier;
            BindDrawerToWindow(drawer, factory, viewModel);
        }

        return drawer;
    }

    public static SmartWindow GetWindowFor(string factoryId, IWindow viewModel = null, bool createNewIfMultipleAllowed = true)
    {
        var factory = InvertApplication.Container.Resolve<IWindowFactory>(factoryId);
        return GetWindowFor(factory, viewModel, createNewIfMultipleAllowed);
    }

    public static IEnumerable<IWindowDrawer> GetByFactoryId(string identifier)
    {
        return WindowDrawers.Where(d => d.WindowFactoryId == identifier);
    }

    public static void BindDrawerToWindow(IWindowDrawer drawer , IWindowFactory factory, IWindow window = null )
    {
        if(window == null) window = factory.GetDefaultViewModelObject(drawer.PersistedData); 
        if(window.GetType() != factory.ViewModelType) throw new Exception("Type of viewmodel != vm type of the factory");
        drawer.PersistedData = null;
        drawer.WindowFactoryId = factory.Identifier;
        drawer.ViewModel = window;
        drawer.RepaintOnUpdate = factory.RepaintOnUpdate;
        if(!WindowDrawers.Contains(drawer))
            WindowDrawers.Add(drawer);
        factory.SetAreasFor(drawer);
    }

    public static void BindDrawerToWindow(IWindowDrawer drawer , string factoryId )
    {
        if (string.IsNullOrEmpty(factoryId)) throw new Exception("Bad bad bad");
        var factory = InvertApplication.Container.Resolve<IWindowFactory>(drawer.WindowFactoryId); 
        BindDrawerToWindow(drawer, factory);
    }

}
