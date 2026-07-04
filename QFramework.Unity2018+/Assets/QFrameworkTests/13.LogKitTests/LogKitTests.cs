using System;
using System.Text;
using NUnit.Framework;

namespace QFramework.Tests
{
    public class LogKitTests
    {
        [TearDown]
        public void TearDown()
        {
            LogKit.Level = LogKit.LogLevel.Normal;
        }

        [Test]
        public void Level_Default_Is_Normal()
        {
            Assert.AreEqual(LogKit.LogLevel.Normal, LogKit.Level);
        }

        [Test]
        public void Level_Set_Changes_Level()
        {
            LogKit.Level = LogKit.LogLevel.None;

            Assert.AreEqual(LogKit.LogLevel.None, LogKit.Level);
        }

        [Test]
        public void Level_All_Values_Are_Valid()
        {
            Assert.AreEqual(0, (int)LogKit.LogLevel.None);
            Assert.AreEqual(1, (int)LogKit.LogLevel.Exception);
            Assert.AreEqual(2, (int)LogKit.LogLevel.Error);
            Assert.AreEqual(3, (int)LogKit.LogLevel.Warning);
            Assert.AreEqual(4, (int)LogKit.LogLevel.Normal);
            Assert.AreEqual(5, (int)LogKit.LogLevel.Max);
        }

        [Test]
        public void Builder_Returns_New_StringBuilder()
        {
            var builder = LogKit.Builder();

            Assert.IsNotNull(builder);
            Assert.IsInstanceOf<StringBuilder>(builder);
            Assert.AreEqual(0, builder.Length);
        }
    }

    public class LogKitExtensionTests
    {
        [TearDown]
        public void TearDown()
        {
            LogKit.Level = LogKit.LogLevel.Normal;
        }

        [Test]
        public void GreenColor_Wraps_With_Tags()
        {
            var builder = new StringBuilder();

            var result = builder.GreenColor(self => self.Append("Hello"));

            Assert.AreSame(builder, result);
            Assert.AreEqual("<color=green>Hello</color>", builder.ToString());
        }

        [Test]
        public void LogInfo_Extension_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => "Hello".LogInfo());
        }

        [Test]
        public void LogWarning_Extension_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => "Hello".LogWarning());
        }

        [Test]
        public void LogError_Extension_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => "Hello".LogError());
        }

        [Test]
        public void LogException_Extension_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => new Exception("Test").LogException());
        }
    }
}
