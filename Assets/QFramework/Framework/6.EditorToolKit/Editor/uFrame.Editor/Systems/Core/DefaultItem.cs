namespace QF.GraphDesigner
{
    public class DefaultItem : IItem
    {
        public DefaultItem(string title, string @group)
        {
            Title = title;
            Group = @group;
            SearchTag = Title + " " + group;
        }

        public string Title { get; set; }
        public string Group { get; set; }
        public string SearchTag { get; set; }
        public string Description { get; set; }
    }
}