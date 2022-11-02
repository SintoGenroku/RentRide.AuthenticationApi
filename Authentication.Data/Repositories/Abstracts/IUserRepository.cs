using System.Collections.ObjectModel;
using Authentication.Common;
using Authentication.Data.Contracts;

namespace Authentication.Data.Repositories.Abstracts;

public interface IUserRepository : IRepository<User>
{
    ReadOnlyCollection<User> GetAllUsers();

    Task<User> GetByIdAsync(Guid id);

    Task<User> GetByNameAsync(string name);

    Task UpdateActivityAsync(User user);
}