using UnityEngine;

namespace QF
{
    public class Language
    {
        public static bool IsChinese
        {
            get
            {
                return Application.systemLanguage == SystemLanguage.Chinese ||
                       Application.systemLanguage == SystemLanguage.ChineseSimplified;
            }
        }
    }
}