using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Invert.Data;
using QF;
using QF.Json;

namespace Invert.Data
{
    public class FastJsonFileRecordManager : IDataRecordManager
    {
        public IRepository Repository { get; set; }
        private Dictionary<string, IDataRecord> _cached;
        private DirectoryInfo _directoryInfo;
        private HashSet<string> _removed;

        public string RootPath { get; set; }
        private bool _isCommiting = false;
        public void Initialize(IRepository repository)
        {
            Repository = repository;
            // Watcher = new FileSystemWatcher(DirectoryInfo.FullName, "*.json");
            //   Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
            //| NotifyFilters.FileName | NotifyFilters.DirectoryName | ;
            //Watcher.EnableRaisingEvents = true;
            //Watcher.Changed += (sender, args) =>
            //{
            //    if (!_isCommiting)
            //    {
            //        _cached = null;
            //        _loadedCached = false;
            //        Repository.Signal<IDataRecordManagerRefresh>(_ => _.ManagerRefreshed(this));
            //    }
            //};
            //Watcher.Created += (sender, args) =>
            //{
            //    if (!_isCommiting)
            //    {
            //        _cached = null;
            //        _loadedCached = false;
            //        Repository.Signal<IDataRecordManagerRefresh>(_=>_.ManagerRefreshed(this));
            //    }
            //};

            //Watcher.Deleted += (sender, args) =>
            //{
            //    if (!_isCommiting)
            //    {
            //        _cached = null;
            //        _loadedCached = false;
            //        Repository.Signal<IDataRecordManagerRefresh>(_=>_.ManagerRefreshed(this));
            //    }
            //};
        }

        public FileSystemWatcher Watcher { get; set; }

        public Type For { get; set; }

        public PropertyInfo[] ForiegnKeys
        {
            get { return _foriegnKeys ?? (_foriegnKeys = For.GetProperties(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public).Where(p=>p.IsDefined(typeof(KeyProperty),true)).ToArray()); }
            set { _foriegnKeys = value; }
        }

        //Caching value of Records path to reduce Path.Combine invokations
        public string RecordsPath
        {
            get { return _recordsPath ?? ( _recordsPath = Path.Combine(RootPath, For.FullName)); }
        }

        public FastJsonFileRecordManager(IRepository repository, string rootPath, Type @for)
        {
            RootPath = rootPath;
            For = @for;
            Repository = repository;
            Initialize(repository);
        }

        public DirectoryInfo DirectoryInfo
        {
            get { return _directoryInfo ?? (_directoryInfo = new DirectoryInfo(RecordsPath)); }
            set { _directoryInfo = value; }
        }

        private bool _loadedCached;
        private string _recordsPath;
        private PropertyInfo[] _foriegnKeys;

        private void LoadRecordsIntoCache()
        {
            if (_loadedCached) return;

            if (!DirectoryInfo.Exists)
            {
                DirectoryInfo.Create();
            }

            //Using code stolen from CodeProject which iterates over files 8x faster. (lag reduction was around 1/6 according to the compiler
            foreach (var file in DirectoryInfo.GetFiles("*"))
            {
                LoadRecord(file.FullName);
            }
            _loadedCached = true;
        }



        private string ReadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        private void LoadRecord(string file)
        {
            if (Cached.ContainsKey(Path.GetFileNameWithoutExtension(file))) return;
            try
            {
                var record = InvertJsonExtensions.DeserializeObject(For, JSON.Parse(ReadFile(file))) as IDataRecord;
                if (record != null)
                {
                    record.Repository = this.Repository;

                    Cached.Add(record.Identifier, record);
                    record.Changed = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error parsing file {0}", file), ex);
            }

        }

        public IDataRecord GetSingle(string identifier)
        {

            LoadRecordsIntoCache();

            if (!Cached.ContainsKey(identifier))
            {

                return null;
            }
            return Cached[identifier];
        }

        public IEnumerable<IDataRecord> GetAll()
        {

            LoadRecordsIntoCache();

            return Cached.Values.Where(p => !Removed.Contains(p.Identifier));
        }

        public void Add(IDataRecord o)
        {
            if (Removed.Contains(o.Identifier))
                Removed.Remove(o.Identifier);

            o.Changed = true;
            if (string.IsNullOrEmpty(o.Identifier))
            {
                o.Identifier = Guid.NewGuid().ToString();
            }
            o.Repository = this.Repository;
            if (!Cached.ContainsKey(o.Identifier))
            {
                Cached.Add(o.Identifier, o);
                Repository.Signal<IDataRecordInserted>(_ => _.RecordInserted(o));
            }
        }

        public void Commit()
        {
            _isCommiting = true;
            if (!DirectoryInfo.Exists)
            {
                DirectoryInfo.Create();
            }
            foreach (var item in Cached)
            {
                //filename is calculated inside of the if scopes, because, Path.Combine seems to be a bottleneck. ECSDemoProject took around 4000 invokcations to this method and took average
                //time of 1000ms to process all the Path.Combine.
                if (Removed.Contains(item.Key))
                {
                    var filename = Path.Combine(RecordsPath, item.Key + ".json");

                    if (File.Exists(filename))
                        File.Delete(filename);
                }
                else
                {
                    if (item.Value.Changed)
                    {
                        var filename = Path.Combine(RecordsPath, item.Key + ".json");
                        var json = InvertJsonExtensions.SerializeObject(item.Value);
                        File.WriteAllText(filename, json.ToString(true));
                    }
                    item.Value.Changed = false;
                }
            }
            _isCommiting = false;
        }

        public void Remove(IDataRecord item)
        {
            Repository.Signal<IDataRecordRemoving>(_ => _.RecordRemoving(item));
            Removed.Add(item.Identifier);
            Repository.Signal<IDataRecordRemoved>(_ => _.RecordRemoved(item));
        }

        public void Import(ExportedRecord record)
        {
            LoadRecordsIntoCache();
            if (Cached.ContainsKey(record.Identifier))
            {
                InvertJsonExtensions.DeserializeExistingObject(Cached[record.Identifier],
                    JSON.Parse(record.Data).AsObject);
            }
            else
            {
                var dataRecord = InvertJsonExtensions.DeserializeObject(For, JSON.Parse(record.Data)) as IDataRecord;
                if (dataRecord != null)
                {
                    Add(dataRecord);
                }
            }
        }

        public HashSet<string> Removed
        {
            get { return _removed ?? (_removed = new HashSet<string>()); }
            set { _removed = value; }
        }

        public Dictionary<string, IDataRecord> Cached
        {
            get { return _cached ?? (_cached = new Dictionary<string, IDataRecord>(StringComparer.OrdinalIgnoreCase)); }
            set { _cached = value; }
        }
    }
}
