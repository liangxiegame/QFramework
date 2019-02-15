using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFramework.GraphDesigner;
using Invert.Data;
using QFramework.GraphDesigner.Unity.WindowsPlugin;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.ViewModels
{
    public class PackageManagerWindow : WindowViewModel
    {
        private IRepository _repository;

        public IRepository Repository
        {
            get { return _repository ?? (_repository = InvertApplication.Container.Resolve<IRepository>()); }
            set { _repository = value; }
        }

    }
}
