using Authentication.Common;
using Authentication.Services.Abstracts;
using Microsoft.AspNetCore.Identity;
using RentRide.AuthenticationApi.Models.Requests;

namespace Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    public async Task<AuthenticationResult> SignInAsync(LoginRequestModel userRequestModel)
    {

        var user = await _userManager.FindByNameAsync(userRequestModel.username);
        if (user == null)
        {
            return AuthenticationResult.SignInFailed(new List<string>{"Invalid username"});
        }

        var result  = await _signInManager.PasswordSignInAsync(userRequestModel.username, userRequestModel.password, false, false );

        return ConvertToServiceResult(result);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
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
    
    private static AuthenticationResult ConvertToServiceResult(SignInResult result)
    {
        if (!result.Succeeded)
        {
            var errors = new List<string> { "invalid password" };

            return AuthenticationResult.SignInFailed(errors);
        }

        return AuthenticationResult.RegistrationSuccessful();
    }
}