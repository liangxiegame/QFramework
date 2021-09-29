using System;
using System.Collections.Generic;
using System.Linq;

namespace Unidux.SceneTransition
{
    [Serializable]
    public class PageState<TPage> : StateElement, ICloneable where TPage : struct
    {
        // TODO: change typw from IList to Stack 
        public readonly IList<IPageEntity<TPage>> Stack = new List<IPageEntity<TPage>>();

        public IPageEntity<TPage> Current
        {
            get { return this.Stack.Count > 0 ? this.Stack.Last() : null; }
        }

        public string Name
        {
            get { return this.Stack.Count > 0 ? this.Current.Page.ToString() : "-"; }
        }

        public IPageData Data
        {
            get { return this.Current != null ? this.Current.Data : null; }
        }

        public TData GetData<TData>() where TData : IPageData
        {
            return (TData) this.Current.Data;
        }

        public bool IsReady
        {
            get { return this.Stack.Count > 0; }
        }

        public PageState()
        {
        }

        public PageState(PageState<TPage> state)
        {
            foreach (var page in state.Stack)
            {
                this.Stack.Add(page);
            }

            this.SetStateChanged(state.IsStateChanged);
        }

        public object Clone()
        {
            return new PageState<TPage>(this);
        }
    }
}