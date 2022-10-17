using Authentication.Common;

namespace Authentication.Services.Abstracts;

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterAsync(User user, string password);
    Task<AuthenticationResult> SignInAsync(User user, string password);
    Task SignOutAsync();
}