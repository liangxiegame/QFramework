using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Middlewares
{
    public class MiddlewaresBehaviour : MonoBehaviour
    {
        public Button DispatchButton;
        public Button CrashButton;
        public Text MessageText;

        void Start()
        {
            // Thunk
            this.DispatchButton.OnClickAsObservable()
                .Select(_ => Time.fixedTime)
                .Subscribe(time =>
                {
                    Unidux.Dispatch(new MiddlewareDuck.Action(time))
                        .AsThunkObservable()
                        .Delay(TimeSpan.FromSeconds(1))
                        .Select(_ => Time.fixedTime)
                        .Subscribe(endTime =>
                        {
                            var startTime = Unidux.State.StartTime;
                            this.MessageText.text = string.Format("start:{0} => end:{1}", startTime, endTime);
                        });
                });
            
            // Crash
            this.CrashButton.OnClickAsObservable()
                .Subscribe(time =>
                {
                    Unidux.Dispatch(new MiddlewareDuck.Action(-1))
                        .AsThunkObservable()
                        .Subscribe();
                });
        }
    }
}