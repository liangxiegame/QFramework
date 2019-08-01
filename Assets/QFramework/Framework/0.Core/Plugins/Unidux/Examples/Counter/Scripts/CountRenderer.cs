using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Counter
{
    [RequireComponent(typeof(Text))]
    public class CountRenderer : MonoBehaviour
    {
        void Start()
        {
            var text = this.GetComponent<Text>();

            Unidux.Subject
                .StartWith(Unidux.State)
                .Subscribe(state => text.text = state.Count.ToString())
                .AddTo(this)
                ;
        }
    }
}