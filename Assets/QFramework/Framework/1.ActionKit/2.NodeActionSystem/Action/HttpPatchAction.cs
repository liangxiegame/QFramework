using System;
using System.Collections.Generic;

namespace QFramework
{
    public abstract class HttpPatchAction : NodeAction
    {
        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract string                     Url     { get; }
        
        protected abstract Dictionary<string,string> form { get; }
        
        protected abstract string Id { get; }

        private IDisposable mDisposable;
        
        
        
        protected override void OnBegin()
        {   
            API.HttpPatch(Url.FillFormat(Id), Headers, form, responseText =>
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