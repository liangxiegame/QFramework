using UnityEngine;

namespace QFramework
{
    public class LocaleKitChangeLanguageAction : MonoBehaviour
    {
        public Language Language;

        public void Execute()
        {
            LocaleKit.ChangeLanguage(Language);
        }
    }
}
