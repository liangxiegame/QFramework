using System.Linq;
using ModestTree;
using UnityEngine;

#pragma warning disable 219

namespace Zenject
{
    public class CheatSheet : Installer<CheatSheet>
    {
        public override void InstallBindings()
        {
            // Create a new instance of Foo for every class that asks for it
            Container.Bind<Foo>().AsTransient();

            // Create a new instance of Foo for every class that asks for an IFoo
            Container.Bind<IFoo>().To<Foo>().AsTransient();

            // Non generic version of the above
            Container.Bind(typeof(IFoo)).To(typeof(Foo)).AsTransient();

            ///////////// AsSingle

            // Create one definitive instance of Foo and re-use that for every class that asks for it
            Container.Bind<Foo>().AsSingle();

            // Create one definitive instance of Foo and re-use that for every class that asks for IFoo
            Container.Bind<IFoo>().To<Foo>().AsSingle();

            // Bind the same instance to multiple types
            // In this example, the same instance of Foo will be used for all three types
            // (we have to use the non-generic version of Bind when mapping to multiple types)
            Container.Bind(typeof(Foo), typeof(IFoo), typeof(IFoo2)).To<Foo>().AsSingle();

            ///////////// BindInterfaces

            // This will have the exact same effect as the above line
            // Bind all interfaces that Foo implements and Foo itself to a new singleton of type Foo
            Container.BindInterfacesAndSelfTo<Foo>().AsSingle();

            // Bind only the interfaces that Foo implements to an instance of Foo
            // This can be useful if you don't want any classes to directly reference the concrete
            // derived type
            Container.BindInterfacesTo<Foo>().AsSingle();

            ///////////// FromInstance

            // Use the given instance everywhere that Foo is used
            // Note that in this case there's no good reason to use FromInstance
            Container.Bind<Foo>().FromInstance(new Foo());

            // This is simply a shortcut for the above binding
            // This can be a bit nicer since the type argument can be deduced from the parameter
            Container.BindInstance(new Foo());

            // Bind multiple instances at once
            Container.BindInstances(new Foo(), new Bar());

            ///////////// Binding primitive types

            // BindInstance is more commonly used with primitive types
            // Use the number 10 every time an int is requested
            Container.Bind<int>().FromInstance(10);
            Container.Bind<bool>().FromInstance(false);

            // Or equivalently:
            Container.BindInstance(10);
            Container.BindInstance(false);

            // You'd never really want to do the above though - you should almost always use a When condition for primitive values
            Container.BindInstance(10).WhenInjectedInto<Foo>();

            ///////////// FromMethod

            // Create instance of Foo when requested, using the given method
            // Note that for more complex construction scenarios, you might consider using a factory
            // instead with FromFactory
            Container.Bind<Foo>().FromMethod(GetFoo);

            // Randomly return one of several different implementations of IFoo
            // We use Instantiate here instead of just new so that Foo1 gets its members injected
            Container.Bind<IFoo>().FromMethod(GetRandomFoo);

            // You an also use an anonymouse delegate directly
            Container.Bind<Foo>().FromMethod(ctx => new Foo());

            // This is equivalent to AsTransient
            Container.Bind<Foo>().FromMethod(ctx => ctx.Container.Instantiate<Foo>());

            InstallMore();
        }

        Foo GetFoo(InjectContext ctx)
        {
            return new Foo();
        }

        IFoo GetRandomFoo(InjectContext ctx)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                {
                    return ctx.Container.Instantiate<Foo1>();
                }
                case 1:
                {
                    return ctx.Container.Instantiate<Foo2>();
                }
            }

            return ctx.Container.Instantiate<Foo3>();
        }

        void InstallMore()
        {
            ///////////// FromResolveGetter

            // Bind to a property on another dependency
            // This can be helpful to reduce coupling between classes
            Container.Bind<Foo>().AsSingle();

            Container.Bind<Bar>().FromResolveGetter<Foo>(foo => foo.GetBar());

            // Another example using values
            Container.Bind<string>().FromResolveGetter<Foo>(foo => foo.GetTitle());

            ///////////// FromNewComponentOnNewGameObject

            // Create a new game object at the root of the scene and add the Foo MonoBehaviour to it
            Container.Bind<Foo>().FromNewComponentOnNewGameObject().AsSingle();

            // You can also specify the game object name to use using WithGameObjectName
            Container.Bind<Foo>().FromNewComponentOnNewGameObject().WithGameObjectName("Foo1").AsSingle();

            // Bind to an interface instead
            Container.Bind<IFoo>().To<Foo>().FromNewComponentOnNewGameObject().AsSingle();

            ///////////// FromComponentInNewPrefab (singleton)

            // Create a new game object at the root of the scene using the given prefab
            // After zenject creates a new GameObject from the given prefab, it will
            // search the prefab for a component of type 'Foo' and return that
            GameObject prefab = null;
            Container.Bind<Foo>().FromComponentInNewPrefab(prefab).AsSingle();

            // Bind to interface instead
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(prefab).AsSingle();

            // You can also add multiple components
            // Note here that only one instance of the given prefab will be
            // created
            // For this to work, there must be both a Foo MonoBehaviour and
            // a Bar MonoBehaviour somewhere on the prefab
            Container.Bind(typeof(Foo), typeof(Bar)).FromComponentInNewPrefab(prefab).AsSingle();

            ///////////// FromComponentInNewPrefab (Transient)

            // Instantiate a new copy of 'prefab' every time an instance of Foo is
            // requested by a constructor parameter, injected field, etc.
            Container.Bind<Foo>().FromComponentInNewPrefab(prefab).AsTransient();

            // Bind to interface instead
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(prefab);

            ///////////// Identifiers

            // Bind a globally accessible string with the name 'PlayerName'
            // Note however that a better option might be to create a Settings object and bind
            // that instead
            Container.Bind<string>().WithId("PlayerName").FromInstance("name of the player");

            // This is the equivalent of the line above, and is a bit more readable
            Container.BindInstance("name of the player").WithId("PlayerName");

            // We can also use IDs to bind multiple instances of the same type:
            Container.BindInstance("foo").WithId("FooA");
            Container.BindInstance("asdf").WithId("FooB");

            InstallMore2();
        }

        // Then when we inject these dependencies we have to use the same ID:
        public class Norf
        {
            [Inject(Id = "FooA")]
            public string Foo;
        }

        public class Qux
        {
            [Inject(Id = "FooB")]
            public string Foo;
        }

        public void InstallMore2()
        {
            ///////////// AsCached

            // In this example, we bind three instances of Foo, including one without an ID
            // We have to use AsCached here because Foo is not a singleton, but we also
            // do not want a new Foo created every time like AsTransient
            // This will result in a maximum of 3 instances of Foo
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().WithId("FooA").AsCached();
            Container.Bind<Foo>().WithId("FooA").AsCached();

            InstallMore3();
        }

        // When an ID is unspecified in an [Inject] field, it will use the first
        // instance
        // Bindings without IDs can therefore be used as a default and we can
        // specify IDs for specific versions of the same type
        public class Norf2
        {
            [Inject]
            public Foo Foo;
        }

        // Qux2._foo will be the same instance as Norf2._foo
        // This is because we are using AsCached rather than AsTransient
        public class Qux2
        {
            [Inject]
            public Foo Foo;

            [Inject(Id = "FooA")]
            public Foo Foo2;
        }

        public void InstallMore3()
        {
            ///////////// Conditions

            // This will make Foo only visible to Bar
            // If we add Foo to the constructor of any other class it won't find it
            Container.Bind<Foo>().AsSingle().WhenInjectedInto<Bar>();

            // Use different implementations of IFoo dependending on which
            // class is being injected
            Container.Bind<IFoo>().To<Foo1>().AsSingle().WhenInjectedInto<Bar>();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            // Use "Foo1" as the default implementation except when injecting into
            // class Qux, in which case use Foo2
            // This works because if there is a condition match, that takes precedence
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            // Allow depending on Foo in only a few select classes
            Container.Bind<Foo>().AsSingle().WhenInjectedInto(typeof(Bar), typeof(Qux), typeof(Baz));

            // Supply "my game" for any strings that are injected into the Gui class with the identifier "Title"
            Container.BindInstance("my game").WithId("Title").WhenInjectedInto<Gui>();

            // Supply 5 for all ints that are injected into the Gui class
            Container.BindInstance(5).WhenInjectedInto<Gui>();

            // Supply 5 for all ints that are injected into a parameter or field
            // inside type Gui that is named 'width'
            // Note that this is usually not a good idea since the name of a field can change
            // easily and break the binding but shown here as an example of a more complex
            // condition
            Container.BindInstance(5.0f).When(ctx =>
                ctx.ObjectType == typeof(Gui) && ctx.MemberName == "width");

            // Create a new 'Foo' for every class that is created as part of the
            // construction of the 'Bar' class
            // So if Bar has a constructor parameter of type Qux, and Qux has
            // a constructor parameter of type IFoo, a new Foo will be created
            // for that case
            Container.Bind<IFoo>().To<Foo>().AsTransient().When(
                ctx => ctx.AllObjectTypes.Contains(typeof(Bar)));

            ///////////// Complex conditions example

            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Bar>().WithId("Bar1").AsCached();
            Container.Bind<Bar>().WithId("Bar2").AsCached();

            // Here we use the 'ParentContexts' property of inject context to sync multiple corresponding identifiers
            Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar1")).Any());
            Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar2")).Any());

            // This results in:
            Assert.That(Container.ResolveId<Bar>("Bar1").Foo == foo1);
            Assert.That(Container.ResolveId<Bar>("Bar2").Foo == foo2);

            ///////////// FromResolve

            // FromResolve does another lookup on the container
            // This will result in IBar, IFoo, and Foo, all being bound to the same instance of
            // Foo which is assume to exist somewhere on the given prefab
            GameObject fooPrefab = null;
            Container.Bind<Foo>().FromComponentInNewPrefab(fooPrefab).AsSingle();
            Container.Bind<IBar>().To<Foo>().FromResolve();
            Container.Bind<IFoo>().To<IBar>().FromResolve();

            // This will result in the same behaviour as the above
            Container.Bind(typeof(Foo), typeof(IBar), typeof(IFoo)).To<Foo>().FromComponentInNewPrefab(fooPrefab).AsSingle();

            InstallMore4();
        }

        public class FooInstaller : Installer<FooInstaller>
        {
            public FooInstaller(string foo)
            {
            }

            public override void InstallBindings()
            {
            }
        }

        public class FooInstallerWithArgs : Installer<string, FooInstallerWithArgs>
        {
            public FooInstallerWithArgs(string foo)
            {
            }

            public override void InstallBindings()
            {
            }
        }

        void InstallMore4()
        {
            ///////////// Installing Other Installers

            // Immediately call InstallBindings() on FooInstaller
            FooInstaller.Install(Container);

            // Before calling FooInstaller, configure a property of it
            Container.BindInstance("foo").WhenInjectedInto<FooInstaller>();
            FooInstaller.Install(Container);

            // The arguments can also be added to the Installer<> generic arguments to make them
            // strongly typed
            FooInstallerWithArgs.Install(Container, "foo");

            ///////////// Manual Use of Container

            // This will fill in any parameters marked as [Inject] and also call any [Inject] methods
            var foo = new Foo();
            Container.Inject(foo);

            // Return an instance for IFoo, using the bindings that have been added previously
            // Internally it is what is triggered when you fill in a constructor parameter of type IFoo
            // Note: It will throw an exception if it cannot find a match
            Container.Resolve<IFoo>();

            // Same as the above except returns null when it can't find the given type
            Container.TryResolve<IFoo>();

            // Return a list of 2 instances of type Foo
            // Note that in this case simply calling Resolve<IFoo> will trigger an exception
            Container.BindInstance(new Foo());
            Container.BindInstance(new Foo());
            var foos = Container.ResolveAll<IFoo>();

            // Create a new instance of Foo and inject on any of its members
            // And fill in any constructor parameters Foo might have
            Container.Instantiate<Foo>();

            GameObject prefab1 = null;
            GameObject prefab2 = null;

            // Instantiate a new prefab and have any injectables filled in on the prefab
            GameObject go = Container.InstantiatePrefab(prefab1);

            // Instantiate a new prefab and return a specific monobehaviour
            Foo foo2 = Container.InstantiatePrefabForComponent<Foo>(prefab2);

            // Add a new component to an existing game object
            Foo foo3 = Container.InstantiateComponent<Foo>(go);
        }

        public interface IFoo2
        {
        }

        public interface IFoo
        {
        }

        public interface IBar : IFoo
        {
        }

        public class Foo : MonoBehaviour, IFoo, IFoo2, IBar
        {
            public Bar GetBar()
            {
                return new Bar();
            }

            public string GetTitle()
            {
                return "title";
            }
        }

        public class Foo1 : IFoo
        {
        }

        public class Foo2 : IFoo
        {
        }

        public class Foo3 : IFoo
        {
        }

        public class Baz
        {
        }

        public class Gui
        {
        }

        public class Bar : IBar
        {
            public Foo Foo
            {
                get
                {
                    return null;
                }
            }
        }
    }
}
