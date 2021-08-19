using UnityEngine;

namespace QFramework.Example
{
    public class MVCWithIOCModel
    {
        public readonly Property<int> Count;

        public MVCWithIOCModel()
        {
            var count = PlayerPrefs.GetInt("COUNT", 0);
            Count = new Property<int>(count);
            Count.Bind(value => PlayerPrefs.SetInt("COUNT", value));
        }
    }
}