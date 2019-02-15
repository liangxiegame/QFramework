using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invert.Data
{
    public interface IRepository
    {
        void Add(IDataRecord obj);
        TObjectType Create<TObjectType>() where TObjectType : class,IDataRecord, new();
        TObjectType GetSingle<TObjectType>(string identifier) where TObjectType : class,IDataRecord, new();
        
        TObjectType GetById<TObjectType>(string identifier);
        IEnumerable<TObjectType> All<TObjectType>() where TObjectType : class,IDataRecord;
        void Commit();

        IEnumerable<TObjectType> AllOf<TObjectType>() where TObjectType : IDataRecord;
        IEnumerable AllOf(Type o);
        void RemoveAll<TObjectType>();
        void Remove(IDataRecord record);
        void Reset();
        void RemoveAll<TObjectType>(Predicate<TObjectType> expression) where TObjectType : class;
        void MarkDirty(IDataRecord graphData);

        string GetUniqueName(string s);
        void Signal<TEventType>(Action<TEventType> perform);
        void AddListener<TEventType>(TEventType instance);

        TObjectType GetSingleLazy<TObjectType>(ref string keyProperty, Action<TObjectType> created) where TObjectType : class,IDataRecord, new();
        TObjectType GetSingle<TObjectType>() where TObjectType : class, IDataRecord, new();

        TObjectType GetSingleLazy<TObjectType>( Action<TObjectType> created = null) where TObjectType : class, IDataRecord, new();
    }





 
}