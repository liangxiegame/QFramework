using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    [Serializable]
    [Obsolete]
    public class OpenGraph
    {
        [SerializeField]
        private string _graphIdentifier;

        [SerializeField]
        private string _graphName;

        [SerializeField]
        private string[] _path;

        public string GraphIdentifier
        {
            get { return _graphIdentifier; }
            set { _graphIdentifier = value; }
        }

        public string GraphName
        {
            get { return _graphName; }
            set { _graphName = value; }
        }

        public string[] Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}