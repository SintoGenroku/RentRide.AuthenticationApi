using System.Collections.ObjectModel;
using Authentication.Common;
using Authentication.Common.Exceptions;
using Authentication.Data.Abstracts;
using Authentication.Services.Abstracts;

namespace Authentication.Services;

public class UserService : IUserService
{
    private readonly IAuthenticationUnitOfWork _unitOfWork;


    public UserService(IAuthenticationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User doesn't exist");
        }
            
        return user;
    }

    public async Task<ReadOnlyCollection<User>> GetUsersAsync()
    {
        var users =  _unitOfWork.Users.GetAllUsers();

        return users;
    }

    public async Task DeleteUsersAsync(User user)
    { 
        await _unitOfWork.Users.DeleteAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _unitOfWork.Users.UpdateAsync(user);
    }

    public async Task<User> GetByNameAsync(string username)
    {
        var user = await _unitOfWork.Users.GetByNameAsync(username);

        return user;
    }

    public async Task UpdateUserActivityAsync(User user)
    {
        await _unitOfWork.Users.UpdateActivityAsync(user);
    }
}