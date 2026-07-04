using NUnit.Framework;

namespace QFramework.Tests
{
    public class EnumEventSystemTests
    {
        enum TestEvent
        {
            EventA = 1,
            EventB = 2,
            EventC = 3
        }

        [Test]
        public void Register_And_Send()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var triggered = false;
            EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { triggered = true; });

            EnumEventSystem.Global.Send(TestEvent.EventA);

            Assert.IsTrue(triggered);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void Register_Returns_UnRegister_Token()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var unRegister = EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { });

            Assert.IsNotNull(unRegister);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void UnRegister_Stops_Delivery()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var callCount = 0;
            var unRegister = EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { callCount++; });

            unRegister.UnRegister();
            EnumEventSystem.Global.Send(TestEvent.EventA);

            Assert.AreEqual(0, callCount);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void UnRegister_By_Key_And_Handler()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var callCount = 0;
            System.Action<int, object[]> handler = (key, args) => { callCount++; };
            EnumEventSystem.Global.Register(TestEvent.EventA, handler);

            EnumEventSystem.Global.UnRegister(TestEvent.EventA, handler);
            EnumEventSystem.Global.Send(TestEvent.EventA);

            Assert.AreEqual(0, callCount);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void UnRegister_Removes_Specific_Handler()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var firstCallCount = 0;
            var secondCallCount = 0;
            System.Action<int, object[]> firstHandler = (key, args) => { firstCallCount++; };
            System.Action<int, object[]> secondHandler = (key, args) => { secondCallCount++; };
            EnumEventSystem.Global.Register(TestEvent.EventA, firstHandler);
            EnumEventSystem.Global.Register(TestEvent.EventA, secondHandler);

            EnumEventSystem.Global.UnRegister(TestEvent.EventA, firstHandler);
            EnumEventSystem.Global.Send(TestEvent.EventA);

            Assert.AreEqual(0, firstCallCount);
            Assert.AreEqual(1, secondCallCount);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void UnRegisterAll_Clears_Everything()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var callCount = 0;
            EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { callCount++; });
            EnumEventSystem.Global.Register(TestEvent.EventB, (key, args) => { callCount++; });

            EnumEventSystem.Global.UnRegisterAll();
            EnumEventSystem.Global.Send(TestEvent.EventA);
            EnumEventSystem.Global.Send(TestEvent.EventB);

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Send_With_No_Listener_Does_Not_Throw()
        {
            EnumEventSystem.Global.UnRegisterAll();

            Assert.DoesNotThrow(() => EnumEventSystem.Global.Send(TestEvent.EventA));

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void Multiple_Handlers_All_Triggered()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var callCount = 0;
            EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { callCount++; });
            EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { callCount++; });
            EnumEventSystem.Global.Register(TestEvent.EventA, (key, args) => { callCount++; });

            EnumEventSystem.Global.Send(TestEvent.EventA);

            Assert.AreEqual(3, callCount);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void Send_Passes_Arguments()
        {
            EnumEventSystem.Global.UnRegisterAll();

            var receivedKey = 0;
            object[] receivedArgs = null;
            EnumEventSystem.Global.Register(TestEvent.EventC, (key, args) =>
            {
                receivedKey = key;
                receivedArgs = args;
            });

            EnumEventSystem.Global.Send(TestEvent.EventC, 10, "hello", true);

            Assert.AreEqual((int)TestEvent.EventC, receivedKey);
            Assert.IsNotNull(receivedArgs);
            Assert.AreEqual(3, receivedArgs.Length);
            Assert.AreEqual(10, receivedArgs[0]);
            Assert.AreEqual("hello", receivedArgs[1]);
            Assert.AreEqual(true, receivedArgs[2]);

            EnumEventSystem.Global.UnRegisterAll();
        }

        [Test]
        public void Global_Instance_Is_Not_Null()
        {
            Assert.IsNotNull(EnumEventSystem.Global);
        }
    }

    public class StringEventSystemTests
    {
        [Test]
        public void Register_And_Send()
        {
            var eventSystem = new StringEventSystem();
            var triggered = false;
            eventSystem.Register("event-a", () => { triggered = true; });

            eventSystem.Send("event-a");

            Assert.IsTrue(triggered);
        }

        [Test]
        public void UnRegister_Stops_Delivery()
        {
            var eventSystem = new StringEventSystem();
            var callCount = 0;
            var unRegister = eventSystem.Register("event-a", () => { callCount++; });

            unRegister.UnRegister();
            eventSystem.Send("event-a");

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Send_Unknown_Key_No_Error()
        {
            var eventSystem = new StringEventSystem();

            Assert.DoesNotThrow(() => eventSystem.Send("unknown-event"));
        }

        [Test]
        public void Register_Generic_And_Send()
        {
            var eventSystem = new StringEventSystem();
            var receivedData = 0;
            eventSystem.Register<int>("event-a", data => { receivedData = data; });

            eventSystem.Send("event-a", 100);

            Assert.AreEqual(100, receivedData);
        }

        [Test]
        public void UnRegister_Generic_Stops_Delivery()
        {
            var eventSystem = new StringEventSystem();
            var callCount = 0;
            var unRegister = eventSystem.Register<int>("event-a", data => { callCount++; });

            unRegister.UnRegister();
            eventSystem.Send("event-a", 100);

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Global_Instance_Is_Not_Null()
        {
            Assert.IsNotNull(StringEventSystem.Global);
        }
    }
}
