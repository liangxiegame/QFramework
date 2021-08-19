using UnityEngine;

namespace QFramework.Example.ActionKit
{
    public class SayHelloWorld : ActionKitAction
    {
        protected override void OnBegin()
        {
            Debug.Log("Hello World !");

            // 结束此 Action
            Finish();
        }
    }

    public class HelloWorldActionExample : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            this.ExecuteNode(new SayHelloWorld());
        }
    }
}