using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace QFramework.Tests
{
    public class ActionKit0_0_6Tests
    {

        [UnityTest]
        public IEnumerator UntilActionTest()
        {
            var watch = new Stopwatch();
            var untilAction = UntilAction.Allocate(() => Time.time > 3);
            
            watch.Start();
            while (!untilAction.Execute(Time.deltaTime))
            {
                yield return new WaitForEndOfFrame();
            }

            watch.Stop();

            Assert.Greater(watch.ElapsedMilliseconds, 2000);
            Assert.Less(watch.ElapsedMilliseconds, 3100);
        }

        [Test]
        public void OnlyBeginActionTest()
        {
            var called = false;
            var onlyBeginAction = OnlyBeginAction.Allocate((action) =>
            {

                called = true;
            });

            onlyBeginAction.Execute(Time.deltaTime);
            
            Assert.IsTrue(called);
        }

        [Test]
        public void EventActionTest()
        {
            var called = false;
            
            var eventAction = EventAction.Allocate(() => { called = true; });
            
            eventAction.Execute(Time.deltaTime);

            Assert.IsTrue(called);
        }

        [UnityTest]
        public IEnumerator DelayActionTest()
        {
            var watch = new Stopwatch();
            watch.Start();
            var delayAction = DelayAction.Allocate(1, () =>
            {
                watch.Stop();
            });

            while (!delayAction.Execute(Time.deltaTime))
            {
                yield return new WaitForEndOfFrame();
            }
            
            Assert.Greater(watch.ElapsedMilliseconds,900);
            Assert.Less(watch.ElapsedMilliseconds,1100);
        }
        

        [UnityTest]
        public IEnumerator RepeatNodeTest()
        {
            var callCount = 0;

            var delayAction = DelayAction.Allocate(1.0f, () => { callCount++; });

            var repeatNode = new RepeatNode(delayAction, 2);
            
            while (!repeatNode.Execute(Time.deltaTime))
            {
                yield return null;
            }

            Assert.AreEqual(2, callCount);
        }


        [UnityTest]
        public IEnumerator TimelineNodeTest()
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            var timelineNode = new Timeline();
            timelineNode.Append(1.0f,EventAction.Allocate(() =>
            {
                stopwatch.Stop();
            }));
            
            while (!timelineNode.Execute(Time.deltaTime))
            {
                yield return null;
            }

            Assert.Greater(stopwatch.ElapsedMilliseconds, 900);
        }
        
        [UnityTest]
        public IEnumerator SpawnNodeTest()
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            var spawnNode = new SpawnNode();
            spawnNode.Add(DelayAction.Allocate(1, () => {  }));
            spawnNode.Add(DelayAction.Allocate(1, () => {  }));
            spawnNode.Add(DelayAction.Allocate(1, () => { stopwatch.Stop(); }));
            
            while (!spawnNode.Execute(Time.deltaTime))
            {
                yield return null;
            }

            Assert.Less(stopwatch.ElapsedMilliseconds, 1100);
        }
        
        [UnityTest]
        public IEnumerator SequenceNodeTest()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var sequenceNode = new SequenceNode();
            sequenceNode.Append(DelayAction.Allocate(1, () => {  }));
            sequenceNode.Append(DelayAction.Allocate(1, () => {  }));
            sequenceNode.Append(DelayAction.Allocate(1, () => { stopwatch.Stop(); }));
            
            while (!sequenceNode.Execute(Time.deltaTime))
            {
                yield return null;
            }

            Assert.Greater(stopwatch.ElapsedMilliseconds, 2900);
        }
    }
}