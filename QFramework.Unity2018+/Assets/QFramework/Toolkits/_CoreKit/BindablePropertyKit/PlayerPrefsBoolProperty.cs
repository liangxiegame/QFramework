/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public class PlayerPrefsBooleanProperty : BindableProperty<bool>
    {
        public PlayerPrefsBooleanProperty(string saveKey, bool defaultValue = false)
        {
            mValue = PlayerPrefs.GetInt(saveKey, defaultValue ? 1 : 0) == 1;

            this.Register(value => PlayerPrefs.SetInt(saveKey, value ? 1 : 0));
        }
    }
}