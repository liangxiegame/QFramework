using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using QF;

namespace Invert.Data
{
    public class TypeDatabase : IRepository
    {
        public Dictionary<Type, List<object>> Listeners
        {
            get { return _listeners ?? (_listeners = new Dictionary<Type, List<object>>()); }
            set { _listeners = value; }
        
        }

        public void AddListener<TEventType>(TEventType instance)
        {
            if (!Listeners.ContainsKey(typeof (TEventType)))
            {
                Listeners.Add(typeof(TEventType), new List<object>());
            }
            Listeners[typeof(TEventType)].Add(instance);
        }
        public void Signal<TEventType>(Action<TEventType> perform)
        {
            foreach (var item in Repositories.ToArray())
            {
                if (typeof (TEventType).IsAssignableFrom(item.Key))
                {
                    foreach (var record in item.Value.GetAll().OfType<TEventType>())
                    {
                        var record1 = record;
                        perform(record1);
                    }
                }
            }
            if (Listeners.ContainsKey(typeof (TEventType)))
            {
                foreach (var listener in Listeners[typeof (TEventType)])
                {
                    var listener1 = listener;
                    perform((TEventType) listener1);
                }
            }
        }

        private Dictionary<Type, IDataRecordManager> _repositories;
        private Dictionary<Type, List<object>> _listeners;

        public Dictionary<Type, IDataRecordManager> Repositories
        {
            get { return _repositories ?? (_repositories = new Dictionary<Type, IDataRecordManager>()); }
            set { _repositories = value; }
        }

        public ITypeRepositoryFactory Factory { get; set; }

        public TypeDatabase(ITypeRepositoryFactory factory)
        {
            Factory = factory;
            Repositories = factory.CreateAllManagers(this)
                .ToDictionary(k => k.For, v => v);
        }

        public void Add(IDataRecord obj)
        {
            var repo = GetRepositoryFor(obj.GetType());
            repo.Add(obj);
            MarkDirty(obj);
        }

        public TObjectType Create<TObjectType>() where TObjectType : class, IDataRecord, new()
        {
            var obj = new TObjectType();
            var repo = GetRepositoryFor(typeof (TObjectType));
            repo.Add(obj);
            return obj;
        }
        public TObjectType GetSingle<TObjectType>() where TObjectType : class, IDataRecord, new()
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            return repo.GetAll().FirstOrDefault() as TObjectType;
        }
        public TObjectType GetSingle<TObjectType>(string identifier) where TObjectType : class, IDataRecord, new()
        {
            if (string.IsNullOrEmpty(identifier)) return default(TObjectType);
            var repo = GetRepositoryFor(typeof(TObjectType));
            return repo.GetSingle(identifier) as TObjectType;
        }

        public TObjectType GetSingleLazy<TObjectType>( ref string keyProperty, Action<TObjectType> created) where TObjectType : class, IDataRecord, new()
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            // Try and grab the item
            var item = string.IsNullOrEmpty(keyProperty) ? null : repo.GetSingle(keyProperty) as TObjectType;
            // If we found one return it
            if (item != null) return item;
            // Otherwise create it
            item = new TObjectType()
            {
                Identifier = Guid.NewGuid().ToString(),
                Repository = this
            };
            // Set the ForeignKey field
            keyProperty = item.Identifier;
            item.Changed = true;
            // Ensure its added to the repository
            repo.Add(item);
            if (created != null) 
                created(item);
            return item;
        }

        public TObjectType GetSingleLazy<TObjectType>( Action<TObjectType> created = null) where TObjectType : class, IDataRecord, new()
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            // Try and grab the item
            var item = this.GetSingle<TObjectType>();
            // If we found one return it
            if (item != null) return item;
            // Otherwise create it
            item = new TObjectType()
            {
                Identifier = Guid.NewGuid().ToString(),
                Repository = this
            };
            item.Changed = true;
            // Ensure its added to the repository
            repo.Add(item);
            if (created != null)
                created(item);
            return item;
        }


        public TObjectType GetById<TObjectType>(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) return default(TObjectType);
            foreach (var item in Repositories)
            {
                if (typeof (TObjectType).IsAssignableFrom(item.Key))
                {
                    var result= (TObjectType)item.Value.GetSingle(identifier);
                    if (result != null)
                        return result;
                }
            }
            return default(TObjectType);
        }

        public IDataRecordManager GetRepositoryFor(Type type)
        {
            IDataRecordManager repo;
            if (!Repositories.TryGetValue(type, out repo))
            {
                repo = Factory.CreateRepository(this, type);
                Repositories.Add(type, repo);
            }
            return repo;
        }

        public IEnumerable<TObjectType> All<TObjectType>() where TObjectType : class, IDataRecord
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            return repo.GetAll().Cast<TObjectType>();
        }

        public IEnumerable<TObjectType> AllOf<TObjectType>() where TObjectType : IDataRecord
        {
            foreach (var repo in Repositories)
            {
                if (typeof(TObjectType).IsAssignableFrom(repo.Key))
                {
                    foreach (var item in repo.Value.GetAll())
                    {
                        yield return (TObjectType)item;
                    }
                }
            }
        }

        public IEnumerable AllOf(Type o)
        {

            if (!typeof (IDataRecord).IsAssignableFrom(o)) yield break;

            foreach (var repo in Repositories)
            {
                if (o.IsAssignableFrom(repo.Key))
                {
                    foreach (var item in repo.Value.GetAll())
                    {
                        yield return item;
                    }
                }
            }

        }

        public List<ExportedRepository> Export()
        {
            var list = Repositories.Select(p => new ExportedRepository()
            {
                Records = p.Value.GetAll().Select(record=>new ExportedRecord()
                {
                    Data = InvertJsonExtensions.SerializeObject(record).ToString(true),
                    Identifier = record.Identifier
                }).ToList(),
                Type = p.Key.FullName
            }).ToList();
            return list;
        }
        public void Import(List<ExportedRepository> exportedItems)
        {
            foreach (var item in exportedItems)
            {
                var type = InvertApplication.FindType(item.Type);
                if (type != null)
                {
                    var repository = GetRepositoryFor(type);
                    foreach (var record in item.Records)
                    {
                        repository.Import(record);
                    }
                    
                }
            }
        }

        public void Commit()
        {
            foreach (var item in Repositories)
            {
                item.Value.Commit();
            }
        }

        public void RemoveAll<TObjectType>()
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            foreach (var item in repo.GetAll())
            {
                repo.Remove(item);
            }
        }
        public void RemoveAll<TObjectType>(Predicate<TObjectType> expression) where TObjectType : class
        {
            var repo = GetRepositoryFor(typeof(TObjectType));
            foreach (var item in repo.GetAll().Where(x=>expression(x as TObjectType)))
            {
                repo.Remove(item);
            }
        }

        public void MarkDirty(IDataRecord graphData)
        {
            
        }

        public string GetUniqueName(string s)
        {
            // TODO 2.0 ??? Unique Names
            return s;

        }

        public void Remove(IDataRecord record)
        {
            var repo = GetRepositoryFor(record.GetType());
            repo.Remove(record);
        }

        public void Reset()
        {
            Repositories.Clear();
        }
    }
}