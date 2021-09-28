

namespace QFramework
{
    [System.Serializable]
    public class ABMD5  
    {
        public string ABName;
        public float ABSize;
        public string MD5;

        public ABMD5(string aBName, float aBSize, string mD5)
        {
            ABName = aBName;
            ABSize = aBSize;
            MD5 = mD5;
        }
    }
}
