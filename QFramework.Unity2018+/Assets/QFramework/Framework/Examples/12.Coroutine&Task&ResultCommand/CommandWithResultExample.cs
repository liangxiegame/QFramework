using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace QFramework.Example
{
    public class CommandWithResultExample : MonoBehaviour
    {
        public class ExampleArchitecture : Architecture<ExampleArchitecture>
        {
            protected override void Init()
            {
                
            }
        }
        
        public class SimpleResultCommand : AbstractCommand<string>
        {
            protected override string OnExecute()
            {
                return "Hello Command With Result";
            }
        }

        public class ACoroutineCommand : AbstractCommand<IEnumerator>
        {
            protected override IEnumerator OnExecute()
            {
                Debug.Log("ACoroutineCommandStart:" + Time.time);
                yield return new WaitForSeconds(1.0f);
                Debug.Log("ACoroutineCommandFinish:" + Time.time);
            }
        }
        
        public class TaskACommand : AbstractCommand<Task<bool>>
        {
            protected override async Task<bool> OnExecute()
            {
                await Task.Delay(TimeSpan.FromSeconds(2.0f));
                return true;
            }
        }

        void Start()
        {
            Debug.Log(ExampleArchitecture.Interface.SendCommand(new SimpleResultCommand()));
            StartCoroutine(ExampleArchitecture.Interface.SendCommand(new ACoroutineCommand()));
            SendTaskACommand();
        }

        async void SendTaskACommand()
        {
            
            var result = await ExampleArchitecture.Interface.SendCommand(new TaskACommand());
            Debug.Log("ATaskCommandResult:" + result + " time:" + Time.time);
        }
    }
}
