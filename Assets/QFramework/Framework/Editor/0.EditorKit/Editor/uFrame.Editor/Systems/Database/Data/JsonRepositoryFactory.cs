using System;
using System.Collections.Generic;
using System.IO;

namespace Invert.Data
{
    public class JsonRepositoryFactory : ITypeRepositoryFactory
    {
        private DirectoryInfo _directory;
        public string RootPath { get; set; }

        public DirectoryInfo Directory
        {
            get { return _directory ?? (_directory = new DirectoryInfo(RootPath)); }
            set { _directory = value; }
        }
        public JsonRepositoryFactory(string rootPath)
        {
            RootPath = rootPath;
            if (!Directory.Exists)
            {
                Directory.Create();
            }
        }

        public IEnumerable<IDataRecordManager> CreateAllManagers(IRepository repository)
        {
            foreach (var item in Directory.GetDirectories())
            {
                var fullname = item.Name;
                Type type = null;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
             
                    type = assembly.GetType(fullname);
                    if (type != null)
                    {
                     
                        break;
                    } 
                }
                if (type == null)
                {
                    //throw new Exception(string.Format("Database didn't load entirely, {0} type not found, delete or rename folder {1}",fullname,item.FullName));
                    continue;
                }
                yield return CreateRepository(repository,type);
            }
        }

        public IDataRecordManager CreateRepository(IRepository repository, Type type)
        {
            return new FastJsonFileRecordManager(repository, RootPath, type);
        }
    }
}