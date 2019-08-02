using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Invert.uFrame.Editor.ViewModels;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ConnectorInfo
    {
        private ConnectorViewModel[] _inputs;
        private ConnectorViewModel[] _outputs;

        public ConnectorInfo(ConnectorViewModel[] allConnectors, DiagramViewModel viewModel)
        {
            AllConnectors = allConnectors;
            DiagramData = viewModel.GraphData;
            DiagramViewModel = viewModel;
       
        }

        public ConnectorViewModel[] AllConnectors
        {
            get;
            set;
        }
        public DiagramViewModel DiagramViewModel { get; set; }
        public IGraphData DiagramData { get; set; }


        public ConnectorViewModel[] Inputs
        {
            get { return _inputs ?? (_inputs = AllConnectors.Where(p => p.Direction == ConnectorDirection.Input).ToArray()); }

        }
        public ConnectorViewModel[] Outputs
        {
            get { return _outputs ?? (_outputs = AllConnectors.Where(p => p.Direction == ConnectorDirection.Output).ToArray()); }
        }

        public IEnumerable<ConnectorViewModel> InputsWith<TData>()
        {
            return Inputs.Where(p => p.DataObject is TData);
        }
        public IEnumerable<ConnectorViewModel> OutputsWith<TData>()
        {
            return Outputs.Where(p => p.DataObject is TData);
        }

        public IEnumerable<ConnectionViewModel> ConnectionsByData<TSource, TTarget>(DefaultConnectionStrategy<TSource, TTarget> strategy)
            where TSource : IGraphItem
            where TTarget : IGraphItem
        {
            var alreadyConnected = new List<string>();
            foreach (var output in OutputsWith<TSource>())
            {
                foreach (var input in InputsWith<TTarget>())
                {

                    var tempId = output.DataObject.GetHashCode().ToString() + input.DataObject.GetHashCode();
                    if (alreadyConnected.Contains(tempId)) continue;

                    if (strategy.IsConnected(DiagramViewModel.CurrentRepository, (TSource)output.DataObject, (TTarget)input.DataObject))
                    {
                        yield return new ConnectionViewModel(DiagramViewModel)
                        {
                            IsStateLink = strategy.IsStateLink,
                            Color = GetColor((TTarget)input.DataObject),
                            ConnectorA = output,
                            ConnectorB = input,
                            Remove = strategy.Remove,
                            Apply = strategy.Apply
                        };
                        alreadyConnected.Add(tempId);
                    }


                }
            }
        }

        public Color GetColor(IGraphItem dataObject)
        {
            var item = dataObject as IDiagramNodeItem;
            if (item != null)
            {
                var node = item.Node as GenericNode;
                if (node != null)
                {
                    var color = node.Config.GetColor(node);
                    switch (color)
                    {
                        case NodeColor.Black:
                            return Color.black;
                        case NodeColor.Blue:
                            return new Color(0.25f, 0.25f, 0.65f);
                        case NodeColor.DarkDarkGray:
                            return new Color(0.25f, 0.25f, 0.25f);
                        case NodeColor.DarkGray:
                            return new Color(0.45f, 0.45f, 0.45f);
                        case NodeColor.Gray:
                            return new Color(0.65f, 0.65f, 0.65f);
                        case NodeColor.Green:
                            return new Color(0.00f, 1f, 0f);
                        case NodeColor.LightGray:
                            return new Color(0.75f, 0.75f, 0.75f);
                        case NodeColor.Orange:
                            return new Color(0.059f,0.98f,0.314f);
                        case NodeColor.Pink:
                            return new Color(0.059f, 0.965f, 0.608f);
                        case NodeColor.Purple:
                            return new Color(0.02f, 0.318f, 0.659f);
                        case NodeColor.Red:
                            return new Color(1f,0f,0f);
                        case NodeColor.Yellow:
                            return new Color(1f,0.8f,0f);
                        case NodeColor.YellowGreen:
                            return new Color(0.604f, 0.804f, 0.196f);

                    }

                }
            }
            return Color.white;
        }
    }
}