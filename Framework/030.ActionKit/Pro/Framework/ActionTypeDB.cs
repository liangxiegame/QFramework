using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class ActionTypeDB
    {
        private static Dictionary<string, Type> mDB = null;

        static Dictionary<string, Type> MakeSureDB()
        {
            if (mDB == null)
            {
                Search();
            }

            return mDB;
        }
        
        public static IEnumerable<Type> GetAll()
        {
            return MakeSureDB().Values;
        }

        public static Type GetTypeByFullName(string fullName)
        {
            return MakeSureDB()[fullName];
        }
        
        
        static void Search()
        {
            // 准备所有的 Action
            mDB = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(IAction).IsAssignableFrom(t))
                .ToDictionary(t => t.FullName);
        }
    }
}