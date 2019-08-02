using System;
using QFramework.GraphDesigner;
using Invert.Data;
using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IGraphItem : IItem, IDataRecord
    {
        string Label { get; }
        bool IsValid { get; }


    }

     
}