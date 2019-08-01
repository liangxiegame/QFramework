using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    // This class just exists to solve a circular dependency that would otherwise happen if we
    // attempted to inject TickableManager into either SignalDeclaration or SignalBus
    // And we need to directly depend on TickableManager because we need each SignalDeclaration
    // to have a unique tick priority
    public class SignalDeclarationAsyncInitializer : IInitializable
    {
        readonly LazyInject<TickableManager> _tickManager;
        readonly List<SignalDeclaration> _declarations;

        public SignalDeclarationAsyncInitializer(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> declarations,
            [Inject(Optional = true, Source = InjectSources.Local)]
            LazyInject<TickableManager> tickManager)
        {
            _declarations = declarations;
            _tickManager = tickManager;
        }

        public void Initialize()
        {
            for (int i = 0; i < _declarations.Count; i++)
            {
                var declaration = _declarations[i];

                if (declaration.IsAsync)
                {
                    Assert.IsNotNull(_tickManager.Value, "TickableManager is required when using asynchronous signals");
                    _tickManager.Value.Add(declaration, declaration.TickPriority);
                }
            }
        }
    }
}

