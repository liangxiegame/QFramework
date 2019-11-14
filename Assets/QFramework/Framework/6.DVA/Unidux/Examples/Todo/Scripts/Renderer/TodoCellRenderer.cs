using UnityEngine;
using UnityEngine.UI;

namespace Unidux.Example.Todo
{
    public class TodoCellRenderer : MonoBehaviour, ICellRenderer<Todo>
    {
        public Text Text;
        public Toggle Toggle;

        public void Render(int index, Todo item)
        {
            this.Text.text = item.Text;
            this.Toggle.isOn = item.Completed;
        }
    }
}