using System;
using System.Collections.Generic;

namespace QFramework
{
    public abstract class HttpPostAction : NodeAction
    {

        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract string                     Url     { get; }

        protected abstract Dictionary<string, string> form { get; }

        private IDisposable mDisposable;



        protected override void OnBegin()
        {
            mDisposable = API.HttpPost(Url, Headers, form, responseText =>
            {
                mDisposable = null;
                OnResponse(responseText);
                Finish();
            });
        }

        protected abstract void OnResponse(string text);

        protected override void OnDispose()
        {
            if (mDisposable.IsNotNull())
            {
                mDisposable.Dispose();
                mDisposable = null;
            }

            base.OnDispose();
        }
    }
}