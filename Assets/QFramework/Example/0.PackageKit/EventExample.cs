using UnityEngine;
using UniRx;

namespace QF.PackageKit.Example
{
    public class A
    {
    }

    public class B
    {
    }

    public class EventExample : MonoBehaviour
    {
        EventAggregator mEventDispatcher = new EventAggregator();

        // Use this for initialization
        void Start()
        {
            mEventDispatcher.GetEvent<A>()
                .Subscribe(a => Log.I("a message"));
            
            mEventDispatcher.GetEvent<B>()
                .Subscribe(b => Log.I("b message"));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                mEventDispatcher.Publish(new A());
                
            }

            if (Input.GetMouseButtonUp(1))
            {
                mEventDispatcher.Publish(new B());
            }
        }
    }
}