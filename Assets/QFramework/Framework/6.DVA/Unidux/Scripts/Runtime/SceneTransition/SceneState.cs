using System;
using System.Collections.Generic;
using System.Linq;
using Unidux.Util;

namespace Unidux.SceneTransition
{
    [Serializable]
    public class SceneState<TScene> : StateElement, ICloneable where TScene : struct
    {
        public readonly IDictionary<TScene, bool> ActiveMap = new Dictionary<TScene, bool>();

        public SceneState()
        {
        }

        public SceneState(SceneState<TScene> state)
        {
            this.ActiveMap.Clear();

            foreach (var key in state.ActiveMap.Keys)
            {
                this.ActiveMap[key] = state.ActiveMap[key];
            }
        }

        public bool IsReady
        {
            get { return this.ActiveMap.Values.Any(enabled => enabled); }
        }

        public object Clone()
        {
            return new SceneState<TScene>(this);
        }

        public IEnumerable<TScene> Removals(IEnumerable<TScene> activeScenes)
        {
            var removals = this.ActiveMap
                .Where(entry => !entry.Value)
                .Select(entry => entry.Key);
            return removals.Intersect<TScene>(activeScenes);
        }

        public IEnumerable<TScene> Additionals(IEnumerable<TScene> activeScenes)
        {
            var additionals = this.ActiveMap
                .Where(entry => entry.Value)
                .Select(entry => entry.Key);

            return additionals.Except(activeScenes);
        }

        public bool NeedsAdjust(
            IEnumerable<TScene> allPageScenes,
            IEnumerable<TScene> pageScenes
        )
        {
            var required = new Dictionary<TScene, bool>();

            foreach (var scene in allPageScenes)
            {
                required[scene] = false;
            }

            foreach (var scene in pageScenes)
            {
                required[scene] = true;
            }

            foreach (var scene in allPageScenes)
            {
                if (required[scene] != this.ActiveMap[scene]) return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is SceneState<TScene>)
            {
                var target = (SceneState<TScene>) obj;
                return this.ActiveMap.SequenceEqual(target.ActiveMap);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ActiveMap.GetHashCode();
        }
    }
}