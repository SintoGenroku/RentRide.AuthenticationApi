using System.Collections.ObjectModel;
using Authentication.Common;

namespace Authentication.Services.Abstracts;

public interface IUserService
{
    Task<ReadOnlyCollection<User>> GetUsersAsync();

    Task DeleteUserAsync(User user);

    Task<User> GetUserByIdAsync(Guid id);

    Task UpdateAsync(User user);

    Task<User> GetByNameAsync(string username);

    Task UpdateUserActivityAsync(User user);
}