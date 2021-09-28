using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public interface IStorage : ILUtility
    {
        int LoadInt(string key, int initialValue = 0);
        void SaveInt(string key, int value);
    }

    public class PlayerPrefsStorage : IStorage
    {
        public int LoadInt(string key, int initialValue = 0)
        {
            return PlayerPrefs.GetInt(key, initialValue);
        }

        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
    }
}