using System.Collections.Generic;
using System.Linq;

namespace Unidux.SceneTransition
{
    public interface ISceneConfig<TScene, TPage>
    {
        IDictionary<TScene, int> CategoryMap { get; }

        IDictionary<TPage, TScene[]> PageMap { get; }
    }

    public static class ISceneConfigExtension
    {
        public static IEnumerable<TScene> GetPageScenes<TScene, TPage>(
            this ISceneConfig<TScene, TPage> config)
        {
            return config.CategoryMap
                .Where(entry => entry.Value == SceneCategory.Page)
                .Select(entry => entry.Key);
        }
    }
}