using System;
using System.Collections.Generic;
using System.Linq;
using Unidux.Util;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Unidux.Experimental.Editor
{
    public partial class UniduxPanelStateTab
    {
        public delegate TResult Func6<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3,
            T4 arg4,
            T5 arg5);

        private Vector2 _scrollPosition = Vector2.zero;
        private Dictionary<string, bool> _foldingMap = new Dictionary<string, bool>();
        private Dictionary<string, int> _pageMap = new Dictionary<string, int>();
        private object _newState = null;
        private ISubject<object> _stateSubject;
        private const int PerPage = 10;

        public void Render(IStoreAccessor _store)
        {
            if (_store == null)
            {
                EditorGUILayout.HelpBox("Please Set IStoreAccessor", MessageType.Warning);
                return;
            }

            // scrollview of state
            {
                this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);

                if (!(_store.StoreObject.ObjectState is ICloneable))
                {
                    EditorGUILayout.HelpBox("Please inplement ICloneable on State", MessageType.Warning);
                    return;
                }
                
                var state = _store.StoreObject.ObjectState;
                var names = new List<string>();
                var type = state.GetType();

                if (!state.Equals(this._newState))
                {
                    this._newState = ((ICloneable)state).Clone();
                }

                var dirty = this.RenderObject(names, state.GetType().Name, this._newState, type, _ => { });
                EditorGUILayout.EndScrollView();

                // XXX: it might be slow and should be updated less frequency.
                if (dirty)
                {
                    _store.StoreObject.ObjectState = this._newState;
                    this._newState = null;
                }
            }
        }

        Func6<IList<string>, string, object, Type, Action<object>, bool> SelectObjectRenderer(Type type, object element)
        {
            Func6<IList<string>, string, object, Type, Action<object>, bool> render;

            if (type.IsValueType)
            {
                render = this.SelectValueRender(type, element);
            }
            else
            {
                render = this.SelectClassRender(type, element);
            }

            return render;
        }

        bool RenderObject(
            IList<string> rootNames,
            string name,
            object element,
            Type type,
            Action<object> setter
        )
        {
            return this.SelectObjectRenderer(type, element)(rootNames, name, element, type, setter);
        }

        int RenderPager(
            string pagerName,
            string foldingKey,
            System.Collections.ICollection collection,
            Action<object, int> renderValueWithIndex,
            bool disable = false
        )
        {
            var listSize = collection.Count;
            var page = this._pageMap.GetOrDefault(foldingKey, 0);
            var lastPage = (listSize - 1) / PerPage;
            page = this.RenderPagerHeader(pagerName, page, lastPage);
            this._pageMap[foldingKey] = page;

            var startIndex = page * PerPage;
            var endIndex = Math.Min((page + 1) * PerPage, listSize);

            if (disable)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            var i = 0;
            foreach (var element in collection)
            {
                if (i >= startIndex && i < endIndex)
                {
                    renderValueWithIndex(element, i);
                }

                i++;
            }

            if (disable)
            {
                EditorGUI.EndDisabledGroup();
            }

            return page;
        }

        int RenderPagerHeader(
            string pagerName,
            int page,
            int lastPage
        )
        {
            if (lastPage < 1)
            {
                return page;
            }
            
            bool hasPrev = page > 0;
            bool hasNext = lastPage > page;

            EditorGUILayout.BeginHorizontal();

            {
                EditorGUI.BeginDisabledGroup(!hasPrev);
                if (GUILayout.Button("<<<") && hasPrev)
                {
                    page -= 100;
                }
                if (GUILayout.Button("<<") && hasPrev)
                {
                    page -= 10;
                }
                if (GUILayout.Button("<") && hasPrev)
                {
                    page--;
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.LabelField(pagerName, GUILayout.MinWidth(30));
            page = EditorGUILayout.IntField(page, GUILayout.MinWidth(30));
            EditorGUILayout.LabelField("/" + lastPage, GUILayout.MinWidth(30));

            {
                EditorGUI.BeginDisabledGroup(!hasNext);
                if (GUILayout.Button(">") && hasNext)
                {
                    page++;
                }
                if (GUILayout.Button(">>") && hasNext)
                {
                    page += 10;
                }
                if (GUILayout.Button(">>>") && hasNext)
                {
                    page += 100;
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();

            page = page < 0 ? 0 : page;
            page = page > lastPage ? lastPage : page;

            return page;
        }

        string GetFoldingName(ICollection<string> collection, string name)
        {
            return name;
        }

        string GetFoldingKey(IEnumerable<string> rootNames)
        {
            return string.Join(".", rootNames.ToArray());
        }
    }
}