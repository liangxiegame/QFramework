using System;
using QF.GraphDesigner;
using QF;
using UnityEngine;

namespace Invert.Windows
{
    public interface IWindowFactory<TWindow> : IWindowFactory
    {

    }
    public class WindowFactory<TWindow> : IWindowFactory<TWindow> where TWindow : class, IWindow 
    {

        private readonly IQFrameworkContainer container;

        public Type ViewModelType
        {
            get { return typeof(TWindow); }
        }
        
        private event Action<IWindowDrawer> OnSetAreasForWindowDrawer; 

        public string LauncherTitle { get; set; }

        public bool ShowInLauncher { get; set; }
        
        public Texture2D LauncherIcon { get; set; }
        
        public string Identifier { get; set; }

        public Func<string, TWindow> RestoreFunction { get; set; }

        public Func<IWindow> ViewModelFactory { get; set; }

        public string LauncherIdentifier { get; set; }

        public WindowFactory(IQFrameworkContainer container, string identifier)
        {
            this.container = container;
            this.Identifier = identifier;
        }

        public TWindow GetDefaultViewModel(string key)
        {
            if (RestoreFunction != null)
            {
                return RestoreFunction(key);
            }
            return null;
        }

        public IWindow GetDefaultViewModelObject(string key)
        {
            return GetDefaultViewModel(key);
        }

        public void SetAreasFor(IWindowDrawer drawerContainer)
        {
            drawerContainer.Drawers.Clear();

            //if (drawerContainer.ViewModel is IKeyHandler)
            //{
            //    drawerContainer.Drawers.Add(new KeyboardDispatcher()
            //    {
            //        DataSelector = () => drawerContainer.ViewModel as IKeyHandler,
            //        Layout = new AreaLayout(0, 0, 0, 0)
            //    });
            //}

            if(OnSetAreasForWindowDrawer != null)
                OnSetAreasForWindowDrawer(drawerContainer);

        }

        public WindowFactory<TWindow> HasPanel<TArea,TAreaData>(Func<TWindow, TAreaData> selector, AreaLayout layout = null)
        {
            OnSetAreasForWindowDrawer += w =>
            {
                var typedViewModel = w.ViewModel as TWindow;
                if (typedViewModel == null) return;
                var area = (Area<TAreaData>)Activator.CreateInstance(typeof(TArea));
                InvertApplication.Container.Inject(area);
                area.DataSelector = () => selector(typedViewModel);
                area.Layout = layout ?? new AreaLayout(0,0,16,16);
                w.Drawers.Add(area);
            };

            return this;
        }

        public WindowFactory<TWindow> HasPanel<TArea,TAreaData>(AreaLayout layout = null) where TAreaData : class, TWindow
        {
            this.HasPanel<TArea, TAreaData>(w => w as TAreaData,layout);

            return this;
        }




        public WindowFactory<TWindow> WithLaucherEntry(TWindow instance, string title, Texture2D texture = null)
        {
            LauncherTitle = title;
            LauncherIcon = texture;
            ShowInLauncher = true;
            return this;
        }    
        
        public WindowFactory<TWindow> WithLaucherEntry(Func<TWindow> vmFactory, string title, Texture2D texture = null)
        {
            LauncherTitle = title;
            LauncherIcon = texture;
            ShowInLauncher = true;
            ViewModelFactory = ()=>(IWindow)vmFactory();
            return this;
        }

        public WindowFactory<TWindow> WithDefaultInstance(Func<string, TWindow> selector)
        {
            RestoreFunction = selector;
            return this;
        }       
        
        public WindowFactory<TWindow> WithRepaintOnUpdate(bool repaint)
        {
            RepaintOnUpdate = repaint;
            return this;
        }

        public bool RepaintOnUpdate { get; set; }

        public WindowFactory<TWindow> AllowMultiple()
        {
            this.Multiple = true;
            return this;
        }

        public bool Multiple { get; set; }
    }

    

}


