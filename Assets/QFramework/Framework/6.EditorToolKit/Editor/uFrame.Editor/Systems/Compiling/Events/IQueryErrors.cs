using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public interface IQueryErrors
    {
        void QueryErrors(List<ErrorInfo> errorInfo);
    }
}