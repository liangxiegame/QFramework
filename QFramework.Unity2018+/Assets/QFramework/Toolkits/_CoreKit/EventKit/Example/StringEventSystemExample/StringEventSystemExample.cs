using UnityEngine;

namespace QFramework.Example
{
    public class StringEventSystemExample : MonoBehaviour
    {
        void Start()
        {
            StringEventSystem.Global.Register("TEST_ONE", () =>
            {
                Debug.Log("TEST_ONE");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 事件 + 参数
            StringEventSystem.Global.Register<int>("TEST_TWO", (count) =>
            {
                Debug.Log("TEST_TWO:" + count);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StringEventSystem.Global.Send("TEST_ONE");
                StringEventSystem.Global.Send("TEST_TWO",10);
                
            }
        }
    }
}

// 输出结果
// 点击鼠标左键
// TEST_ONE
// TEST_TWO:10
