using System;
using QF.GraphDesigner;
using QF;

namespace Invert.Windows {

    public abstract class Area<TData> : Area
    {
        private IPlatformDrawer _drawer;
        public Func<TData> DataSelector { get; set; }

        public override void Draw()
        {
            Draw(DataSelector());
        }

        public abstract void Draw(TData data);

    }

    public abstract class Area
    {
        public abstract void Draw();
        public AreaLayout Layout { get; set; }
    }
}