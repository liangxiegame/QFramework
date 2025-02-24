/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class LocaleKit : Architecture<LocaleKit>
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            // 如果创建了 Config
            // if config is created
            if (Config)
            {
                var @_ = Interface;
            }
        }
        
        protected override void Init()
        {
            var languageIndex = LoadCommand("CURRENT_LANGUAGE_INDEX", 0);

            if (languageIndex >= Config.LanguageDefines.Count)
            {
                languageIndex = 0;
            }

            CurrentLanguage.Value = Config.LanguageDefines[languageIndex].Language;
        }

        public static BindableProperty<Language> CurrentLanguage = new BindableProperty<Language>(Language.English);

        public static Action<string, int> SaveCommand = (key, languageIndex) =>
            PlayerPrefs.SetInt(key, languageIndex);

        public static Func<string, int, int> LoadCommand = (key, defaultLanguageIndex) =>
            PlayerPrefs.GetInt(key, defaultLanguageIndex);
        

        private static LanguageDefineConfig mConfig;

        public static LanguageDefineConfig Config =>
            mConfig ? mConfig : mConfig = Resources.Load<LanguageDefineConfig>(nameof(LanguageDefineConfig));


        public static Language GetNextLanguage()
        {
            var languageIndex = Config.LanguageDefines.FindIndex(l => l.Language == CurrentLanguage.Value);
            languageIndex++;

            if (languageIndex >= Config.LanguageDefines.Count)
            {
                languageIndex = 0;
            }

            return Config.LanguageDefines[languageIndex].Language;
        }

        public static void ChangeLanguage(Language language)
        {
            CurrentLanguage.Value = language;
            SaveCommand("CURRENT_LANGUAGE_INDEX", Config.LanguageDefines.FindIndex(l => l.Language == language));
        }
    }
}