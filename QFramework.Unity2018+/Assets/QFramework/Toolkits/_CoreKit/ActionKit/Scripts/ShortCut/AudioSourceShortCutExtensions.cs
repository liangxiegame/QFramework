/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public static class AudioSourceShortCutExtensions
    {
        public static IAction PlayAction(this AudioSource self)
        {
            return ActionKit.Custom<AudioSource>(api =>
            {
                api.OnStart(() =>
                {
                    api.Data = self;
                    self.Play();
                }).OnExecute(_ =>
                {
                    if (api.Data && !api.Data.isPlaying)
                    {
                        api.Finish();
                    }
                });
            });
        }
    }
}