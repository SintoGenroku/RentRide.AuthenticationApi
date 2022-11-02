using System.Collections.ObjectModel;
using Authentication.Common;
using Authentication.Data.Core;
using Authentication.Data.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly DbContext _context;
    public UserRepository(DbContext context) : base(context)
    {
        _context = context;
    }

    public ReadOnlyCollection<User> GetAllUsers()
    {
        var users = GetUserQuery().ToList().AsReadOnly();

        return users;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        var user = await GetUserQuery()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user;
    }

    public async Task<User> GetByNameAsync(string name)
    {
        var user = await GetUserQuery()
            .FirstOrDefaultAsync(user => user.Username == name);

        return user;
    }

    public async Task UpdateActivityAsync(User user)
    {
        Data.Attach(user);
        Data.Entry(user).Property("IsActive").IsModified = true;

        await _context.SaveChangesAsync();
    }

    private IQueryable<User> GetUserQuery()
    {
        return Data
            .Include(user => user.Roles);
    }
}