using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.List
{
    public class ListRenderer : MonoBehaviour
    {
        public GameObject ListItem;

        void OnEnable()
        {
            Unidux.Subject
                .TakeUntilDisable(this)
                .Where(state => state.List.IsStateChanged)
                .StartWith(Unidux.State)
                .Subscribe(state => this.Render(state))
                .AddTo(this)
                ;
        }

        void Render(State state)
        {
            // remove all child
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }

            // add child
            foreach (string text in state.List.Texts)
            {
                var item = Instantiate(ListItem);
                item.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>(), false);
                item.GetComponent<Text>().text = text;
            }
        }
    }
}