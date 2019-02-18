using System;
using UnityEngine;
using System.Collections;
using QFramework;
using uFrame.Kernel;

namespace uFrame.Kernel
{
    public interface ISystemLoader
    {

        void Load();

        IEnumerator LoadAsync();

        IQFrameworkContainer Container { get; set; }

        IEventAggregator EventAggregator { get; set; }

    }

    public partial class SystemLoader : MonoBehaviour, ISystemLoader
    {
        public virtual void Load()
        {

        }

        public virtual IEnumerator LoadAsync()
        {
            yield break;
        }

        public IQFrameworkContainer Container { get; set; }

        public IEventAggregator EventAggregator { get; set; }
    }

}