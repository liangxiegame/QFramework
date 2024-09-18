using System;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace QFramework.Tests
{
    public class ListPoolTest
    {
        [Test]
        public void ListPool_ReleaseTest()
        {
            var list = ListPool<string>.Get();
            
            list.Release2Pool();

            try
            {
                list.Release2Pool();
                Assert.IsTrue(false);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
