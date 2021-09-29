using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.SimpleHttp
{
    [RequireComponent(typeof(Button))]
    public class SimpleHttpGetDispatcher : MonoBehaviour
    {
        public string Url = "http://date.jsontest.com/";
        
        void Start()
        {
            this.GetComponent<Button>()
                .OnClickAsObservable()
                .Select(_ => SimpleHttpDuck.ActionCreator.Get(this.Url))
                .Subscribe(action => Unidux.Dispatch(action))
                .AddTo(this)
                ;
        }
    }
}