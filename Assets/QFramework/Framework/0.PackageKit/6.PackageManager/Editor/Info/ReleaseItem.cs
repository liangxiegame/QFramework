/****************************************************************************
 * Copyright (c) 2018.7 liangxie
 ****************************************************************************/

using System;
using QF.Extensions;

namespace QF
{
    [Serializable]
    public class ReleaseItem
    {
        public ReleaseItem()
        {

        }

        public ReleaseItem(string version, string content, string author, DateTime date)
        {
            this.version = version;
            this.content = content;
            this.author = author;
            this.date = date.ToString("yyyy 年 MM 月 dd 日 HH:mm");
        }

        public string version = "";
        public string content = "";
        public string author  = "";
        public string date    = "";
        
        
        public int VersionNumber
        {
            get
            {
                if (version.IsNullOrEmpty())
                {
                    return 0;
                }
                
                var numbersStr = version.RemoveString("v").Split('.');

                var retNumber = numbersStr[2].ToInt();
                retNumber += numbersStr[1].ToInt() * 100;
                retNumber += numbersStr[0].ToInt() * 10000;
                
                return retNumber;
            }
        }
    }
}