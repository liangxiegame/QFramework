using System;

namespace Unidux.Example.Todo
{
    [Serializable]
    public class Todo
    {
        public uint Id;
        public string Text;
        public bool Completed;

        public Todo(uint id = 0, string text = "", bool completed = false)
        {
            this.Id = id;
            this.Text = text;
            this.Completed = completed;
        }
    }
}