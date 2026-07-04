using NUnit.Framework;

namespace QFramework.Tests
{
    public class SingletonTests
    {
        [SetUp]
        public void SetUp()
        {
            TestSingleton.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            TestSingleton.Reset();
        }

        [Test]
        public void Instance_Returns_Same_Object()
        {
            var first = TestSingleton.Instance;
            var second = TestSingleton.Instance;

            Assert.AreSame(first, second);
        }

        [Test]
        public void OnSingletonInit_Called()
        {
            Assert.AreEqual(0, TestSingleton.InitCount);

            var instance = TestSingleton.Instance;
            var secondAccess = TestSingleton.Instance;

            Assert.AreSame(instance, secondAccess);
            Assert.AreEqual(1, TestSingleton.InitCount);
        }

        [Test]
        public void Dispose_Nullifies_Instance()
        {
            var instance = TestSingleton.Instance;

            Assert.IsTrue(TestSingleton.HasInstance);

            instance.Dispose();

            Assert.IsFalse(TestSingleton.HasInstance);
        }

        [Test]
        public void Re_Access_After_Discrete_Creates_New()
        {
            var first = TestSingleton.Instance;
            var firstId = first.Id;

            first.Dispose();
            var second = TestSingleton.Instance;

            Assert.AreNotSame(first, second);
            Assert.AreNotEqual(firstId, second.Id);
            Assert.AreEqual(2, TestSingleton.InitCount);
        }

        [Test]
        public void Thread_Safety_Concurrent_Access()
        {
            var instances = new TestSingleton[64];

            System.Threading.Tasks.Parallel.For(0, instances.Length, i =>
            {
                instances[i] = TestSingleton.Instance;
            });

            var expected = instances[0];
            for (var i = 1; i < instances.Length; i++)
            {
                Assert.AreSame(expected, instances[i]);
            }

            Assert.AreEqual(1, TestSingleton.InitCount);
        }

        class TestSingleton : Singleton<TestSingleton>
        {
            public static int InitCount = 0;
            static int mNextId = 0;
            public int Id = ++mNextId;

            private TestSingleton()
            {
            }

            public static bool HasInstance
            {
                get { return mInstance != null; }
            }

            public static void Reset()
            {
                mInstance = null;
                InitCount = 0;
                mNextId = 0;
            }

            public override void OnSingletonInit()
            {
                InitCount++;
            }
        }
    }

    public class SingletonPropertyTests
    {
        [SetUp]
        public void SetUp()
        {
            TestSingletonProperty.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            TestSingletonProperty.Reset();
        }

        [Test]
        public void Instance_Returns_Same_Object()
        {
            var first = TestSingletonProperty.Instance;
            var second = TestSingletonProperty.Instance;

            Assert.AreSame(first, second);
        }

        [Test]
        public void OnSingletonInit_Called()
        {
            Assert.AreEqual(0, TestSingletonProperty.InitCount);

            var instance = TestSingletonProperty.Instance;
            var secondAccess = TestSingletonProperty.Instance;

            Assert.AreSame(instance, secondAccess);
            Assert.AreEqual(1, TestSingletonProperty.InitCount);
        }

        [Test]
        public void Dispose_Nullifies_Instance()
        {
            var instance = TestSingletonProperty.Instance;

            Assert.IsTrue(TestSingletonProperty.HasInstance);

            instance.Dispose();

            Assert.IsFalse(TestSingletonProperty.HasInstance);
        }

        [Test]
        public void Re_Access_After_Discrete_Creates_New()
        {
            var first = TestSingletonProperty.Instance;
            var firstId = first.Id;

            first.Dispose();
            var second = TestSingletonProperty.Instance;

            Assert.AreNotSame(first, second);
            Assert.AreNotEqual(firstId, second.Id);
            Assert.AreEqual(2, TestSingletonProperty.InitCount);
        }

        class TestSingletonProperty : ISingleton
        {
            public static int InitCount = 0;
            static int mNextId = 0;
            public int Id = ++mNextId;

            private TestSingletonProperty()
            {
            }

            public static TestSingletonProperty Instance
            {
                get { return SingletonProperty<TestSingletonProperty>.Instance; }
            }

            public static bool HasInstance
            {
                get
                {
                    var fieldInfo = typeof(SingletonProperty<TestSingletonProperty>).GetField("mInstance",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    return fieldInfo.GetValue(null) != null;
                }
            }

            public static void Reset()
            {
                SingletonProperty<TestSingletonProperty>.Dispose();
                InitCount = 0;
                mNextId = 0;
            }

            public void OnSingletonInit()
            {
                InitCount++;
            }

            public void Dispose()
            {
                SingletonProperty<TestSingletonProperty>.Dispose();
            }
        }
    }
}
