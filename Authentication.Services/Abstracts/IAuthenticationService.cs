using Authentication.Common;
using RentRide.AuthenticationApi.Models;
using RentRide.AuthenticationApi.Models.Requests;

namespace Authentication.Services.Abstracts;

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterAsync(User user, string password);
    Task<AuthenticationResult> SignInAsync(LoginRequestModel user);
    Task SignOutAsync();
}