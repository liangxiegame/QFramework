using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.SimpleHttp
{
    [RequireComponent(typeof(Text))]
    public class SimpleHttpRenderer : MonoBehaviour
    {
        void Start()
        {
            Unidux.Subject
                .Where(state => state.IsStateChanged)
                .Subscribe(state => this.Render(state))
                .AddTo(this)
                ;
        }

        void Render(State state)
        {
            var message = new StringBuilder()
                    .Append("URL:").Append(state.Url).Append("\n")
                    .Append("StatusCode:").Append(state.StatusCode).Append("\n")
                    .Append("Body:").Append(state.Body).Append("\n")
                ;
            this.GetComponent<Text>().text = message.ToString();
        }
    }
}