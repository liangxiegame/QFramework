/****************************************************************************
 * Copyright (c) 2018.7 liangxie
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QF
{
    [Serializable]
    public class Readme
    {
        public List<ReleaseItem> items;

        public ReleaseItem GetItem(string version)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            return items.First(s => s.version == version);
        }

        public void AddReleaseNote(ReleaseItem pluginReadme)
        {
            if (items == null)
            {
                items = new List<ReleaseItem> {pluginReadme};
            }
            else
            {

                bool exist = false;
                foreach (var item in items)
                {
                    if (item.version == pluginReadme.version)
                    {
                        item.content = pluginReadme.content;
                        item.author = pluginReadme.author;
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    items.Add(pluginReadme);
                }

            }
        }

    }
}