namespace QFramework
{
    public class SystemQFrameworkEvent : QFrameworkEvent
    {

        public SystemQFrameworkEvent(string title, string systemMethodName)
            : base(title)
        {
            SystemMethodName = systemMethodName;
        }

        public string SystemMethodName { get; set; }
    }

}