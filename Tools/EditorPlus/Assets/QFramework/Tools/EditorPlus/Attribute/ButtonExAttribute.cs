// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Date: 2018-04-12 14:19
//  ****************************************************************************/

using System;

namespace UnityEngine
{
    public enum ShowIfRunTime
    {
        All,
        Playing,
        Editing,
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonExAttribute : Attribute
    {
        public readonly string txtButtonName;
        public ShowIfRunTime showIfRunTime;

        public ButtonExAttribute(string txtButtonName = null)
        {
            this.txtButtonName = txtButtonName;
        }
    }
}