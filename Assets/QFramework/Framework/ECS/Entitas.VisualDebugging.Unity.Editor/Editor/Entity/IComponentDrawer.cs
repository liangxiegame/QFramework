using System;


namespace QFramework.VisualDebugging.Unity.Editor {

    public interface IComponentDrawer {

        bool HandlesType(Type type);

        IComponent DrawComponent(IComponent component);
    }
}
