using NUnit.Framework;

namespace QFramework.Tests
{
    public class ConsoleModuleTests
    {
        [Test]
        public void Title_Set_Get()
        {
            var module = new ConsoleModule();

            module.Title = "Test Console";

            Assert.AreEqual("Test Console", module.Title);
        }

        [Test]
        public void Default_Set_Get()
        {
            var module = new ConsoleModule();

            module.Default = true;

            Assert.IsTrue(module.Default);
        }

        [Test]
        public void DrawGUI_Invokes_OnDrawGUI()
        {
            var module = new ConsoleModule();
            var invoked = false;
            module.OnDrawGUI = () => invoked = true;

            module.DrawGUI();

            Assert.IsTrue(invoked);
        }

        [Test]
        public void OnInit_Does_Not_Throw()
        {
            var module = new ConsoleModule();

            Assert.DoesNotThrow(() => module.OnInit());
        }

        [Test]
        public void OnDestroy_Does_Not_Throw()
        {
            var module = new ConsoleModule();

            Assert.DoesNotThrow(() => module.OnDestroy());
        }

        [Test]
        public void Title_Extension_Returns_Self()
        {
            var module = new ConsoleModule();

            var result = module.Title("x");

            Assert.AreSame(module, result);
            Assert.AreEqual("x", module.Title);
        }

        [Test]
        public void OnGUI_Extension_Sets_Callback()
        {
            var module = new ConsoleModule();
            var invoked = false;

            module.OnGUI(() => invoked = true);
            module.DrawGUI();

            Assert.IsTrue(invoked);
        }

        [Test]
        public void AsDefault_Extension_Sets_Default()
        {
            var module = new ConsoleModule();

            module.AsDefault();

            Assert.IsTrue(module.Default);
        }
    }

    public class ConsoleKitTests
    {
        [SetUp]
        public void SetUp()
        {
            ConsoleKit.RemoveAllModules();

            foreach (var module in ConsoleKit.Modules)
            {
                module.Default = false;
            }
        }

        [TearDown]
        public void TearDown()
        {
            ConsoleKit.RemoveAllModules();

            foreach (var module in ConsoleKit.Modules)
            {
                module.Default = false;
            }
        }

        [Test]
        public void Modules_Default_Has_Two()
        {
            Assert.AreEqual(2, ConsoleKit.Modules.Count);
        }

        [Test]
        public void AddModule_Increases_Count()
        {
            var countBefore = ConsoleKit.Modules.Count;

            ConsoleKit.AddModule(new ConsoleModule().Title("Test"));

            Assert.AreEqual(countBefore + 1, ConsoleKit.Modules.Count);
        }

        [Test]
        public void RemoveAllModules_Keeps_Defaults()
        {
            ConsoleKit.AddModule(new ConsoleModule().Title("Test"));

            ConsoleKit.RemoveAllModules();

            Assert.AreEqual(2, ConsoleKit.Modules.Count);
        }

        [Test]
        public void GetDefaultIndex_Returns_First_Default()
        {
            ConsoleKit.Modules[1].Default = true;
            ConsoleKit.AddModule(new ConsoleModule().Title("Test").AsDefault());

            var result = InvokeGetDefaultIndex();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ShowModule_Sets_Default()
        {
            var module = new ConsoleModule().Title("Test Module");
            ConsoleKit.AddModule(module);

            ConsoleKit.ShowModule("Test Module");

            Assert.IsTrue(module.Default);
        }

        private static int InvokeGetDefaultIndex()
        {
            var methodInfo = typeof(ConsoleKit).GetMethod(
                "GetDefaultIndex",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Assert.IsNotNull(methodInfo);
            return (int)methodInfo.Invoke(null, null);
        }
    }
}
