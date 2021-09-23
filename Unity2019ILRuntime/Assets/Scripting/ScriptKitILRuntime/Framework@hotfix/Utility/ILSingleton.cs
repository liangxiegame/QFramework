namespace QFramework
{
    public interface ILHasInit
    {
        void OnInit();
    }
    
    public class ILSingleton<T> where T : ILHasInit, new()
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new T();
                    mInstance.OnInit();
                }

                return mInstance;
            }
        }
    }
}