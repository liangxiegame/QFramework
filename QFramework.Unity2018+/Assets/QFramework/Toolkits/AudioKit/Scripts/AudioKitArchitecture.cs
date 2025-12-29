/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    internal class Architecture : Architecture<Architecture>
    {
        internal static PlaySoundChannelSystem PlaySoundChannelSystem { get; } = new PlaySoundChannelSystem();
        internal static AudioKitSettingsModel SettingsModel { get; } = new AudioKitSettingsModel();
        internal static PlayingSoundPoolModel PlayingSoundPoolModel { get; } = new PlayingSoundPoolModel();

        internal static AudioLoaderPoolModel LoaderPoolModel { get; } = new AudioLoaderPoolModel();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            InitArchitecture();
            LogKit.I("AudioKit.Architecture Inited");
        }

        protected override void Init()
        {
            RegisterSystem(new ConsoleModuleSystem());
            RegisterSystem(PlaySoundChannelSystem);
            
            RegisterModel(PlayingSoundPoolModel);
            RegisterModel(LoaderPoolModel);
            RegisterModel(SettingsModel);
        }
    }
}