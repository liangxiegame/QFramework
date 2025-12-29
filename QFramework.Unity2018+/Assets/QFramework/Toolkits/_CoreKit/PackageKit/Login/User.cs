#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    internal static class User
    {
        public static BindableProperty<string> Username = new BindableProperty<string>(LoadString("username"));
        public static BindableProperty<string> Password = new BindableProperty<string>(LoadString("password"));
        public static BindableProperty<string> Token = new BindableProperty<string>(LoadString("token"));

        public static bool Logined
        {
            get
            {
                return !string.IsNullOrEmpty(Token.Value) &&
                       !string.IsNullOrEmpty(Username.Value) &&
                       !string.IsNullOrEmpty(Password.Value);
            }
        }


        public static void Save()
        {
            Username.SaveString("username");
            Password.SaveString("password");
            Token.SaveString("token");
        }

        public static void Clear()
        {
            Username.Value = string.Empty;
            Password.Value = string.Empty;
            Token.Value = string.Empty;
            Save();
        }

        public static void SaveString(this BindableProperty<string> selfProperty, string key)
        {
            EditorPrefs.SetString(key, selfProperty.Value);
        }


        public static string LoadString(string key)
        {
            return EditorPrefs.GetString(key, string.Empty);
        }
    }
}
#endif