using UniRx;
using UnityEngine;

namespace QF.PackageKit.Example {
    /// <summary>
    /// 定义两个消息
    /// </summary>
    public class A { }

    public class B {
        public string Key;
    }

    public class EventExample : MonoBehaviour {

        // Use this for initialization
        void Start () {
            SimpleEventSystem.GetEvent<A> ()
                .Subscribe (a => Log.I ("a message"))
                .AddTo (this); // 可以与 MonoBehavour 绑定生命周期，即 MonoBehaviour 销毁时，自动取消订阅。

            SimpleEventSystem.GetEvent<B> ()
                .Subscribe (b => {
                    if (b.Key == "say") {
                        Log.I ("b message");
                    }
                });
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetMouseButtonUp (0)) {
                SimpleEventSystem.Publish (new A ());

            }

            if (Input.GetMouseButtonUp (1)) {
                SimpleEventSystem.Publish (new B () { Key = "say" });
            }
        }
    }
}