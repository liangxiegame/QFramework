using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using Invert.Windows;

public class QuickAccessWindowViewModel : IWindow
{
    private readonly QuickAccessContext _context;

    public QuickAccessWindowViewModel(QuickAccessContext context)
    {
        _context = context;
        UpdateSearch();
    }
    public int SelectedIndex { get; set; }
    private List<QuickAccessItem> _quickLaunchItems;
    private string _searchText = "";
    public string Identifier { get; set; }

    public string SearchText
    {
        get { return _searchText; }
        set { _searchText = value; }
    }

    public void UpdateSearch()
    {
        QuickLaunchItems.Clear();
        var launchItems = new List<IItem>();

        InvertApplication.SignalEvent<IQuickAccessEvents>(_ => _.QuickAccessItemsEvents(_context, launchItems));
        
//        foreach (var item in launchItems.SelectMany(p => p))
//        {
//            if (item.Title.Contains(SearchText))
//            {
//                QuickLaunchItems.Add(item);
//            }
//            //if (QuickLaunchItems.Count >= 10)
//            //{
//            //    break;
//            //}
//        }
        GroupedLaunchItems = QuickLaunchItems.GroupBy(p => p.Group).ToArray();

    }

    public IGrouping<string, QuickAccessItem>[] GroupedLaunchItems { get; set; }

    public List<QuickAccessItem> QuickLaunchItems
    {
        get { return _quickLaunchItems ?? (_quickLaunchItems = new List<QuickAccessItem>()); }
        set { _quickLaunchItems = value; }
    }

    

    public void ItemSelected(QuickAccessItem item)
    {
        InvertApplication.Execute(new LambdaCommand("Select Item", () =>
        {
            QuickLaunchItems[SelectedIndex].Action(QuickLaunchItems[SelectedIndex].Item);
        }));
        InvertApplication.SignalEvent<IWindowsEvents>(i=>i.WindowRequestCloseWithViewModel(this));
    }


    public void Execute()
    {
        InvertApplication.Execute(new LambdaCommand("Select Item", () =>
        {
            QuickLaunchItems[SelectedIndex].Action(QuickLaunchItems[SelectedIndex].Item);
        }));
        InvertApplication.SignalEvent<IWindowsEvents>(i => i.WindowRequestCloseWithViewModel(this));
    }

    public void MoveUp()
    {
        if (SelectedIndex <= 0)
            SelectedIndex = QuickLaunchItems.Count -1;
        else
            SelectedIndex--;
    }

    public void MoveDown()
    {
        SelectedIndex = (SelectedIndex+1) % QuickLaunchItems.Count;
    }

    public void Execute(QuickAccessItem item)
    {
        var x = item.Item;
        var z = item;
        InvertApplication.Execute(new LambdaCommand("Select Item", () =>
        {
            z.Action(x);
        }));

        InvertApplication.SignalEvent<IWindowsEvents>(i => i.WindowRequestCloseWithViewModel(this));
    }
}