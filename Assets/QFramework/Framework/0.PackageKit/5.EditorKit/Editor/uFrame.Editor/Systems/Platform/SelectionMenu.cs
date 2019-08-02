using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class SelectionMenu
    {
        private List<IItem> _items;

        public List<IItem> Items
        {
            get { return _items ?? (_items = new List<IItem>()); }
            set { _items = value; }
        }

        //TODO Little guide for Micah
        /// <summary>
        /// You can pass any IItem here. 
        /// SelectionMenuItem will invoke it's action when clicked
        /// Anything else will do no action when clicked
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        public void AddItem(IItem item, SelectionMenuCategory category = null)
        {
            var foundCategory = category == null ? null : Items.FirstOrDefault(s => s == category) as SelectionMenuCategory;
            if (foundCategory != null) foundCategory.Add(item);
            else Items.Add(item);
        }

        public void ConvertAndAdd(IEnumerable<IItem> regularItems, Action<IItem> itemAction, bool categoriesByGroups = true)
        {
            var items = regularItems.ToArray();

            var categories = items.Where(_ => !string.IsNullOrEmpty(_.Group)).Select(_ => _.Group).Distinct().Select(_ => new SelectionMenuCategory()
            {
                Title = _
            });

            foreach (var category in categories)
            {
                this.AddItem(category);
                var category1 = category;
                foreach (var item in items.Where(_ => _.Group == category1.Title))
                {
                    var item1 = item;
                    this.AddItem(new SelectionMenuItem(item, ()=>itemAction(item1)), category);
                }
            }

            foreach (var item in items.Where(_ => string.IsNullOrEmpty(_.Group)))
            {
                var item1 = item;
                this.AddItem(new SelectionMenuItem(item1, ()=>itemAction(item1)));
            }

        }


        public SelectionMenuCategory CreateCategoryIfNotExist(params string[] path)
        {
            if (path.Length < 1)
            {
                throw new Exception("Path must have at least one value");
            }
            List<IItem> items = Items;
            SelectionMenuCategory pathMenuItem = null;
            foreach (var pathItem in path)
            {
                pathMenuItem = items.OfType<SelectionMenuCategory>().FirstOrDefault(p => p.Title == pathItem);
                if (pathMenuItem == null)
                {
                    pathMenuItem = new SelectionMenuCategory()
                    {
                        Title = pathItem,
                    };
                    items.Add(pathMenuItem);
                }
                items = pathMenuItem.ChildItems;
            }
            return pathMenuItem;
        }


        protected IItem FindItem(string name)
        {
            return FindItem(this.Items, name);
        }

        protected IItem FindItem(IEnumerable<IItem> items, string name, bool recursive = true)
        {
            foreach (var item in items)
            {
                if (item.Title == name)
                    return item;
                if (recursive)
                {
                    var treeItem = item as ITreeItem;
                    if (treeItem != null)
                    {
                        var result = FindItem(treeItem.Children, name);
                        if (result != null)
                            return result;
                    }
                }
                
            }
            return null;
        }
        
    }
}