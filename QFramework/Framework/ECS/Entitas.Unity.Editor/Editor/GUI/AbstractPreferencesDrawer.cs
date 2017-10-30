using Entitas.Utils;

namespace Entitas.Unity.Editor
{

    public abstract class AbstractPreferencesDrawer : IEntitasPreferencesDrawer
    {

        public abstract int Priority { get; }
        public abstract string Title { get; }

        protected bool mDrawContent = true;

        public abstract void Initialize(Properties properties);

        public void Draw(Properties properties)
        {
            mDrawContent = EntitasEditorLayout.DrawSectionHeaderToggle(Title, mDrawContent);
            if (mDrawContent)
            {
                EntitasEditorLayout.BeginSectionContent();
                {
                    DrawContent(properties);
                }
                EntitasEditorLayout.EndSectionContent();
            }
        }

        protected abstract void DrawContent(Properties properties);
    }
}