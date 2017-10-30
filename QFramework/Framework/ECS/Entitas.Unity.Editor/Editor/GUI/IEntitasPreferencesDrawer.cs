using Entitas.Utils;

namespace Entitas.Unity.Editor
{
    public interface IEntitasPreferencesDrawer
    {
        int Priority { get; }

        string Title { get; }

        void Initialize(Properties properties);

        void Draw(Properties properties);
    }
}
