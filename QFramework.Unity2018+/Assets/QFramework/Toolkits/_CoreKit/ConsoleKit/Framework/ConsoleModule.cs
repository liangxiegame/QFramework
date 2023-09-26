using System;

namespace QFramework
{
    public class ConsoleModule
    {
        public virtual string Title { get; set; }

        public virtual Action OnDrawGUI { get; set; }

        public virtual void OnInit(){}
        public virtual void DrawGUI()
        {
            OnDrawGUI?.Invoke();
        }
        
        public virtual void OnDestroy(){}
    }

    public static class ConsoleModuleExtensions
    {
        public static T Title<T>(this T self, string title) where T : ConsoleModule
        {
            self.Title = title;
            return self;
        }
        
        public static T OnGUI<T>(this T self, Action onGUI) where T : ConsoleModule
        {
            self.OnDrawGUI = onGUI;
            return self;
        }
    }
}