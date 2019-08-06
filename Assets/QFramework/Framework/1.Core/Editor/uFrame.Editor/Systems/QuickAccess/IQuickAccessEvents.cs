using System.Collections.Generic;
using QF.GraphDesigner;

public interface IQuickAccessEvents
{
    void QuickAccessItemsEvents(QuickAccessContext context, List<IItem> items);
}