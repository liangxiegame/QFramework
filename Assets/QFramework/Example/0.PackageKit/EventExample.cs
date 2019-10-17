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

        // Use this for initialization
        void Start()
        {
            SimpleEventSystem.GetEvent<A>()
                .Subscribe(a => Log.I("a message"));
            
            SimpleEventSystem.GetEvent<B>()
                .Subscribe(b => Log.I("b message"));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                SimpleEventSystem.Publish(new A());
                
            }

            if (Input.GetMouseButtonUp(1))
            {
                SimpleEventSystem.Publish(new B());
            }
        }
    }
}