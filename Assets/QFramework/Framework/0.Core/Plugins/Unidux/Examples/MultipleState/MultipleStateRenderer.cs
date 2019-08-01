using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.MultipleState
{
    public class MultipleStateRenderer : MonoBehaviour
    {
        public Text Text;

        void Start()
        {
            Unidux.Subject
                .Subscribe(state => this.Render(state))
                .AddTo(this);
        }

        void Render(State state)
        {
            this.Text.text = new StringBuilder()
                .Append("IntValue:").Append(state.IntValue).Append("\n")
                .Append("UintValue:").Append(state.UintValue).Append("\n")
                .Append("LongValue:").Append(state.LongValue).Append("\n")
                .Append("UlongValue:").Append(state.UlongValue).Append("\n")
                .Append("FloatValue:").Append(state.FloatValue).Append("\n")
                .Append("DoubleValue:").Append(state.DoubleValue).Append("\n")
                .Append("BoolValue:").Append(state.BoolValue).Append("\n")
                .Append("StringValue:").Append(state.StringValue).Append("\n")
                .Append("FriendValue:").Append(state.AnimalFriend).Append("\n")
                .Append("ColorValue:").Append(state.ColorValue).Append("\n")
                .Append("Vector2Value:").Append(state.Vector2Value).Append("\n")
                .Append("Vector3Value:").Append(state.Vector3Value).Append("\n")
                .Append("Vector4Value:").Append(state.Vector4Value).Append("\n")
                .Append("ListValue:").Append(string.Join(",", state.ListValue.ToArray())).Append("\n")
                .Append("DictionaryValue:")
                .Append(string.Join(",", state.DictionaryValue.Select(p => p.Key + ":" + p.Value).ToArray()))
                .Append("\n")
                .Append("StringArray:").Append(string.Join(",", state.StringArray)).Append("\n")
                .Append("IdNameState:").Append(state.IdNameState).Append("\n")
                .Append("EmptyState:").Append(state.EmptyState).Append("\n")
                .ToString();
        }
    }
}