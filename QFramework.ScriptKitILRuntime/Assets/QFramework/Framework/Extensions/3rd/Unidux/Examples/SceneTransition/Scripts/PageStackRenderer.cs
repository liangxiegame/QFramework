using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.SceneTransition
{
    [RequireComponent(typeof(Text))]
    public class PageStackRenderer : MonoBehaviour
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
            var message = new StringBuilder();

            foreach (var page in state.Page.Stack)
            {
                message.Append(page.Page.ToString());
                message.Append(">");
            }
            
            this.GetComponent<Text>().text = message.ToString();
        }
    }
}