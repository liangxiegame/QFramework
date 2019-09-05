using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace QF.GraphDesigner.Unity
{
    public class ThreadingUtils
    {

        public static DispatcherResult DispatchOnMainThread(System.Action x)
        {
            var d = new DispatcherResult();

            EditorApplication.delayCall += () =>
            {
                x();
                d.Done = true;
            };

            return d;
        }

        public static DispatcherResult WaitOnMainThread(Func<bool> selector)
        {
            var d = new DispatcherResult();

            EditorApplication.CallbackFunction callbackFunction = null;
            callbackFunction = () =>
            {
                if (selector())
                {
                    d.Done = true;
                    EditorApplication.update -= callbackFunction;
                }
            };
            EditorApplication.update += callbackFunction;

            return d;
        }



        public class DispatcherResult
        {
            public bool Done { get; set; }
        }
    }
}
