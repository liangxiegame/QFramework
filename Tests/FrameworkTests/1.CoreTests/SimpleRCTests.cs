using NUnit.Framework;

namespace QFramework.Tests
{
    public class SimpleRCTests
    {
        class Light
        {
            public bool Opening { get; private set; }
            public void Open()
            {
                Opening = true;
            }

            public void Close()
            {
                Opening = false;
            }
        }

        class Room : SimpleRC
        {
            public readonly Light Light = new Light();

            public void EnterPeople()
            {
                if (RefCount == 0)
                {
                    Light.Open();
                }

                Retain();
                
            }

            public void LeavePeople()
            {
                // 这里才真正的走出了
                Release();
            }

            protected override void OnZeroRef()
            {
                Light.Close();
            }
        }
        
        [Test]
        public void SimpleRC_Test()
        {
            var room = new Room();

            Assert.AreEqual(room.RefCount, 0);
            Assert.IsFalse(room.Light.Opening);
            
            room.EnterPeople();
            
            Assert.AreEqual(room.RefCount, 1);
            Assert.IsTrue(room.Light.Opening);

            room.EnterPeople();
            room.EnterPeople();
            
            Assert.AreEqual(room.RefCount, 3);

            room.LeavePeople();
            room.LeavePeople();
            room.LeavePeople();
            
            Assert.IsFalse(room.Light.Opening);
            Assert.AreEqual(room.RefCount, 0);

            room.EnterPeople();
            
            Assert.AreEqual(room.RefCount, 1);
        }
    }
}