using Authentication.Data.Contracts;
using Authentication.Data.Repositories.Abstracts;

namespace Authentication.Data.Abstracts;

public interface IAuthenticationUnitOfWork : IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
}