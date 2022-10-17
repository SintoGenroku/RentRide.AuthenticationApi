using Microsoft.EntityFrameworkCore;
using Authentication.Common;
using Authentication.Data.Core;
using Authentication.Data.Repositories.Abstracts;

namespace Authentication.Data.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{

    public RoleRepository(DbContext context) : base(context)
    {
    }
    
    public async Task<Role> GetByIdAsync(Guid id)
    {
        var result = await Data.FirstOrDefaultAsync(role => role.Id == id);

        return result;
    }

    public async Task<Role> GetByNameAsync(string name)
    {
        var result = await Data.FirstOrDefaultAsync(role => role.Name == name);

        return result;
    }
}