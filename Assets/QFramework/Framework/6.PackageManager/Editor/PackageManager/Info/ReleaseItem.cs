/****************************************************************************
 * Copyright (c) 2018.7 liangxie
 ****************************************************************************/

using System;

namespace QFramework
{
    [Serializable]
    public class ReleaseItem
    {
        public ReleaseItem()
        {

        }

        public ReleaseItem(string version, string content, string author, string date)
        {
            this.version = version;
            this.content = content;
            this.author = author;
            this.date = date;
        }

        public string version = "";
        public string content = "";
        public string author  = "";
        public string date    = "";
    }
}