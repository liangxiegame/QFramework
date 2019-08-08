using System;
using QF.GraphDesigner;
using Invert.Data;
using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IGraphItem : IItem, IDataRecord
    {
        string Label { get; }
        bool IsValid { get; }


    }

     
}