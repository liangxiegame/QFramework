using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.List
{
    [RequireComponent(typeof(Button))]
    public class ListItemDispatcher : MonoBehaviour
    {
        public string ItemName = "item";

        void Start()
        {
            this.GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => Unidux.Dispatch(List.ActionCreator.Add(this.ItemName)))
                .AddTo(this);
        }
    }
}