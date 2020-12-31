using System;
using System.Collections.Generic;
using UnityEngine;

namespace Invert.Data
{
    public interface IRepository
    {
        void Add(IDataRecord obj);

        TObjectType GetById<TObjectType>(string identifier);
        IEnumerable<TObjectType> All<TObjectType>() where TObjectType : class,IDataRecord;

        IEnumerable<TObjectType> AllOf<TObjectType>() where TObjectType : IDataRecord;
        void Remove(IDataRecord record);

        void Signal<TEventType>(Action<TEventType> perform);
    }





 
}