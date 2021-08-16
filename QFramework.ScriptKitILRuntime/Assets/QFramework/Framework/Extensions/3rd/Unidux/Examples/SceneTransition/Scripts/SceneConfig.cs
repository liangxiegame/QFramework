using System.Collections.Generic;
using Unidux.SceneTransition;

namespace Unidux.Example.SceneTransition
{
    public class SceneConfig : ISceneConfig<Scene, Page>
    {
        private IDictionary<Scene, int> _categoryMap;
        private IDictionary<Page, Scene[]> _pageMap;
        
        public IDictionary<Scene, int> CategoryMap
        {
            get
            {
                return this._categoryMap = this._categoryMap ?? new Dictionary<Scene, int>()
                {
                    {Scene.Base, SceneCategory.Permanent},
                    {Scene.Header, SceneCategory.Page},
                    {Scene.Footer, SceneCategory.Page},
                    {Scene.Content1, SceneCategory.Page},
                    {Scene.Content2, SceneCategory.Page},
                    {Scene.Modal, SceneCategory.Modal},
                };
            }
        }

        public IDictionary<Page, Scene[]> PageMap
        {
            get
            {
                return this._pageMap = this._pageMap ?? new Dictionary<Page, Scene[]>()
                {
                    {Page.Page1, new[] {Scene.Header, Scene.Footer, Scene.Content1}},
                    {Page.Page2, new[] {Scene.Header, Scene.Footer, Scene.Content2}},
                    {Page.Page3, new[] {Scene.Header, Scene.Content2}},
                };
            }
        }
    }
}