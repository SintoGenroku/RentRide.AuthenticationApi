using System.Collections.ObjectModel;
using Authentication.Common;

namespace Authentication.Services.Abstracts;

public interface IUserService
{
    Task<ReadOnlyCollection<User>> GetUsersAsync();

    Task DeleteUsersAsync(User user);

    Task<User> GetUserByIdAsync(Guid id);

    Task UpdateAsync(User user);

    Task<User> GetByNameAsync(string username);
}