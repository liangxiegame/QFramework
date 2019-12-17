using System;
using QFramework.CodeGen;
using Invert.Data;
using UnityEngine;

namespace QFramework.CodeGen
{
    public interface IGraphItem : IItem, IDataRecord
    {
        string Label { get; }
        bool IsValid { get; }


    }

     
}