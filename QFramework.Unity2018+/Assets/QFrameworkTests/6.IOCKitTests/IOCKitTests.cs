using NUnit.Framework;

namespace QFramework.Tests
{
    public class InjectAttributeTests
    {
        [Test]
        public void Default_Constructor() { Assert.IsNull(new QFramework.InjectAttribute().Name); }

        [Test]
        public void Named_Constructor() { Assert.AreEqual("primary", new QFramework.InjectAttribute("primary").Name); }
    }

    public class QFrameworkContainerTests
    {
        [Test]
        public void Register_And_Resolve()
        {
            var container = new QFramework.QFrameworkContainer();
            container.Register<IFoo, Foo>();

            var result = container.Resolve<IFoo>();

            Assert.IsInstanceOf<Foo>(result);
            Assert.AreEqual("Foo", result.Name);
        }

        [Test]
        public void Register_Instance_Resolve_Returns_Same()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();

            container.RegisterInstance(foo);
            var result = container.Resolve<IFoo>();

            Assert.AreSame(foo, result);
        }

        [Test]
        public void Register_Instance_With_Name()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();

            container.RegisterInstance(foo, "primary");
            var result = container.Resolve<IFoo>("primary");

            Assert.AreSame(foo, result);
        }

        [Test]
        public void Resolve_Returns_Null_When_Not_Registered()
        {
            Assert.IsNull(new QFramework.QFrameworkContainer().Resolve<IFoo>());
        }

        [Test]
        public void Resolve_With_RequireInstance_Returns_Null()
        {
            var container = new QFramework.QFrameworkContainer();
            container.Register<IFoo, Foo>();

            var result = container.Resolve<IFoo>(requireInstance: true);

            Assert.IsNull(result);
        }

        [Test]
        public void Inject_Sets_Field_With_Inject_Attribute()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();
            var target = new FieldInjectTarget();

            container.RegisterInstance(foo);
            container.Inject(target);

            Assert.AreSame(foo, target.FooField);
        }

        [Test]
        public void Inject_Sets_Property_With_Inject_Attribute()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();
            var target = new PropertyInjectTarget();

            container.RegisterInstance(foo);
            container.Inject(target);

            Assert.AreSame(foo, target.FooProperty);
        }

        [Test]
        public void Inject_With_Named_Registration()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo primary = new Foo();
            IFoo secondary = new Bar();
            var target = new NamedInjectTarget();

            container.RegisterInstance(primary, "primary");
            container.RegisterInstance(secondary, "secondary");
            container.Inject(target);

            Assert.AreSame(primary, target.Primary);
            Assert.AreSame(secondary, target.Secondary);
        }

        [Test]
        public void Inject_Does_Nothing_On_Null()
        {
            var container = new QFramework.QFrameworkContainer();

            Assert.DoesNotThrow(() => container.Inject(null));
        }

        [Test]
        public void Clear_Removes_All()
        {
            var container = new QFramework.QFrameworkContainer();
            container.Register<IFoo, Foo>();
            container.RegisterInstance<SimpleClass>(new SimpleClass());
            container.RegisterRelation<RelationContext, IFoo, Foo>();

            container.Clear();

            Assert.IsNull(container.Resolve<IFoo>());
            Assert.IsNull(container.Resolve<SimpleClass>(requireInstance: true));
            Assert.IsNull(container.ResolveRelation<RelationContext, IFoo>());
        }

        [Test]
        public void CreateInstance_With_No_Args()
        {
            var container = new QFramework.QFrameworkContainer();

            var result = (SimpleClass)container.CreateInstance(typeof(SimpleClass));

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void CreateInstance_With_Explicit_Args()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();

            var result = (DependentClass)container.CreateInstance(typeof(DependentClass), foo);

            Assert.AreSame(foo, result.Foo);
        }

        [Test]
        public void ResolveAll_Returns_All_Named_Registrations()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo primary = new Foo();
            IFoo secondary = new Bar();

            container.RegisterInstance(primary, "primary");
            container.RegisterInstance(secondary, "secondary");

            var count = 0;
            var foundPrimary = false;
            var foundSecondary = false;
            foreach (var foo in container.ResolveAll<IFoo>())
            {
                count++;
                foundPrimary |= ReferenceEquals(primary, foo);
                foundSecondary |= ReferenceEquals(secondary, foo);
            }

            Assert.AreEqual(2, count);
            Assert.IsTrue(foundPrimary);
            Assert.IsTrue(foundSecondary);
        }

        [Test]
        public void RegisterRelation_And_Resolve()
        {
            var container = new QFramework.QFrameworkContainer();
            container.RegisterRelation<RelationContext, IFoo, Foo>();

            var result = container.ResolveRelation<RelationContext, IFoo>();

            Assert.IsInstanceOf<Foo>(result);
        }

        [Test]
        public void ResolveRelation_Returns_Null_When_Not_Registered()
        {
            Assert.IsNull(new QFramework.QFrameworkContainer().ResolveRelation<RelationContext, IFoo>());
        }

        [Test]
        public void InjectAll_Injects_All_Registered_Instances()
        {
            var container = new QFramework.QFrameworkContainer();
            IFoo foo = new Foo();
            var firstTarget = new FieldInjectTarget();
            var secondTarget = new PropertyInjectTarget();

            container.RegisterInstance(typeof(FieldInjectTarget), firstTarget, "first", false);
            container.RegisterInstance(typeof(PropertyInjectTarget), secondTarget, "second", false);
            container.RegisterInstance<IFoo>(foo, false);

            container.InjectAll();

            Assert.AreSame(foo, firstTarget.FooField);
            Assert.AreSame(foo, secondTarget.FooProperty);
        }
    }

    public class TupleTests
    {
        [Test]
        public void Equals_Same_Values()
        {
            var first = new QFramework.Tuple<int, string>(1, "one");
            var second = new QFramework.Tuple<int, string>(1, "one");

            Assert.AreEqual(first, second);
        }

        [Test]
        public void Equals_Different_Values()
        {
            var first = new QFramework.Tuple<int, string>(1, "one");
            var second = new QFramework.Tuple<int, string>(2, "two");

            Assert.AreNotEqual(first, second);
        }

        [Test]
        public void GetHashCode_Consistent()
        {
            var first = new QFramework.Tuple<int, string>(1, "one");
            var second = new QFramework.Tuple<int, string>(1, "one");

            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Test]
        public void Null_Handling()
        {
            var first = new QFramework.Tuple<string, string>(null, null);
            var second = new QFramework.Tuple<string, string>(null, null);
            var different = new QFramework.Tuple<string, string>(null, "value");

            Assert.AreEqual(first, second);
            Assert.AreEqual(0, first.GetHashCode());
            Assert.AreNotEqual(first, different);
        }
    }

    public class TypeMappingCollectionTests
    {
        [Test]
        public void Indexer_Set_And_Get()
        {
            var collection = new QFramework.TypeMappingCollection();

            collection[typeof(IFoo), "primary"] = typeof(Foo);

            Assert.AreEqual(typeof(Foo), collection[typeof(IFoo), "primary"]);
        }

        [Test]
        public void Indexer_Returns_Null_When_Missing()
        {
            Assert.IsNull(new QFramework.TypeMappingCollection()[typeof(IFoo), "missing"]);
        }
    }

    public class TypeInstanceCollectionTests
    {
        [Test]
        public void Indexer_Set_And_Get()
        {
            var collection = new QFramework.TypeInstanceCollection();
            IFoo foo = new Foo();

            collection[typeof(IFoo), "primary"] = foo;

            Assert.AreSame(foo, collection[typeof(IFoo), "primary"]);
        }

        [Test]
        public void Indexer_Returns_Null_When_Missing()
        {
            Assert.IsNull(new QFramework.TypeInstanceCollection()[typeof(IFoo), "missing"]);
        }
    }

    public interface IFoo { string Name { get; } }
    public class Foo : IFoo { public string Name => "Foo"; }
    public class Bar : IFoo { public string Name => "Bar"; }
    public class FieldInjectTarget { [QFramework.Inject] public IFoo FooField; }
    public class PropertyInjectTarget { [QFramework.Inject] public IFoo FooProperty { get; set; } }
    public class NamedInjectTarget
    {
        [QFramework.Inject("primary")] public IFoo Primary;
        [QFramework.Inject("secondary")] public IFoo Secondary;
    }
    public class SimpleClass { public int Value; }
    public class DependentClass { public IFoo Foo; public DependentClass(IFoo foo) { Foo = foo; } }
    public class RelationContext { }
}
