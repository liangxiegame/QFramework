using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF.GraphDesigner.Unity.WindowsPlugin;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.ViewModels
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
