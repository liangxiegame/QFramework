using System;

namespace Unidux.SceneTransition
{
    [Serializable]
    public class PageEntity<TPage> : IPageEntity<TPage>, ICloneable
        where TPage : struct
    {
        public TPage Page { get; private set; }
        public IPageData Data { get; set; }

        public PageEntity(TPage page, IPageData data)
        {
            this.Page = page;
            this.Data = data;
        }

        public PageEntity(PageEntity<TPage> entity)
        {
            this.Page = entity.Page;
            this.Data = entity.Data;
        }

        public object Clone()
        {
            return new PageEntity<TPage>(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is PageEntity<TPage>)
            {
                var target = (PageEntity<TPage>) obj;
                return this.Page.Equals(target.Page) &&
                       (this.Data == null && target.Data == null || this.Data != null && this.Data.Equals(target.Data));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}