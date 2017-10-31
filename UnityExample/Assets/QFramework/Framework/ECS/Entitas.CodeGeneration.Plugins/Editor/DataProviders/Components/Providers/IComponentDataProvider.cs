using System;

namespace QFramework.CodeGeneration.Plugins {

    public interface IComponentDataProvider {

        void Provide(Type type, ComponentData data);
    }
}
