using System.Runtime.CompilerServices;
using Authentication.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Core;

public class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _instances;
    private readonly Dictionary<Type, Type> _specificRepositories;
    private readonly DbContext _dbContext;

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
        _instances = new Dictionary<Type, object>();
        _specificRepositories = new Dictionary<Type, Type>();
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (!_instances.ContainsKey(typeof(T)))
        {
            if (!_specificRepositories.TryGetValue(typeof(T), out var repositoryType))
            {
                repositoryType = typeof(Repository<T>);
            }
            _instances.Add(typeof(T), Activator.CreateInstance(repositoryType,_dbContext));
        }

        return (IRepository<T>)_instances[typeof(T)];
    }

    protected void AddSpecificRepository<TEntity, TRepositoryType>()
    {
        _specificRepositories.Add(typeof(TEntity), typeof(TRepositoryType));
    }
}