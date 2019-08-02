namespace Invert.Windows
{
    public class AreaLayout
    {
        public AreaLayout(int gridOffsetLeft, int gridOffsetTop, int gridWidth, int gridHeight)
        {
            GridOffsetLeft = gridOffsetLeft;
            GridOffsetTop = gridOffsetTop;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

        public int GridOffsetLeft { get; set; }
        public int GridOffsetTop { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
    }
}