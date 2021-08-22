using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.SceneTransition
{
    [RequireComponent(typeof(Text))]
    public class PageTitleRenderer : MonoBehaviour
    {
        void Start()
        {
            Unidux.Subject
                .Where(state => state.Page.IsStateChanged)
                .StartWith(Unidux.State)
                .Where(state => state.Page.IsReady)
                .Subscribe(this.Render)
                .AddTo(this);
        }

        void Render(State state)
        {
            this.GetComponent<Text>().text = state.Page.Current.Page.ToString();
        }
    }
}