using System;

namespace QFramework {

    public interface IComponentDataProvider {

        void Provide(Type type, ComponentData data);
    }
}
