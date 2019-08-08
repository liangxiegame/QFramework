namespace QF.GraphDesigner
{
    public interface INotify
    {
        void Notify(string message, string icon, int time = 5000 );
        void Notify(string message, NotificationIcon icon, int time = 5000);
        void NotifyWithActions(string message, NotificationIcon icon, params NotifyActionItem[] actions);
    }
}