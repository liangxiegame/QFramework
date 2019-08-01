using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unidux.Example.Todo
{
    public class BaseListRenderer<TRenderer, TRenderValue> : MonoBehaviour
        where TRenderer : MonoBehaviour, ICellRenderer<TRenderValue>
    {
        private List<TRenderer> objectPool = new List<TRenderer>();

        private int _ReallocSize = 1;

        protected int ReallocSize
        {
            get { return this._ReallocSize; }
            set { this._ReallocSize = value; }
        }

        protected void InitObjects(Transform parentTransform, TRenderer prefab, int count)
        {
            if (objectPool.Count >= count)
            {
                return;
            }

            this.AllocObjects(parentTransform, prefab, count);
        }

        protected void AllocObjects(Transform parentTransform, TRenderer prefab, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.AllocObject(parentTransform, prefab);
            }
        }

        private TRenderer AllocObject(Transform parentTransform, TRenderer prefab)
        {
            var _object = GameObject.Instantiate(prefab) as TRenderer;
            _object.transform.SetParent(parentTransform, false);
            _object.gameObject.SetActive(false);
            objectPool.Add(_object);
            return _object;
        }

        public void Render(Transform parentTransform, TRenderer prefab, IEnumerable<TRenderValue> values)
        {
            foreach (var item in values.Select((value, index) => new {value, index}))
            {
                if (item.index >= objectPool.Count)
                {
                    this.AllocObjects(parentTransform, prefab, this.ReallocSize);
                }

                this.RenderCell(item.index, objectPool[item.index], item.value);
            }

            this.DisableUnusedObjects(values.Count());
        }

        protected virtual void RenderCell(int index, TRenderer renderer, TRenderValue value)
        {
            renderer.Render(index, value);
            renderer.gameObject.SetActive(true);
        }

        public void DisableUnusedObjects(int usedObjects)
        {
            for (int i = usedObjects; i < this.objectPool.Count; i++)
            {
                var renderer = this.objectPool[i];

                if (renderer.gameObject.activeSelf)
                {
                    renderer.gameObject.SetActive(false);
                }
            }
        }
    }
}