using Authentication.Common;
using Authentication.Services.Abstracts;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
        private readonly UserManager<User> _userManager;

    public AuthenticationService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> RegisterAsync(User user, string password)
    {
        user.Roles ??= new List<Role>();

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return ConvertToServiceResult(result);
        }
        await _userManager.AddToRoleAsync(user, RoleNames.Client);
        
        return ConvertToServiceResult(result);
    }
    public Task<AuthenticationResult> SignInAsync(User user, string password)
    {
        throw new NotImplementedException();
    }

    public Task SignOutAsync()
    {
        throw new NotImplementedException();
    }
       
    private static AuthenticationResult ConvertToServiceResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToArray();

            return AuthenticationResult.RegistrationFailed(errors);
        }

        return AuthenticationResult.RegistrationSuccessful();
    }
}