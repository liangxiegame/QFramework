namespace QF.GraphDesigner
{
    public interface IPlatformPreferences
    {
        bool GetBool(string name, bool def);
        string GetString(string name, string def);
        float GetFloat(string name, float def);
        float GetInt(string name, int def);
        void SetBool(string name, bool value);
        void SetString(string name, string value);
        void SetFloat(string name, float value);
        void SetInt(string name, int value);
        bool HasKey(string name);
        void DeleteKey(string name);
        void DeleteAll();
    }
}