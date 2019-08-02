using System;

namespace QFramework.GraphDesigner
{
    public class TemplateException :Exception
    {
        public TemplateException(string message) : base(message)
        {
        }

        public TemplateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}