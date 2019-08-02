using System.Collections.Generic;

namespace QFramework.GraphDesigner
{
    public interface IQueryErrors
    {
        void QueryErrors(List<ErrorInfo> errorInfo);
    }
}