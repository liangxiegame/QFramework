using System;
using System.Collections.Generic;

namespace Invert.Data
{
    public interface ITypeRepositoryFactory
    {
        IDataRecordManager CreateRepository(IRepository typeDatabase, Type type);
        IEnumerable<IDataRecordManager> CreateAllManagers(IRepository repository);
    }
}