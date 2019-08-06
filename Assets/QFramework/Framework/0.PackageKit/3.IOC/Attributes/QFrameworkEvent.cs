using System;

namespace QF
{
    public class QFrameworkEvent : Attribute
    {
        public string Title { get; set; }

        public QFrameworkEvent(string title)
        {
            Title = title;
        }

    }
}