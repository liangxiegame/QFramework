using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace QFramework.Tests
{
    public class ListPoolTests
    {
        [Test]
        public void Get_Returns_New_When_Empty()
        {
            var list = ListPool<ListPoolGetNewItem>.Get();

            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Get_Returns_Pooled()
        {
            var list = ListPool<ListPoolGetPooledItem>.Get();

            ListPool<ListPoolGetPooledItem>.Release(list);
            var pooledList = ListPool<ListPoolGetPooledItem>.Get();

            Assert.AreSame(list, pooledList);
        }

        [Test]
        public void Release_Clears_List()
        {
            var list = ListPool<ListPoolReleaseClearsItem>.Get();
            list.Add(new ListPoolReleaseClearsItem());

            ListPool<ListPoolReleaseClearsItem>.Release(list);
            var pooledList = ListPool<ListPoolReleaseClearsItem>.Get();

            Assert.AreSame(list, pooledList);
            Assert.AreEqual(0, pooledList.Count);
        }

        [Test]
        public void Release_Throws_On_Double_Release()
        {
            var list = ListPool<ListPoolDoubleReleaseItem>.Get();

            ListPool<ListPoolDoubleReleaseItem>.Release(list);

            Assert.Throws<InvalidOperationException>(() => ListPool<ListPoolDoubleReleaseItem>.Release(list));
        }

        [Test]
        public void Release2Pool_Extension()
        {
            var list = ListPool<ListPoolExtensionItem>.Get();

            list.Release2Pool();
            var pooledList = ListPool<ListPoolExtensionItem>.Get();

            Assert.AreSame(list, pooledList);
        }
    }

    public class DictionaryPoolTests
    {
        [Test]
        public void Get_Returns_New_When_Empty()
        {
            var dictionary = DictionaryPool<DictionaryPoolGetNewKey, DictionaryPoolGetNewValue>.Get();

            Assert.IsNotNull(dictionary);
            Assert.AreEqual(0, dictionary.Count);
        }

        [Test]
        public void Get_Returns_Pooled()
        {
            var dictionary = DictionaryPool<DictionaryPoolGetPooledKey, DictionaryPoolGetPooledValue>.Get();

            DictionaryPool<DictionaryPoolGetPooledKey, DictionaryPoolGetPooledValue>.Release(dictionary);
            var pooledDictionary = DictionaryPool<DictionaryPoolGetPooledKey, DictionaryPoolGetPooledValue>.Get();

            Assert.AreSame(dictionary, pooledDictionary);
        }

        [Test]
        public void Release_Clears_Dictionary()
        {
            var dictionary = DictionaryPool<DictionaryPoolReleaseClearsKey, DictionaryPoolReleaseClearsValue>.Get();
            dictionary.Add(new DictionaryPoolReleaseClearsKey(), new DictionaryPoolReleaseClearsValue());

            DictionaryPool<DictionaryPoolReleaseClearsKey, DictionaryPoolReleaseClearsValue>.Release(dictionary);
            var pooledDictionary = DictionaryPool<DictionaryPoolReleaseClearsKey, DictionaryPoolReleaseClearsValue>.Get();

            Assert.AreSame(dictionary, pooledDictionary);
            Assert.AreEqual(0, pooledDictionary.Count);
        }

        [Test]
        public void Release2Pool_Extension()
        {
            var dictionary = DictionaryPool<DictionaryPoolExtensionKey, DictionaryPoolExtensionValue>.Get();

            dictionary.Release2Pool();
            var pooledDictionary = DictionaryPool<DictionaryPoolExtensionKey, DictionaryPoolExtensionValue>.Get();

            Assert.AreSame(dictionary, pooledDictionary);
        }
    }

    public class DefaultObjectFactoryTests
    {
        [Test]
        public void Create_Returns_New_Instance()
        {
            var factory = new DefaultObjectFactory<PublicCtorClass>();

            var instance = factory.Create();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Create_Returns_Different_Instances()
        {
            var factory = new DefaultObjectFactory<PublicCtorClass>();

            var first = factory.Create();
            var second = factory.Create();

            Assert.AreNotSame(first, second);
        }
    }

    public class CustomObjectFactoryTests
    {
        [Test]
        public void Create_Uses_Factory_Method()
        {
            var calls = 0;
            var expected = new PublicCtorClass(99);
            var factory = new CustomObjectFactory<PublicCtorClass>(() =>
            {
                calls++;
                return expected;
            });

            var instance = factory.Create();

            Assert.AreSame(expected, instance);
            Assert.AreEqual(1, calls);
        }
    }

    public class NonPublicObjectFactoryTests
    {
        [Test]
        public void Create_Uses_Private_Constructor()
        {
            var factory = new NonPublicObjectFactory<PrivateCtorClass>();

            var instance = factory.Create();

            Assert.IsNotNull(instance);
            Assert.AreEqual(42, instance.Value);
        }

        [Test]
        public void Create_Throws_When_No_Private_Ctor()
        {
            var factory = new NonPublicObjectFactory<PublicCtorClass>();

            Assert.Throws<Exception>(() => factory.Create());
        }
    }

    public class ObjectFactoryTests
    {
        [Test]
        public void Create_With_Type()
        {
            var instance = ObjectFactory.Create(typeof(PublicCtorClass));

            Assert.IsInstanceOf<PublicCtorClass>(instance);
            Assert.AreEqual(1, ((PublicCtorClass)instance).Value);
        }

        [Test]
        public void Create_Generic()
        {
            var instance = ObjectFactory.Create<PublicCtorClass>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.Value);
        }

        [Test]
        public void Create_With_Constructor_Args()
        {
            var instance = ObjectFactory.Create<PublicCtorClass>(7);

            Assert.AreEqual(7, instance.Value);
        }

        [Test]
        public void CreateNonPublicConstructorObject()
        {
            var instance = ObjectFactory.CreateNonPublicConstructorObject<PrivateCtorClass>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(42, instance.Value);
        }

        [Test]
        public void CreateWithInitialAction()
        {
            var callbackInvoked = false;

            var instance = ObjectFactory.CreateWithInitialAction<PublicCtorClass>(obj =>
            {
                callbackInvoked = true;
                obj.Value = 11;
            });

            Assert.IsTrue(callbackInvoked);
            Assert.AreEqual(11, instance.Value);
        }
    }

    public class PoolableObjectTests
    {
        [Test]
        public void Allocate_Returns_Instance()
        {
            var instance = SimplePoolable.Allocate();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Allocate_After_Recycle_Returns_Pooled()
        {
            var instance = SimplePoolable.Allocate();

            instance.Recycle2Cache();
            var pooledInstance = SimplePoolable.Allocate();

            Assert.AreSame(instance, pooledInstance);
        }

        [Test]
        public void Recycle2Cache_Calls_OnRecycle()
        {
            var instance = SimplePoolable.Allocate();
            var recycleCount = instance.RecycleCount;

            instance.Recycle2Cache();

            Assert.AreEqual(recycleCount + 1, instance.RecycleCount);
        }
    }

    class SimplePoolable : PoolableObject<SimplePoolable>
    {
        public int RecycleCount;

        protected override void OnRecycle()
        {
            RecycleCount++;
        }
    }

    class PrivateCtorClass
    {
        public int Value;

        private PrivateCtorClass()
        {
            Value = 42;
        }
    }

    class PublicCtorClass
    {
        public int Value;

        public PublicCtorClass()
        {
            Value = 1;
        }

        public PublicCtorClass(int v)
        {
            Value = v;
        }
    }

    class ListPoolGetNewItem { }

    class ListPoolGetPooledItem { }

    class ListPoolReleaseClearsItem { }

    class ListPoolDoubleReleaseItem { }

    class ListPoolExtensionItem { }

    class DictionaryPoolGetNewKey { }

    class DictionaryPoolGetNewValue { }

    class DictionaryPoolGetPooledKey { }

    class DictionaryPoolGetPooledValue { }

    class DictionaryPoolReleaseClearsKey { }

    class DictionaryPoolReleaseClearsValue { }

    class DictionaryPoolExtensionKey { }

    class DictionaryPoolExtensionValue { }
}
