using System.Collections.Generic;
using QFramework.GraphDesigner;

public interface IQuickAccessEvents
{
    void QuickAccessItemsEvents(QuickAccessContext context, List<IItem> items);
}